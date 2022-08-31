using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class manages the loading of the game scenes.
/// </summary>
public class LoadFirstScene : MonoBehaviour
{
    /// <summary>
    ///     This function is called by Unity before the first frame updates.
    /// </summary>
    private void Start()
    {
        StartGame();
    }

    /// <summary>
    ///     This function loads the 'Player', 'Player Hud' & 'LoadingScreen' scenes.
    ///     If the loading in the 'LoadingScreen' scene is complete, the 'World 1' scene will be loaded.
    /// </summary>
    private async UniTask StartGame()
    {
        var playerPosition = new Vector2(21.5f, 2.5f);
        var worldIndex = 1;
        var dungeonIndex = 0;

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

        await LoadingManager.Instance.LoadData("World 1", worldIndex, dungeonIndex, playerPosition);

        Debug.Log("Finish loading World 1");
    }
}