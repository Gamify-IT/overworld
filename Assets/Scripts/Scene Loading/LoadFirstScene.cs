using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class LoadFirstScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        startGame();
    }

    private async UniTask startGame()
    {
        Vector2 playerPosition = new Vector2(21.5f, 2.5f);
        int worldIndex = 1;
        int dungeonIndex = 0;

        Debug.Log("Start loading Player");

        SceneManager.LoadScene("Player");

        Debug.Log("Finish loading Player");

        Debug.Log("Start loading HUD");

        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);

        Debug.Log("Finish loading HUD");

        Debug.Log("Start loading LoadingScreen");

        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);

        Debug.Log("Finish loading LoadingScreen");
        Debug.Log("Start loading World 1");

        await LoadingManager.instance.loadScene("World 1", worldIndex, dungeonIndex, playerPosition);

        Debug.Log("Finish loading World 1");

        Debug.Log("Start unloading loading Manager");

        await SceneManager.UnloadSceneAsync("LoadingScreen");

        Debug.Log("Finish unloading loading Manager");
    }
}
