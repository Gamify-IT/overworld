using Cysharp.Threading.Tasks;
using NativeWebSocket;
using System;
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
    private bool connected = false;
    private byte playerId;

    // remote player
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject remotePlayerParent;
    private Dictionary<byte, GameObject> connectedRemotePlayers;

    // ping pong routine
    private bool pongReceived = true;
    private float lastPingTime = 0f;
    private float lastPongTime = 0f;


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
        if (connected)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
            PingPongRoutine();
        }
    }

    /// <summary>
    ///     Sends a ping to the server and checks if there is a pong response.
    ///     If not, the websocket connection is closed.
    /// </summary>
    private async void PingPongRoutine()
    {
        if (Time.realtimeSinceStartup - lastPingTime > 30f)
        {
            SendPing();
        }

        if (!pongReceived && Time.unscaledDeltaTime - lastPongTime > 10f)
        {
            Debug.LogError("No pong received. Connection might be lost.");
            // TODO: add UI feedback
            await QuitMultiplayer();
        }
    }

    /// <summary>
    ///     Creates a websocket for communicating with the multiplayer server and handles it throughout a session.
    /// </summary>
    public async Task Initialize()
    {
        OpenWebSocket();

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open");
            connectedRemotePlayers = new();
            SendInitialData();
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
            websocket = new WebSocket(protocol + host + "/multiplayer/ws");
            connected = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error on opening websocket connection: " + e);
            GameObject.Find("Multiplayer Canvas").GetComponent<MultiplayerUI>().ShowFeedbackWindow(FeedbackType.Error);
        }
    }

    #region messages send events
    /// <summary>
    ///     Sends the player's data to the server if one of the event trigger was activated.
    /// </summary>
    /// <param name="trigger">triggering event</param>
    private async void SendData(object sender, EventArgs trigger)
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
            playerId, startPosition,
            (byte) currentArea.worldIndex, (byte) currentArea.dungeonIndex,
            head, body);

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
            playerId, startPosition,
            (byte)currentArea.worldIndex, (byte)currentArea.dungeonIndex,
            head, body);
       
        await websocket.Send(ackMessage.Serialize());
    }

    /// <summary>
    ///     Sends a ping message to the server.
    /// </summary>
    private async void SendPing()
    {
        pongReceived = false;
        lastPingTime = Time.realtimeSinceStartup;
        PingPongMessage ping = new(playerId);
        await websocket.Send(ping.Serialize());
        Debug.Log("sending ping");
    }
    #endregion

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
            case ConnectionMessage:
                GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
                SendAcknowledgeData();
                break;
            case DisconnectionMessage disconnectMessage:
                RemoveRemotePlayer(disconnectMessage);
                break;
            case AcknowledgeMessage:
                GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
                break;
            case PositionMessage positionMessage:
                GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
                remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.UpdatePosition(positionMessage.GetPosition(), positionMessage.GetMovement());
                break;
            case CharacterMessage characterMessage:
                GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
                remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.UpdateCharacterOutfit(characterMessage.GetHead(), characterMessage.GetBody());
                break;
            case AreaMessage areaMessage:
                GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
                remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.UpdateAreaInformation(areaMessage.GetWorldIndex(), areaMessage.GetDungeonIndex());
                break;
            case PingPongMessage:
                Debug.Log("received pong");
                pongReceived = true;
                lastPongTime = Time.realtimeSinceStartup;
                break;
            default:
                throw new Exception("Unknown message type");
        }
    }

    /// <summary>
    ///     Handles the creation of a new remote player, if non-existent, or gets the existing one.
    /// </summary>
    /// <param name="message">received network message</param>
    /// <param name="remotePlayerPrefab">reference to the remote player GameObject</param>
    private void GetOrCreateRemotePlayer(NetworkMessage message, ref GameObject remotePlayerPrefab)
    {
        if (!connectedRemotePlayers.TryGetValue(message.GetPlayerId(), out remotePlayerPrefab))
        {
            if (message is ConnectionMessage connectionMessage)
            {
                remotePlayerPrefab = Instantiate(prefab, connectionMessage.GetStartPosition(), Quaternion.identity, remotePlayerParent.transform);
                connectedRemotePlayers.Add(message.GetPlayerId(), remotePlayerPrefab);
                RemotePlayerAnimation remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.Initialize();
                remotePlayer.UpdateCharacterOutfit(connectionMessage.GetHead(), connectionMessage.GetBody());
                remotePlayer.UpdateAreaInformation(connectionMessage.GetWorldIndex(), connectionMessage.GetDungeonIndex());
            }
            else if (message is AcknowledgeMessage ackMessage)
            {
                remotePlayerPrefab = Instantiate(prefab, ackMessage.GetStartPosition(), Quaternion.identity, remotePlayerParent.transform);
                connectedRemotePlayers.Add(message.GetPlayerId(), remotePlayerPrefab);
                RemotePlayerAnimation remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
                remotePlayer.Initialize();
                remotePlayer.UpdateCharacterOutfit(ackMessage.GetHead(), ackMessage.GetBody());
                remotePlayer.UpdateAreaInformation(ackMessage.GetWorldIndex(), ackMessage.GetDungeonIndex());
            }
            else
            {
                Debug.Log("Waiting for connection or acknowledge message to create new player");
            }
        }
    }

    /// <summary>
    ///     Sends a disconnection message to the server, closes the websocket connection and unsubscribes from event triggers.
    /// </summary>
    public async UniTask<bool> QuitMultiplayer()
    {
        if (connected)
        {
            EventManager.Instance.OnDataChanged -= SendData;
            DisconnectionMessage disconnectMessage = new(playerId);
            await websocket.Send(disconnectMessage.Serialize());
            await websocket.Close();
            RemoveAllRemotePlayers();
            connected = false;
            Debug.Log("Quitted multiplayer");
            return true;
        }

        return false;
    }

    public async UniTask<bool> TimeoutMultiplayer()
    {
        return true;
    }

    /// <summary>
    ///     Removes the player from the connected players dictionary and destroys its prefab.
    /// </summary>
    /// <param name="disconnectMessage">received network message</param>
    private void RemoveRemotePlayer(DisconnectionMessage disconnectMessage)
    {
        if (connectedRemotePlayers.TryGetValue(disconnectMessage.GetPlayerId(), out GameObject remotePlayerPrefab))
        {
            Destroy(remotePlayerPrefab);
            connectedRemotePlayers.Remove(disconnectMessage.GetPlayerId());
        }
        else
        {
            Debug.LogError("Remote player not found");
        }
    }

    /// <summary>
    ///     Removes all players from connected players dictionary and destroys their prefab.
    /// </summary>
    private void RemoveAllRemotePlayers()
    {
        foreach (Transform child in remotePlayerParent.transform)
        {
            Destroy(child.gameObject);
        }
        connectedRemotePlayers.Clear();
    }

    public bool IsConnected()
    {
        return connected;
    }
    public void SetPlayerId(byte id)
    {
       playerId = id;
    }
    public byte GetPayerId()
    {
        return playerId;
    }
    public int GetNumberOfConnectedPlayers()
    {
        return remotePlayerParent.transform.childCount;
    }
}