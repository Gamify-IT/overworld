using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    #region Singleton

    public static LoadingManager Instance { get; private set; }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    ///     This function deletes the singleton instance, so it can be reinitialized.
    /// </summary>
    private void OnDestroy()
    {
        Instance = null;
    }

    #endregion

    #region Attributes

    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI loadingText;

    private string sceneToLoad;
    private int worldIndex;
    private int dungeonIndex;
    private Vector2 playerPosition;

    #endregion

    #region Functionality

    /// <summary>
    /// This function sets up what the <c>Loading Manager</c> should load.
    /// </summary>
    /// <param name="sceneToLoad">name of the scene to load</param>
    /// <param name="worldIndex">index of the current world</param>
    /// <param name="dungeonIndex">index of the current dungeon - null if the area is a world</param>
    /// <param name="playerPosition">position where the player should start</param>
    public void setup(string sceneToLoad, int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        this.sceneToLoad = sceneToLoad;
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.playerPosition = playerPosition;
    }

    /// <summary>
    ///     This function triggers the loading of data from the backend and the loading of the scene.
    ///     If a loading error occurs, the offline mode site is displayed.
    /// </summary>
    public async UniTask LoadData()
    {
        slider.value = 0;
        progressText.text = "0%";
        loadingText.text = "LOADING DATA...";

        if (GameManager.Instance == null)
        {
            Debug.Log("Game Manager not online.");
            return;
        }

        Debug.Log("Start fetching data");

        await GameManager.Instance.FetchData();

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if (GameManager.Instance.loadingError)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
        }
        else
        {
            await LoadScene();
        }
    }

    /// <summary>
    ///     This function loads the scene and unloads unneeded scenes.
    /// </summary>
    public async UniTask LoadScene()
    {
        slider.value = 0.5f;
        progressText.text = "50%";
        loadingText.text = "LOADING WORLD...";

        Debug.Log("Start loading scene");

        await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        Debug.Log("Finish loading scene");

        slider.value = 0.75f;
        progressText.text = "75%";
        loadingText.text = "PROCESSING DATA...";

        Debug.Log("Setting data");

        GameManager.Instance.SetData(worldIndex, dungeonIndex);

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "PREPARING GAME START...";

        Debug.Log("Start unloading other scenes");

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") &&
                !tempSceneName.Equals(sceneToLoad) && !tempSceneName.Equals("LoadingScreen") &&
                !tempSceneName.Equals("OfflineMode"))
            {
                await SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }

        Debug.Log("Finish unloading other scenes");

        slider.value = 0.95f;
        progressText.text = "95%";
        loadingText.text = "SETTING UP PLAYER...";

        Debug.Log("Set player position");

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;

        slider.value = 1;
        progressText.text = "100%";
        loadingText.text = "DONE...";

        Debug.Log("Loading manager done");

        Debug.Log("Start unloading loading Manager");

        await SceneManager.UnloadSceneAsync("LoadingScreen");

        Debug.Log("Finish unloading loading Manager");
    }

    /// <summary>
    ///     This function sets all data but without reloading the scene.
    /// </summary>
    /// <param name="worldIndex">index of the current world</param>
    /// <param name="dungeonIndex">index of the current dungeon - null if the area is a world</param>
    /// <param name="playerPosition">position where the player should start</param>
    public async UniTask ReloadData(int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.playerPosition = playerPosition;

        slider.value = 0;
        progressText.text = "0%";
        loadingText.text = "LOADING DATA...";

        if (GameManager.Instance == null)
        {
            Debug.Log("Game Manager not online yet.");
            return;
        }

        AreaLocationDTO[] unlockedAreasOld = GameManager.Instance.GetUnlockedAreas();

        Debug.Log("Start fetching data");

        await GameManager.Instance.FetchData();

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if (GameManager.Instance.loadingError)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            return;
        }

        slider.value = 0.5f;
        progressText.text = "50%";
        loadingText.text = "PROCESSING DATA...";

        GameManager.Instance.SetData(worldIndex, dungeonIndex);

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "SETTING UP PLAYER...";

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;

        slider.value = 1;
        progressText.text = "100%";
        loadingText.text = "DONE...";

        AreaLocationDTO[] unlockedAreasNew = GameManager.Instance.GetUnlockedAreas();
        string infoText = CheckForNewUnlockedArea(unlockedAreasOld, unlockedAreasNew);

        if (infoText != "")
        {
            await SceneManager.LoadSceneAsync("InfoScreen", LoadSceneMode.Additive);
            string headerText = "";
            InfoManager.Instance.DisplayInfo(headerText, infoText);
        }

        await SceneManager.UnloadSceneAsync("LoadingScreen");
    }

    /// <summary>
    ///     This function checks, if a new area has been unlocked since the last reload
    /// </summary>
    /// <param name="unlockedAreasOld">Set of unlocked areas before the reload</param>
    /// <param name="unlockedAreasNew">Set of unlocked areas after the reload</param>
    /// <returns>A info text to display, if a newly unlocked area was found, "" otherwise</returns>
    public string CheckForNewUnlockedArea(AreaLocationDTO[] unlockedAreasOld, AreaLocationDTO[] unlockedAreasNew)
    {
        string infoText = "";
        foreach (AreaLocationDTO newLocation in unlockedAreasNew)
        {
            bool newArea = true;
            for (int index = 0; index < unlockedAreasOld.Length; index++)
            {
                AreaLocationDTO oldLocation = unlockedAreasOld[index];
                if (newLocation.worldIndex == oldLocation.worldIndex &&
                    newLocation.dungeonIndex == oldLocation.dungeonIndex)
                {
                    newArea = false;
                    break;
                }
            }

            if (newArea)
            {
                if (newLocation.dungeonIndex != 0)
                {
                    infoText = "New Area unlocked: Dungeon " + newLocation.worldIndex + "-" + newLocation.dungeonIndex;
                }
                else
                {
                    infoText = "New Area unlocked: World " + newLocation.worldIndex;
                }

                return infoText;
            }
        }

        return infoText;
    }

    #endregion
}