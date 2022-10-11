using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public static class RestRequest
{
    public static async UniTask<Optional<T>> GetRequest<T>(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Request for path: " + uri);

            // Request and wait for the desired page.
            var request = webRequest.SendWebRequest();

            while (!request.isDone)
            {
                await UniTask.Yield();
            }

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    T value = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    Optional<T> optional = new Optional<T>(value);
                    return optional;
            }
            T type = default;
            Optional<T> v = new Optional<T>(type);
            v.Disable();
            return v;
        }
    }

    public static async UniTask<bool> PostNpcCompleted(string uri, string json)
    {
        Debug.Log("Post Request for path: " + uri + ", posting: " + json);

        //UnityWebRequest webRequest = UnityWebRequest.Post(uri, json);
        UnityWebRequest webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        byte[] bytes = new UTF8Encoding().GetBytes(json);
        webRequest.uploadHandler = new UploadHandlerRaw(bytes);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        using (webRequest)
        {
            // Request and wait for the desired page.
            var request = webRequest.SendWebRequest();

            while (!request.isDone)
            {
                await UniTask.Yield();
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                return false;
            }
            else
            {
                Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                return true;
            }
        }
    }
}
