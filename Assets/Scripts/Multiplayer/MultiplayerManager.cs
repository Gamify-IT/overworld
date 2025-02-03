using NativeWebSocket;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
///     Manages the multiplayer communication with other clients. 
///     Includes communication protocol definition, (de)serializing and the websocket connection.
///     Inlcudes, 
/// </summary>
public class MultiplayerManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetToken(string tokenName);

    private WebSocket websocket;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject remotePlayerParent;
    private Dictionary<string, GameObject> connectedRemotePlayers;
    private bool connected = false;

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
    ///     Creates a websocket for communicating with the multiplayer server. 
    /// </summary>
    public async Task Initialize()
    {
        websocket = new WebSocket("ws://127.0.0.1:3000");
        connected = true;

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            connectedRemotePlayers = new();
            EventManager.Instance.OnDataChanged += SendData;
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            connected = false;
            EventManager.Instance.OnDataChanged -= SendData;

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
                    Debug.LogError("Unknown trigger event!");
                    break;
            }
        }
    }

    /// <summary>
    ///     Updates the remote player using received data.
    /// </summary>
    /// <param name="message">received message to update the player</param>
    private void UpdateRemotePlayer(NetworkMessage message)
    {
        GameObject remotePlayerPrefab = null;
        GetOrCreateRemotePlayer(message, ref remotePlayerPrefab);
        RemotePlayerAnimation remotePlayer = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();

        switch (message)
        {
            case PositionMessage positionMessage:
                remotePlayer.UpdatePosition(positionMessage.GetPosition(), positionMessage.GetMovement());
                break;
            case CharacterMessage characterMessage:
                remotePlayer.UpdateCharacterOutfit(characterMessage.GetHead(), characterMessage.GetBody());
                break;
            default:
                throw new Exception("Unknown message type!");
        }
    }

    /// <summary>
    ///     Handles the creation of a new remote player if it does not exist or gets the existing one.
    /// </summary>
    /// <param name="message">received network message</param>
    /// <param name="remotePlayerPrefab">reference to the remote player GameObject</param>
    private void GetOrCreateRemotePlayer(NetworkMessage message, ref GameObject remotePlayerPrefab)
    {
        if (!connectedRemotePlayers.TryGetValue(message.playerId, out remotePlayerPrefab))
        {
            if (message is PositionMessage positionMessage)
            {
                remotePlayerPrefab = Instantiate(prefab, positionMessage.GetPosition(), Quaternion.identity, remotePlayerParent.transform);
                connectedRemotePlayers.Add(message.playerId, remotePlayerPrefab);
            }
            else
            {
                throw new Exception("First message must be a position message!");
            }
        }
    }

    /// <summary>
    ///     Closes the websocjet connection and event trigger subscription.
    /// </summary>
    private async void OnApplicationQuit()
    {
        EventManager.Instance.OnDataChanged -= SendData;
        connected = false;
        await websocket.Close();
    }
}