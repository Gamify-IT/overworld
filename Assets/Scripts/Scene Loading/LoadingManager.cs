using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class LoadingManager : MonoBehaviour
{
    #region Singleton
    public static LoadingManager instance { get; private set; }

    /// <summary>
    /// This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #endregion

    #region Attributes
    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI loadingText;

    string sceneToLoad;
    int worldIndex;
    int dungeonIndex;
    Vector2 playerPosition;
    #endregion

    #region Functionality
    public async UniTask loadData(string sceneToLoad, int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        this.sceneToLoad = sceneToLoad;
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.playerPosition = playerPosition;

        slider.value = 0;
        progressText.text = "0%";
        loadingText.text = "LOADING DATA...";

        if(GameManager.instance == null)
        {
            Debug.Log("Game Manager not online yet.");
            return;
        }

        Debug.Log("Start fetching data");

        await GameManager.instance.fetchData();

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if(GameManager.instance.loadingError)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
        }
        else
        {
            await loadScene();
        }
    }

    public async UniTask loadScene()
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

        GameManager.instance.setData(worldIndex, dungeonIndex);

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "PREPARING GAME START...";

        Debug.Log("Start unloading other scenes");

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") && !tempSceneName.Equals(sceneToLoad) && !tempSceneName.Equals("LoadingScreen") && !tempSceneName.Equals("OfflineMode"))
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

    public async UniTask reloadData(int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.playerPosition = playerPosition;

        slider.value = 0;
        progressText.text = "0%";
        loadingText.text = "LOADING DATA...";

        if (GameManager.instance == null)
        {
            Debug.Log("Game Manager not online yet.");
            return;
        }

        AreaLocationDTO[] unlockedAreasOld = GameManager.instance.getUnlockedAreas();

        Debug.Log("Start fetching data");

        await GameManager.instance.fetchData();

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if (GameManager.instance.loadingError)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            return;
        }

        slider.value = 0.5f;
        progressText.text = "50%";
        loadingText.text = "PROCESSING DATA...";

        GameManager.instance.setData(worldIndex, dungeonIndex);

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "SETTING UP PLAYER...";

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;

        slider.value = 1;
        progressText.text = "100%";
        loadingText.text = "DONE...";

        AreaLocationDTO[] unlockedAreasNew = GameManager.instance.getUnlockedAreas();
        string infoText = checkForNewUnlockedArea(unlockedAreasOld, unlockedAreasNew);

        if(infoText != "")
        {
            await SceneManager.LoadSceneAsync("InfoScreen", LoadSceneMode.Additive);
            InfoManager.instance.displayInfo(infoText);
        }

        await SceneManager.UnloadSceneAsync("LoadingScreen");
    }

    /// <summary>
    /// This function checks, if a new area has been unlocked since the last reload
    /// </summary>
    /// <param name="unlockedAreasOld">Set of unlocked areas before the reload</param>
    /// <param name="unlockedAreasNew">Set of unlocked areas after the reload</param>
    /// <returns>A info text to display, if a newly unlocked area was found, "" otherwise</returns>
    public string checkForNewUnlockedArea(AreaLocationDTO[] unlockedAreasOld, AreaLocationDTO[] unlockedAreasNew)
    {
        string infoText = "";
        foreach (AreaLocationDTO newLocation in unlockedAreasNew)
        {
            bool newArea = true;
            for(int index = 0; index < unlockedAreasOld.Length; index++)
            {
                AreaLocationDTO oldLocation = unlockedAreasOld[index];
                if (newLocation.worldIndex == oldLocation.worldIndex &&
                    newLocation.dungeonIndex == oldLocation.dungeonIndex)
                {
                    newArea = false;
                    break;
                }
            }
            if(newArea)
            {
                if(newLocation.dungeonIndex != 0)
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
