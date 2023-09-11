using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;

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
        Gamemode gamemode = GetGamemode();

        switch(gamemode)
        {
            case Gamemode.PLAY:
                Debug.Log("Starting in Play Mode");
                GameSettings.SetGamemode(Gamemode.PLAY);
                StartGame();
                break;

            case Gamemode.GENERATOR:
                Debug.Log("Starting in Generator Mode");
                GameSettings.SetGamemode(Gamemode.GENERATOR);
                StartGenerator();
                break;

            case Gamemode.INSPECT:
                Debug.Log("Starting in Inspect Mode");
                GameSettings.SetGamemode(Gamemode.INSPECT);
                StartGenerator();
                break;
        }
    }

    /// <summary>
    ///     This function retrieves the current gamemode
    /// </summary>
    /// <returns>The gamemode specified in the browser variable "Gamemode", PLAY, if not present</returns>
    private Gamemode GetGamemode()
    {
        Gamemode gamemode = Gamemode.PLAY;

        Optional<string> result = BrowserVariable.TryToReadVariable("Gamemode");
        if(result.IsPresent())
        {
            bool success = System.Enum.TryParse<Gamemode>(result.Value(), out gamemode);
            if(success)
            {
                Debug.Log("Specified gamemode: " + gamemode);
            }
            else
            {
                Debug.LogError("Incorrect gamemode specified: " + result.Value());
            }
        }
        else
        {
            Debug.LogError("No gamemode given");
        }
        return gamemode;
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

        await SceneManager.LoadSceneAsync("Player");

        Debug.Log("Finish loading Player");

        Debug.Log("Start loading HUD");

        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);

        Debug.Log("Finish loading HUD");

        Debug.Log("Start loading LoadingScreen");

        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        LoadingManager.Instance.setup("World 1", worldIndex, dungeonIndex, playerPosition);

        Debug.Log("Finish loading LoadingScreen");

        Debug.Log("Start retrieving courseId");

        bool validCourseId = await GameManager.Instance.ValidateCourseId();
        if (!validCourseId)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            return;
        }

        Debug.Log("Finish retrieving courseId");

        Debug.Log("Start retrieving playerId");
        bool validPlayerId = await GameManager.Instance.GetUserData();
        if (!validPlayerId)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            return;
        }

        Debug.Log("Finish retrieving playerId");

        Debug.Log("Start loading World 1");

        await LoadingManager.Instance.LoadData();

        Debug.Log("Finish loading World 1");
    }

    /// <summary>
    ///     This function starts the GeneratorWorld 
    /// </summary>
    /// <returns></returns>
    private async UniTask StartGenerator()
    {
        await SceneManager.LoadSceneAsync("AreaGeneratorManager");
    }
}