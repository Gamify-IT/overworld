using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class RestRequest
{
    /// <summary>
    ///     This function sends a GET REQUEST to the provided <c>uri</c> and casts the responst to an object of type <c>T</c> and returns an <c>Optional</c> of that type.  
    /// </summary>
    /// <typeparam name="T">The type to cast the response to</typeparam>
    /// <param name="uri">The path to send the request to</param>
    /// <returns>An <c>Optional</c> containing the casted return, if an error occurs, the returned <c>Optional</c> is empty</returns>
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
                    Optional<T> result = new Optional<T>(value);
                    return result;
            }
            T type = default;
            Optional<T> v = new Optional<T>(type);
            v.Clear();
            return v;
        }
    }

    /// <summary>
    ///     This function sends a GET REQUEST to the provided <c>uri</c> and casts the responst to an array of objects type <c>T</c> and returns an <c>Optional</c> of that type. 
    /// </summary>
    /// <typeparam name="T">The type to cast the response to</typeparam>
    /// <param name="uri">The path to send the request to</param>
    /// <returns>An <c>Optional</c> containing the casted return, if an error occurs, the returned <c>Optional</c> is empty</returns>
    public static async UniTask<Optional<T[]>> GetArrayRequest<T>(string uri)
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
                    T[] returnValue = JsonHelper.GetJsonArray<T>(webRequest.downloadHandler.text);
                    Optional<T[]> result = new Optional<T[]>(returnValue);
                    return result;
            }
            T type = default;
            T[] array = { type };
            Optional<T[]> v = new Optional<T[]>(array);
            v.Clear();
            return v;
        }
    }

    public static async UniTask<Optional<List<T>>> GetListRequest<T>(string uri)
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
                    List<T> returnValue = JsonHelper.GetJsonList<T>(webRequest.downloadHandler.text);
                    Debug.Log("JSON List of courses: " + returnValue);
                    Optional<List<T>> result = new Optional<List<T>>(returnValue);
                    return result;
            }
            T type = default;
            List<T> list = new List<T> { type };
            Optional<List<T>> v = new Optional<List<T>>(list);
            v.Clear();
            return v;
        }
    }


    /// <summary>
    ///     This function sends a POST REQUEST to the provided <c>uri</c> with the <c>json</c> body. 
    /// </summary>
    /// <param name="uri">The type to cast the response to</param>
    /// <param name="json">The body to be send</param>
    /// <returns>true if post request was successful, false otherwise</returns>
    public static async UniTask<bool> PostRequest(string uri, string json)
    {
        Debug.Log("Post Request for path: " + uri + ", posting: " + json);

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


    /// <summary>
    ///     This function sends a POST REQUEST to the provided <c>uri</c> with the <c>json</c> body. 
    /// </summary>
    /// <param name="uri">The type to cast the response to</param>
    /// <param name="json">The body to be send</param>
    /// <returns>true if post request was successful, false otherwise</returns>
    public static async UniTask<Optional<T>> PostRequest<T>(string uri, string json)
    {
        Debug.Log("Post Request for path: " + uri + ", posting: " + json);

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
                    Optional<T> result = new Optional<T>(value);
                    return result;
            }
            T type = default;
            Optional<T> v = new Optional<T>(type);
            v.Clear();
            return v;
        }
    }

    /// <summary>
    ///     This function sends a PUT REQUEST to the provided <c>uri</c> with the <c>json</c> body. 
    /// </summary>
    /// <param name="uri">The type to cast the response to</param>
    /// <param name="json">The body to be send</param>
    /// <returns>true if put request was successful, false otherwise</returns>
    public static async UniTask<bool> PutRequest(string uri, string json)
    {
        Debug.Log("Put Request for path: " + uri + ", posting: " + json);

        UnityWebRequest webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPUT);
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
