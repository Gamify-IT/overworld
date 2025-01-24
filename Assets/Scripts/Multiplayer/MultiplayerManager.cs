using NativeWebSocket;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetToken(string tokenName);
    private WebSocket websocket;
    private bool connected = false;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject parent;
    private Dictionary<string, GameObject> connectedPlayers;
    private string playerID;

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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region event methods
    private void EnableEvents()
    {
        EventManager.OnPositionChanged += SendData;
    }

    private void DisableEvents()
    {
        EventManager.OnPositionChanged -= SendData;
    }
    #endregion

    private void Start()
    {
        try
        {
            playerID = GetToken("userId");
            Debug.Log("player id:" +  playerID);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogError("Function not found: " + e);
            playerID = "c858aea9-a744-4709-a169-9df329fe4d96";
        }
    }

    void Update()
    {
        if (connected)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }
    }

    /// <summary>
    ///     Creates a websocket for communication with the multiplayer server
    /// </summary>
    /// <returns></returns>
    public async Task Initialize()
    {
        connected = true;
        websocket = new WebSocket("ws://127.0.0.1:3000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            connectedPlayers = new Dictionary<string, GameObject>();
            EnableEvents();
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
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
                UpdatePlayer(bytes);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error on processing message: {e.Message}\n{e.StackTrace}");
            }
        };

        // Keep sending messages at every
        //InvokeRepeating(nameof(SendWebSocketMessage), 1.0f, 0.03f);

        // waiting for messages
        await websocket.Connect();
    }

    /// <summary>
    ///     Sends the player's data to the server if one of its events is triggered
    /// </summary>
    /// <param name="newPosition">trigger event</param>
    private async void SendData(object sender, object data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            Debug.Log(data.ToString());
            if (data is Vector2 newPosition)
            {
                // Sending plain text
                await websocket.Send(SerializePlayerData(playerID, 1, newPosition));
            }
        }
    }

    private byte[] SerializePlayerData(string playerId, byte character, Vector2 position)
    {
        byte[] data = new byte[25];

        // player id (16 Bytes)
        Guid uuid = Guid.Parse(playerId);
        Buffer.BlockCopy(uuid.ToByteArray(), 0, data, 0, 16);

        // character index (1 Byte)
        data[16] = character;

        // position (2 * 4 Bytes)
        Buffer.BlockCopy(BitConverter.GetBytes(position.x), 0, data, 17, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(position.y), 0, data, 21, 4);

        // area id (2 Bytes)
        //Buffer.BlockCopy(BitConverter.GetBytes(areaId.GetWorldIndex()), 0, data, 13, 1);
        //Buffer.BlockCopy(BitConverter.GetBytes(areaId.GetDungeonIndex()), 0, data, 14, 1);

        return data;
    }

    private RemotePlayerData DeserializePlayerData(byte[] data)
    {
        // TODO drop message if wrong protocol 

        // guid
        byte[] playerIdBytes = new byte[16];
        Buffer.BlockCopy(data, 0, playerIdBytes, 0, 16);
        Guid playerId = new Guid(playerIdBytes);

        return new RemotePlayerData(playerID.ToString(), new Vector2(BitConverter.ToSingle(data, 17), BitConverter.ToSingle(data, 21)));
    }

    private void UpdatePlayer(byte[] data)
    {
        RemotePlayerData playerData = DeserializePlayerData(data);
        GameObject playerPrefab;

        if (!connectedPlayers.ContainsKey(playerData.GetId()))
        {
            Debug.Log("creating new player");
            playerPrefab = Instantiate(prefab, playerData.GetPosition(), Quaternion.identity, parent.transform);
            connectedPlayers.Add(playerData.GetId(), playerPrefab);
        }

        playerPrefab = connectedPlayers[playerData.GetId()];

        // TODO: add clipping mechanics

        Rigidbody2D rb  = playerPrefab.GetComponent<Rigidbody2D>();

        rb.MovePosition(playerData.GetPosition());
    }

    private async void OnApplicationQuit()
    {
        connected = false;
        DisableEvents();
        await websocket.Close();
    }
}