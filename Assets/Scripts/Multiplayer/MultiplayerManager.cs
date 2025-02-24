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

    private WebSocket websocket;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject remotePlayerParent;
    private Dictionary<byte, GameObject> connectedRemotePlayers;
    private bool connected = false;
    private byte playerId;

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
        }
    }

    /// <summary>
    ///     Creates a websocket for communicating with the multiplayer server and handles it.
    /// </summary>
    public async Task Initialize()
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
                UpdateRemotePlayer(NetworkMessage.Deserialize(ref bytes));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error on processing message: {e.Message}\n{e.StackTrace}");
            }
        };

        await websocket.Connect();
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
        Debug.Log("Sending intial data");
        Vector2 startPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        AreaLocationDTO currentArea = DataManager.Instance.GetPlayerData().GetCurrentArea();
        string head = DataManager.Instance.GetPlayerData().GetCurrentAccessory();
        string body = DataManager.Instance.GetPlayerData().GetCurrentCharacter();
        ConnectionMessage connectionMessage = new(
            playerId, startPosition,
            (byte) currentArea.worldIndex, (byte) currentArea.dungeonIndex,
            head, body);
        Debug.Log("Sending inital data:" + connectionMessage.GetStartPosition().ToString() + ", " + connectionMessage.GetBody() + ", " + connectionMessage.GetHead());
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
    #endregion

    /// <summary>
    ///     Updates the remote player depending of the received message type.
    /// </summary>
    /// <param name="message">received message to update the player</param>
    private void UpdateRemotePlayer(NetworkMessage message)
    {
        GameObject remotePlayerPrefab = null;
        GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
        RemotePlayerAnimation remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();

        switch (message)
        {
            case ConnectionMessage:
                SendAcknowledgeData();
                break;
            case DisconnectionMessage disconnectMessage:
                RemoveRemotePlayer(disconnectMessage);
                break;
            case AcknowledgeMessage:
                break;
            case PositionMessage positionMessage:
                remotePlayer.UpdatePosition(positionMessage.GetPosition(), positionMessage.GetMovement());
                break;
            case CharacterMessage characterMessage:
                remotePlayer.UpdateCharacterOutfit(characterMessage.GetHead(), characterMessage.GetBody());
                break;
            case AreaMessage areaMessage:
                remotePlayer.UpdateAreaInformation(areaMessage.GetWorldIndex(), areaMessage.GetDungeonIndex());
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
            connected = false;
            Debug.Log("Quitted multiplayer");
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Removes the player from the connected players dictionary and destroys its prefab.
    /// </summary>
    /// <param name="disconnectMessage">received network message</param>
    public void RemoveRemotePlayer(DisconnectionMessage disconnectMessage)
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