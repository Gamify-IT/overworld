using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System;
using NativeWebSocket;
using System.Threading.Tasks;

public class MultiplayerManager : MonoBehaviour
{
    private WebSocket websocket;

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

    private void Update()
    {
        Debug.Log(websocket.State);
        if (websocket.State == WebSocketState.Open) SendWebSocketMessage();
    }

    public async Task InitializeAsync()
    {
        Debug.Log("start multiplayer");
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
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);

        };

        // Keep sending messages at every 0.3s
        InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 1.0f);

        // waiting for messages
        await websocket.Connect();
    }

    async void SendWebSocketMessage()
    {
        Debug.Log(websocket.State);
        if (websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            Debug.Log("sending message...");
            await websocket.SendText("hello world!");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}