using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSubScene : MonoBehaviour
{
    public string sceneToLoad;
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag == "Player")
        {
            SceneManager.LoadScene(sceneToLoad);

            SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);
        }
    }
}
