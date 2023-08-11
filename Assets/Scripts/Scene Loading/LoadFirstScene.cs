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
    [DllImport("__Internal")]
    private static extern string GetToken(string tokenName);

    /// <summary>
    ///     This function is called by Unity before the first frame updates.
    /// </summary>
    private void Start()
    {
        Optional<string> mode = TryToReadMode();
        if(mode.IsPresent())
        {
            if(mode.Value().Equals("Generator"))
            {
                Debug.Log("Starting in Generator Mode");
                StartGenerator();
            }
            else
            {
                Debug.Log("Starting in Play Mode");
                StartGame();
            }
        }
        else
        {
            Debug.LogError("Generator variable not found");
            Debug.Log("Starting in Generator Mode");
            StartGenerator();
        }     
    }

    /// <summary>
    ///     This function tries to read the browser variable "mode"
    /// </summary>
    /// <returns>An optional string, containing the read value, if present</returns>
    private Optional<string> TryToReadMode()
    {
        Optional<string> result = new Optional<string>();
        try
        {
            string mode = GetToken("mode");
            result.SetValue(mode);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogError("Function not found: " + e);
        }
        return result;
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
        await SceneManager.LoadSceneAsync("GeneratorWorld");
        GeneratorManager generator = FindObjectOfType<GeneratorManager>();
        if(generator != null)
        {
            AreaInformation area = new AreaInformation(1, new Optional<int>());
            generator.Setup(area);
        }
    }
}