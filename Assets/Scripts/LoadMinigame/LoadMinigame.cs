using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class LoadMinigame : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string LoadMinigameInIframe(string minigameName, string minigameConfiguration);

    public bool isOnPosition = false;
    public bool isGameOpen = false;

    private void FixedUpdate()
    {
        isOnPosition = GameObject.FindGameObjectWithTag("Player").transform.position.x > 2 && GameObject.FindGameObjectWithTag("Player").transform.position.x < 3 && GameObject.FindGameObjectWithTag("Player").transform.position.y < 13 && GameObject.FindGameObjectWithTag("Player").transform.position.y > 12;
        if (isOnPosition && !isGameOpen)
        {
            isGameOpen = true;
            ExecuteGetRequest("w1g1");
        }
        if (!isOnPosition)
        {
            isGameOpen = false;
        }
    }

    public void ExecuteGetRequest(String staticWorldId)
    {
        StartCoroutine(GetRequest("/api/overworld/get-configurationString-by-staticWorldId/"+ staticWorldId));
    }

    private IEnumerator GetRequest(String uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

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
                    Configuration config = JsonUtility.FromJson<Configuration>(webRequest.downloadHandler.text);
                    LoadMinigameInIframe(config.minigameType, config.configurationString);
                    break;
            }
        }
    }
}
