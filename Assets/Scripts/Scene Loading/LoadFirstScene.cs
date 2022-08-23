using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        startGame();
    }

    private async void startGame()
    {
        Vector2 playerPosition = new Vector2(21.5f, 2.5f);
        int worldIndex = 1;
        int dungeonIndex = 0;

        SceneManager.LoadScene("Player");

        AsyncOperation asyncOperationLoad = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        while (!asyncOperationLoad.isDone) { }

        await LoadingManager.instance.loadScene("World 1", worldIndex, dungeonIndex, playerPosition);

        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);
    }
}
