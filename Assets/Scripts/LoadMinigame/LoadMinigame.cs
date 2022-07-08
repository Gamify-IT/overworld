using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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
            LoadMinigameInIframe("moorhuhn", "config");
        }
    }
}
