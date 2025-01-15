using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System;
using NativeWebSocket;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class MultiplayerManager : MonoBehaviour
{
    private WebSocket websocket;
    private bool connected = false; 
    private AreaLocationDTO areaLocationDTO;
    [SerializeField] private GameObject prefab;
    GameObject player;
    private bool first = true;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        areaLocationDTO = new AreaLocationDTO(1,1);
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

    public async Task Initialize()
    {
        connected = true;
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // getting the message as a string
            MultiplayerDTO data =  MultiplayerDTO.CreateFromJSON(System.Text.Encoding.UTF8.GetString(bytes));
            if (first)
            {
                first = false;
                player = Instantiate(prefab, new Vector3(data.position.x, data.position.y - 1, 0), Quaternion.identity);
            }
            else
            {
                player.transform.position = new Vector3(data.position.x, data.position.y - 1, 0);
            }

            Debug.Log("message: " + System.Text.Encoding.UTF8.GetString(bytes));
        };

        // Keep sending messages at every
        InvokeRepeating(nameof(SendWebSocketMessage), 1.0f, 0.03f);

        // waiting for messages
        await websocket.Connect();
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            Debug.Log(transform.position.ToString());
            await websocket.SendText(JsonUtility.ToJson(new MultiplayerDTO(transform.position, areaLocationDTO, 1)));
        }
    }

    private async void OnApplicationQuit()
    {
        connected = false;
        await websocket.Close();
    }
}