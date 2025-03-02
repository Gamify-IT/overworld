using Cysharp.Threading.Tasks;
using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
///     Manages the multiplayer communication with other clients. 
/// </summary>
public class MultiplayerManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetToken(string tokenName);

    // websocket connection
    private WebSocket websocket;
    private bool isConnected = false;
    private ushort clientId = 0;
    private const string multiplayerApiPath = "/multiplayer-server/api/v1";
    private const string multiplayerSocketPath = "/multiplayer-server/ws";

    // remote player data
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject remotePlayerParent;
    private readonly Dictionary<ushort, GameObject> connectedRemotePlayers = new();
    private readonly Dictionary<ushort, GameObject> inactiveRemotePlayers = new();

    // timeout routine
    private bool isInactive = false;
    private float lastDataChangedTime = 0f;

    #region singelton
    public static MultiplayerManager Instance { get; private set; }
    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Update()
    {
        if (isConnected)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }
        CheckForTimeout();
    }

    #region time based routines
    /// <summary>
    ///     Checks if the player is inactive and pauses the multiplayer.
    ///     If the player is active again, the multiplayer is activated.
    /// </summary>
    private async void CheckForTimeout()
    {
        if (Time.realtimeSinceStartup - lastDataChangedTime > 50f)
        {
            if (isConnected && !isInactive)
            {
                isInactive = true;
                await PauseMultiplayer();
            }
        }
        else
        {
            if (!isConnected && isInactive)
            {
                isInactive = false;
                await ResumeMultiplayer();
            }
        }
    }
    #endregion

    #region websocket connection
    /// <summary>
    ///     Contacts the multiplayer server to establish a new connection.
    /// </summary>
    /// <returns>was the connection successful?</returns>
    public async UniTask<bool> InitConnection()
    {
        string id = clientId != 0 ? clientId.ToString() : string.Empty;
        ConnectionDTO connectionDTO = new(GameManager.Instance.GetUserId(), id, GameManager.Instance.GetCourseId());
        string basePath = multiplayerApiPath + "/join";
        string json = JsonUtility.ToJson(connectionDTO, true);

        Optional<ResponseDTO> data = await RestRequest.PostRequest<ResponseDTO>(basePath, json);

        if (data.IsPresent())
        {
            clientId = data.Value().clientId;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///     Creates a websocket for communicating with the multiplayer server and handles it throughout a session.
    /// </summary>
    public async Task InitializeWebsocket()
    {
        OpenWebSocket();
        StartCoroutine(InitializeDictionarys());

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open");
            SendInitialData();
            lastDataChangedTime = Time.realtimeSinceStartup;
            EventManager.Instance.OnDataChanged += SendData;
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed");
        };

        websocket.OnMessage += (bytes) =>
        {
            try
            {
                ProcessMessage(NetworkMessage.Deserialize(ref bytes));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error on processing message: {e.Message}\n{e.StackTrace}");
            }
        };

        await websocket.Connect();
    }

    /// <summary>
    ///     Opens a new websocket connection with the mutliplayer server.
    /// </summary>
    private void OpenWebSocket()
    {
        string host = new Uri(Application.absoluteURL).Host;
        string protocol = Application.absoluteURL.StartsWith("https://") ? "wss://" : "ws://";
        try
        {
            websocket = new WebSocket(protocol + host + multiplayerSocketPath);
            isConnected = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error on opening websocket connection: " + e);
            GameObject.Find("Multiplayer Canvas").GetComponent<MultiplayerUI>().ShowFeedbackWindow(FeedbackType.Error);
        }
    }
    #endregion

    #region message sending
    /// <summary>
    ///     Sends the player's data to the server if one of the event trigger was activated.
    /// </summary>
    /// <param name="trigger">triggering event</param>
    private async void SendData(object sender, EventArgs trigger)
    {
        lastDataChangedTime = Time.realtimeSinceStartup;

        if (isConnected)
        {
            if (websocket.State == WebSocketState.Open)
            {
                switch (trigger)
                {
                    case EventArgsWrapper<PositionMessage> positionTrigger:
                        await websocket.Send(positionTrigger.GetMessage().Serialize());
                        break;
                    case EventArgsWrapper<CharacterMessage> characterTrigger:
                        await websocket.Send(characterTrigger.GetMessage().Serialize());
                        break;
                    default:
                        Debug.LogError("Unknown trigger event");
                        break;
                }
            }
        }
    }

    /// <summary>
    ///     Sends a connection message with the local player's current data to the server.
    /// </summary>
    private async void SendInitialData()
    {
        Vector2 startPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        AreaLocationDTO currentArea = DataManager.Instance.GetPlayerData().GetCurrentArea();
        string head = DataManager.Instance.GetPlayerData().GetCurrentAccessory();
        string body = DataManager.Instance.GetPlayerData().GetCurrentCharacter();
        ConnectionMessage connectionMessage = new(
            clientId,
            startPosition,
            (byte)currentArea.worldIndex, (byte)currentArea.dungeonIndex,
            head, body
        );

        await websocket.Send(connectionMessage.Serialize());
    }

    /// <summary>
    ///     Sends an acknowledge message with the local player's current data to the server as response of a connection message.
    /// </summary>
    private async void SendAcknowledgeData()
    {
        Vector2 startPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        AreaLocationDTO currentArea = DataManager.Instance.GetPlayerData().GetCurrentArea();
        string head = DataManager.Instance.GetPlayerData().GetCurrentAccessory();
        string body = DataManager.Instance.GetPlayerData().GetCurrentCharacter();
        AcknowledgeMessage ackMessage = new(
            clientId, 
            startPosition,
            (byte)currentArea.worldIndex, (byte)currentArea.dungeonIndex,
            head, body
        );

        await websocket.Send(ackMessage.Serialize());
    }
    #endregion

    #region message processing
    /// <summary>
    ///     Updates the remote player depending on the received message type.
    /// </summary>
    /// <param name="message">received message used to update the player</param>
    private void ProcessMessage(NetworkMessage message)
    {
        GameObject remotePlayerPrefab = null;
        RemotePlayerAnimation remotePlayer;

        switch (message)
        {
            case ConnectionMessage connectionMessage:
                if (GetOrCreateRemotePlayer(connectionMessage, ref remotePlayerPrefab))
                {
                    remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                    remotePlayer.ShowComponents(true);
                    SendAcknowledgeData();
                }
                return;
            case DisconnectionMessage disconnectMessage:
                RemoveRemotePlayer(disconnectMessage);
                return;
            case AcknowledgeMessage acknowledgeMessage:
                if (GetOrCreateRemotePlayer(acknowledgeMessage, ref remotePlayerPrefab))
                {
                    remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                    remotePlayer.ShowComponents(true);
                }
                return;
            case PositionMessage positionMessage:
                if (connectedRemotePlayers.TryGetValue(message.GetClientId(), out remotePlayerPrefab))
                {
                    remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                    remotePlayer.UpdatePosition(positionMessage.GetPosition(), positionMessage.GetMovement());
                }
                return;
            case CharacterMessage characterMessage:
                if (connectedRemotePlayers.TryGetValue(message.GetClientId(), out remotePlayerPrefab))
                {
                    remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                    remotePlayer.UpdateCharacterOutfit(characterMessage.GetHead(), characterMessage.GetBody());
                }
                return;
            case AreaMessage areaMessage:
                if (connectedRemotePlayers.TryGetValue(message.GetClientId(), out remotePlayerPrefab))
                {
                    remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                    remotePlayer.UpdateAreaInformation(areaMessage.GetWorldIndex(), areaMessage.GetDungeonIndex());
                }
                return;
            case TimeoutMessage:
                if(connectedRemotePlayers.TryGetValue(message.GetClientId(), out remotePlayerPrefab))
                {
                    connectedRemotePlayers.Remove(message.GetClientId());
                    inactiveRemotePlayers.Add(message.GetClientId(), remotePlayerPrefab);
                    remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                    remotePlayer.ShowComponents(false);
                }
                return;
            default:
                throw new Exception("Unknown message type");
        }
    }
    #endregion

    #region multiplayer connection handling
    /// <summary>
    ///     Terminates the multiplayer connection completely.
    ///     Includes sending a disconnection message to the server, closing the websocket connection and 
    ///     unsubscribing from event triggers.
    /// </summary>
    public async UniTask<bool> QuitMultiplayer()
    {
        if (isConnected)
        {
            EventManager.Instance.OnDataChanged -= SendData;
            DisconnectionMessage disconnectMessage = new(clientId);
            await websocket.Send(disconnectMessage.Serialize());
            await websocket.Close();
            RemoveAllRemotePlayers();
            isConnected = false;
            Debug.Log("Quitted multiplayer");
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Terminates the multiplayer connection with the option to reconnect.
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> PauseMultiplayer()
    {
        if (isConnected)
        {
            TimeoutMessage timeoutMessage = new(clientId);
            await websocket.Send(timeoutMessage.Serialize());
            await websocket.Close();
            isConnected = false;
            Debug.Log("Paused multiplayer");
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Resumes the multiplayer if the player was inactive and is now active again.
    /// </summary>
    /// <returns></returns>
    private async UniTask ResumeMultiplayer()
    {
        bool successful = await InitConnection();

        if (successful)
        {
            Debug.Log("Reconnecting to multiplayer");
            InitializeWebsocket();
        }
        else
        {
            Debug.LogError("Reconnection to server failed");
            // TODO: add UI feedback
        }
    }
    #endregion

    #region remote player handling
    /// <summary>
    ///     Deletes all exisiting remote players and removes them from the connected players and inactive players dictionarys. 
    /// </summary>
    private IEnumerator InitializeDictionarys()
    {
        yield return new WaitForEndOfFrame();

        // delete existing prefabs
        foreach (var remotePlayer in connectedRemotePlayers)
        {
            Destroy(remotePlayer.Value);
        }

        foreach (var inactivePlayer in inactiveRemotePlayers)
        {
            Destroy(inactivePlayer.Value);
        }

        // clear dictionarys
        connectedRemotePlayers.Clear();
        inactiveRemotePlayers.Clear();
    }

    /// <summary>
    ///     Handles the creation of a new remote player, if non-existent, or gets the existing one.
    /// </summary>
    /// <param name="message">received network message</param>
    /// <param name="remotePlayerPrefab">reference to the remote player GameObject</param>
    private bool GetOrCreateRemotePlayer(NetworkMessage message, ref GameObject remotePlayerPrefab)
    {
        // check if remote player is connected
        if (!connectedRemotePlayers.TryGetValue(message.GetClientId(), out remotePlayerPrefab))
        {
            // test if remote player was inactive
            if (inactiveRemotePlayers.TryGetValue(message.GetClientId(), out remotePlayerPrefab))
            {
                inactiveRemotePlayers.Remove(message.GetClientId());
                connectedRemotePlayers.Add(message.GetClientId(), remotePlayerPrefab);
                RemotePlayerAnimation remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                switch(message)
                {
                    case ConnectionMessage connectionMessage:
                        remotePlayer.UpdatePosition(connectionMessage.GetStartPosition(), Vector2.zero); 
                        break;
                    case AcknowledgeMessage acknowledgeMessage:
                        remotePlayer.UpdatePosition(acknowledgeMessage.GetStartPosition(), Vector2.zero);
                        break;
                    default:
                        Debug.Log("Waiting for connection or acknowledge message to reconnect a player");
                        return false;
                }                           
                return true;
            }
            else if (message is ConnectionMessage connectionMessage)
            {
                CreateRemotePlayer(connectionMessage, out remotePlayerPrefab);
                return true;
            }
            else if (message is AcknowledgeMessage acknowledgeMessage)
            {
                CreateRemotePlayer(acknowledgeMessage, out remotePlayerPrefab);
                return true;
            }
            else
            {
                Debug.Log("Waiting for connection or acknowledge message to create new player");
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    ///     Creates a new prefab for a remote player based on the connection or acknowledge message.
    /// </summary>
    /// <param name="message">received connection or acknowledge message</param>
    /// <param name="remotePlayerPrefab">remote player prefab object</param>
    private void CreateRemotePlayer(NetworkMessage message, out GameObject remotePlayerPrefab)
    {
        RemotePlayerAnimation remotePlayer;
        switch (message)
        {
            case ConnectionMessage connectionMessage:
                remotePlayerPrefab = Instantiate(prefab, connectionMessage.GetStartPosition(), Quaternion.identity, remotePlayerParent.transform);
                connectedRemotePlayers.Add(message.GetClientId(), remotePlayerPrefab);
                remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.Initialize();
                remotePlayer.UpdateCharacterOutfit(connectionMessage.GetHead(), connectionMessage.GetBody());
                remotePlayer.UpdateAreaInformation(connectionMessage.GetWorldIndex(), connectionMessage.GetDungeonIndex());
                return;
            case AcknowledgeMessage acknowledgeMessage:
                remotePlayerPrefab = Instantiate(prefab, acknowledgeMessage.GetStartPosition(), Quaternion.identity, remotePlayerParent.transform);
                connectedRemotePlayers.Add(message.GetClientId(), remotePlayerPrefab);
                remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.Initialize();
                remotePlayer.UpdateCharacterOutfit(acknowledgeMessage.GetHead(), acknowledgeMessage.GetBody());
                remotePlayer.UpdateAreaInformation(acknowledgeMessage.GetWorldIndex(), acknowledgeMessage.GetDungeonIndex());
                return;
            default:
                Debug.Log("Waiting for connection or acknowledge message to reconnect a player");
                remotePlayerPrefab = null;
                return;
        }
    }

    /// <summary>
    ///     Removes the player from the isConnected players dictionary and destroys its prefab.
    /// </summary>
    /// <param name="disconnectMessage">received network message</param>
    private void RemoveRemotePlayer(DisconnectionMessage disconnectMessage)
    {
        if (connectedRemotePlayers.TryGetValue(disconnectMessage.GetClientId(), out GameObject remotePlayerPrefab))
        {
            Destroy(remotePlayerPrefab);
            connectedRemotePlayers.Remove(disconnectMessage.GetClientId());
            inactiveRemotePlayers.Remove(disconnectMessage.GetClientId());
        }
        else
        {
            Debug.LogError("Remote player not found");
        }
    }

    /// <summary>
    ///     Removes all players from isConnected players dictionary and destroys their prefab.
    /// </summary>
    private void RemoveAllRemotePlayers()
    {
        foreach (Transform child in remotePlayerParent.transform)
        {
            Destroy(child.gameObject);
        }
        connectedRemotePlayers.Clear();
    }
    #endregion

    #region getter
    public bool IsConnected()
    {
        return isConnected;
    }
    public ushort GetPayerId()
    {
        return clientId;
    }
    public int GetNumberOfConnectedPlayers()
    {
        return remotePlayerParent.transform.childCount;
    }
    #endregion
}