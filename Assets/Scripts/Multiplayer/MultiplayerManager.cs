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
    private bool connected = false;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject remotePlayerParent;
    private Dictionary<string, GameObject> connectedRemotePlayers;
    private string playerId;

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

    #region event methods
    /// <summary>
    ///     Subscribes to events used as triggers for communication.
    /// </summary>
    private void EnableEvents()
    {
        EventManager.Instance.OnPositionChanged += SendData;
    }

    /// <summary>
    ///     Unsubscribes events when multiplayer is quit.
    /// </summary>
    private void DisableEvents()
    {
        EventManager.Instance.OnPositionChanged -= SendData;
    }
    #endregion

    private void Start()
    {
        // get player id
        try
        {
            playerId = GetToken("userId");
            Debug.Log("player id:" +  playerId);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogError("Function not found: " + e);

            // use mock id for development
#if UNITY_EDITOR
            playerId = "c858aea9-a744-4709-a169-9df329fe4d96";
#endif
        }
    }

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
    ///     Creates a websocket for communicating with the multiplayer server
    /// </summary>
    public async Task Initialize()
    {
        connected = true;
        websocket = new WebSocket("ws://127.0.0.1:3000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            connectedRemotePlayers = new();
            EnableEvents();
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            DisableEvents();
        };

        websocket.OnMessage += (bytes) =>
        {
            try
            {
                UpdateRemotePlayer(DeserializePlayerData(bytes));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error on processing message: {e.Message}\n{e.StackTrace}");
            }
        };

        // Keep sending messages at every
        //InvokeRepeating(nameof(SendWebSocketMessage), 1.0f, 0.03f);

        // wait for connection
        await websocket.Connect();
    }

    /// <summary>
    ///     Sends the player's data to the server if one of the players events is triggered.
    /// </summary>
    /// <param name="data">triggering event</param>
    private async void SendData(object sender, PositionChangedEventArgs data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(SerializePlayerData(playerId, 1, data.GetPosition(), data.GetMovement()));
        }
    }

    /// <summary>
    ///     Updates the remote player using received data.
    /// </summary>
    /// <param name="data">new data of hte remote player</param>
    private void UpdateRemotePlayer(RemotePlayerData data)
    {
        GameObject remotePlayerPrefab;

        if (!connectedRemotePlayers.ContainsKey(data.GetId()))
        {
            remotePlayerPrefab = Instantiate(prefab, data.GetPosition(), Quaternion.identity, remotePlayerParent.transform);
            connectedRemotePlayers.Add(data.GetId(), remotePlayerPrefab);
        }

        remotePlayerPrefab = connectedRemotePlayers[data.GetId()];

        RemotePlayerAnimation remotePlayerAnimation = remotePlayerPrefab.GetComponent<RemotePlayerAnimation>();
        remotePlayerAnimation.UpdatePosition(data.GetPosition(), data.GetMovement());

        // TODO: add clipping mechanics

        //Rigidbody2D rb  = playerPrefab.GetComponent<Rigidbody2D>();

        //rb.MovePosition(playerData.GetPosition());
    }

    #region (de)serializing
    /// <summary>
    ///     Serializes data to be send with a custom communication protocol.
    /// </summary>
    /// <param name="playerId">unique id of the player</param>
    /// <param name="character">unique character representation</param>
    /// <param name="position">player position</param>
    /// <param name="movement">normalized player movement vector</param>
    /// <returns></returns>
    private byte[] SerializePlayerData(string playerId, byte character, Vector2 position, Vector2 movement)
    {
        byte[] data = new byte[33];

        // player id (16 Bytes)
        Guid uuid = Guid.Parse(playerId);
        Buffer.BlockCopy(uuid.ToByteArray(), 0, data, 0, 16);

        // character index (1 Byte)
        data[16] = character;

        // position (2 * 4 Bytes)
        Buffer.BlockCopy(BitConverter.GetBytes(position.x), 0, data, 17, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(position.y), 0, data, 21, 4);

        // movement (2 * 4 Byte)
        Buffer.BlockCopy(BitConverter.GetBytes(movement.x), 0, data, 25, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(movement.y), 0, data, 29, 4);

        // area id (2 Bytes)
        //Buffer.BlockCopy(BitConverter.GetBytes(areaId.GetWorldIndex()), 0, data, 13, 1);
        //Buffer.BlockCopy(BitConverter.GetBytes(areaId.GetDungeonIndex()), 0, data, 14, 1);

        return data;
    }

    /// <summary>
    ///     Deserializes received data and converts it to an RemotePlayerData object.
    /// </summary>
    /// <param name="data">received message</param>
    /// <returns>data of the remote player</returns>
    private RemotePlayerData DeserializePlayerData(byte[] data)
    {
        // TODO: discard message if wrong protocol 

        // extract uuid from message
#if !UNITY_EDITOR
        byte[] playerIdBytes = new byte[16];
        Buffer.BlockCopy(data, 0, playerIdBytes, 0, 16);
        Guid playerId = new(playerIdBytes);
#endif
        return new RemotePlayerData(
            playerId.ToString(), 
            new Vector2(BitConverter.ToSingle(data, 17), BitConverter.ToSingle(data, 21)),
            new Vector2(BitConverter.ToSingle(data, 25), BitConverter.ToSingle(data, 29))
            );
    }
    #endregion

    private async void OnApplicationQuit()
    {
        connected = false;
        DisableEvents();
        await websocket.Close();
    }
}