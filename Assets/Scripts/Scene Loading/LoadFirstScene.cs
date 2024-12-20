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
        Gamemode gamemode = GetGamemode();

        switch (gamemode)
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

            case Gamemode.INSPECTOR:
                Debug.Log("Starting in Inspect Mode");
                GameSettings.SetGamemode(Gamemode.INSPECTOR);
                StartGenerator();
                break;
            case Gamemode.TUTORIAL:
                Debug.Log("Starting Tutorial");
                GameSettings.SetGamemode(Gamemode.TUTORIAL);
                StartTutorial();
                break;
        }
    }

    /// <summary>
    ///     This function retrieves the current gamemode
    /// </summary>
    /// <returns>The gamemode specified in the browser variable "Gamemode", PLAY, if not present</returns>
    private Gamemode GetGamemode()
    {
        //get gamemode parameter of url
        string urlGamemodePart = Application.absoluteURL.Split("&")[^1];

        //get first part
        urlGamemodePart = urlGamemodePart.ToUpper();

        //try to parse to valid gamemode
        bool success = System.Enum.TryParse<Gamemode>(urlGamemodePart, out Gamemode gamemode);
        if (success)
        {
            Debug.Log("Specified gamemode: " + gamemode);
            return gamemode;
        }
        else
        {
            Gamemode defaultGamemode = Gamemode.PLAY;
            Debug.LogError("Incorrect gamemode specified: " + urlGamemodePart + ", setting default gamemode: " + defaultGamemode);
            return defaultGamemode;
        }
    }

    /// <summary>
    ///     This function loads the 'Player', 'Player Hud' & 'LoadingScreen' scenes.
    ///     If the loading in the 'LoadingScreen' scene is complete, the 'World 1' scene will be loaded.
    /// </summary>
    private async UniTask StartGame()
    {
        await GameSettings.FetchValues();

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

        Debug.Log("Start retrieving courseId");

        bool validCourseId = await GameManager.Instance.ValidateCourseId();

        Debug.Log("Finish retrieving courseId");

        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        LoadingManager.Instance.Setup("World 1", worldIndex, dungeonIndex, playerPosition);

        Debug.Log("Finish loading LoadingScreen");

        Debug.Log("Start validating courseId");

        if (!validCourseId)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            OfflineMode.Instance.DisplayInfo("INVALID COURSE ID");
            return;
        }

        Debug.Log("Finish validating courseId");

        Debug.Log("Start retrieving playerId");

        bool validPlayerId = await GameManager.Instance.GetUserData();

        if (!validPlayerId)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            OfflineMode.Instance.DisplayInfo("INVALID PLAYER ID");
            return;
        }

        Debug.Log("Finish retrieving playerId");

        Debug.Log("Start loading World");

        await LoadingManager.Instance.LoadData();

        Debug.Log("Finish loading World");
    }

    /// <summary>
    ///     This function starts the GeneratorWorld for the Generator or Inspector
    /// </summary>
    /// <returns></returns>
    private async UniTask StartGenerator()
    {
        await GameSettings.FetchValues();
        await SceneManager.LoadSceneAsync("AreaGeneratorManager");
    }

    /// <summary>
    ///     This function starts the tutorial mode using the offline mode and hardcoded data
    /// </summary>
    private async void StartTutorial()
    {
        Debug.Log("Start loading Tutorial");

        await GameSettings.FetchValues();

        var playerPosition = new Vector2(23f, -6f);
        var worldIndex = 0;
        var dungeonIndex = 0;

        Debug.Log("Start loading Player");

        await SceneManager.LoadSceneAsync("Player");

        Debug.Log("Finish loading Player");

        Debug.Log("Start loading HUD");

        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);

        Debug.Log("Finish loading HUD");

        Debug.Log("Start loading LoadingScreen");

        Debug.Log("Start retrieving courseId");

        bool validCourseId = await GameManager.Instance.ValidateCourseId();

        Debug.Log("Finish retrieving courseId");

        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        LoadingManager.Instance.Setup("Tutorial", worldIndex, dungeonIndex, playerPosition);

        Debug.Log("Finish loading LoadingScreen");

        Debug.Log("Start validating courseId");

        if (!validCourseId)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            OfflineMode.Instance.DisplayInfo("Welcome to the Tutorial!");
            return;
        }

        Debug.Log("Finish validating courseId");

        await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
        OfflineMode.Instance.DisplayInfo("Welcome to the Tutorial!");

        Debug.Log("Finish loading Tutorial");
    }
}