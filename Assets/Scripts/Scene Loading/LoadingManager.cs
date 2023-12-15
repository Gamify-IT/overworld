using System.Collections.Generic;
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
    ///     This function sets up what the <c>Loading Manager</c> should load.
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

        if (DataManager.Instance == null)
        {
            Debug.Log("Data Manager not online.");
            return;
        }

        Debug.Log("Start fetching data");

        bool errorLoadingPlayerData = await GameManager.Instance.FetchData();
        bool errorLoadingAreaData = await DataManager.Instance.FetchAreaData();
        Debug.Log("Area loading: " + errorLoadingAreaData);
        
        bool loadingError = errorLoadingPlayerData | errorLoadingAreaData;

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if (loadingError)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);

            //display correct error message
            string info = "AN ERROR OCCURRED WHILE LOADING THE REQUIRED DATA... \n COULD NOT LOAD:";
            if(errorLoadingPlayerData)
            {
                info += "\n - PLAYER DATA";
            }
            if(errorLoadingAreaData)
            {
                info += "\n - AREA DATA";
            }
            OfflineMode.Instance.DisplayInfo(info);
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
        AreaLocationDTO[] unlockedAreas = DataManager.Instance.GetPlayerData().unlockedAreas;
        SetupProgessBar(unlockedAreas);

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "PREPARING GAME START...";

        Debug.Log("Start unloading other scenes");

        UnloadUnneededScenesExcept(sceneToLoad);

        Debug.Log("Finish unloading other scenes");

        slider.value = 0.95f;
        progressText.text = "95%";
        loadingText.text = "SETTING UP PLAYER...";

        Debug.Log("Set player position");

        AreaData world1 = DataManager.Instance.GetAreaData(new AreaInformation(1, new Optional<int>())).Value();
        if (world1.IsGeneratedArea())
        {
            playerPosition = FindStartingPosition(world1);
        }

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
    ///     This function finds a valid starting position, if world 1 is a generated area
    /// </summary>
    /// <param name="areaData">The data of World 1</param>
    /// <returns>A valid starting position</returns>
    private Vector2 FindStartingPosition(AreaData areaData)
    {
        CellType[,] tiles = areaData.GetAreaMapData().GetLayout().GetCellTypes();

        for(int y=0; y<tiles.GetLength(1); y++)
        {
            for(int x=0; x<tiles.GetLength(0); x++)
            {
                if(tiles[x,y] == CellType.FLOOR)
                {
                    return new Vector2(x, y) + GetOffset() + new Vector2(0.5f, 0.5f);
                }
            }
        }

        return new Vector2(0, 0);
    }

    /// <summary>
    ///     This function returns the grid offset of world 1
    /// </summary>
    /// <returns>The grid offset of world 1</returns>
    private Vector2Int GetOffset()
    {
        string path = "AreaInfo/World1";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AreaInformationDTO areaInformationDTO = AreaInformationDTO.CreateFromJSON(json);
        AreaInformationData areaInformation = AreaInformationData.ConvertDtoToData(areaInformationDTO);
        return areaInformation.GetGridOffset();
    }

    /// <summary>
    ///     This function sets all data but without reloading the scene.
    /// </summary>
    /// <param name="worldIndex">index of the current world</param>
    /// <param name="dungeonIndex">index of the current dungeon - null if the area is a world</param>
    /// <param name="playerPosition">position where the player should start</param>
    public async UniTask ReloadData(string sceneToLoad, int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        this.sceneToLoad = sceneToLoad;
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

        AreaLocationDTO[] unlockedAreasOld = DataManager.Instance.GetPlayerData().unlockedAreas;

        Debug.Log("Start fetching data");

        bool loadingSuccesful = await GameManager.Instance.FetchData();

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if (loadingSuccesful)
        {
            await SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
            return;
        }

        slider.value = 0.5f;
        progressText.text = "50%";
        loadingText.text = "PROCESSING DATA...";

        GameManager.Instance.SetData(worldIndex, dungeonIndex);
        AreaLocationDTO[] unlockedAreasNew = DataManager.Instance.GetPlayerData().unlockedAreas;
        SetupProgessBar(unlockedAreasNew);
        string infoText = CheckForNewUnlockedArea(unlockedAreasOld, unlockedAreasNew);

        if (infoText != "")
        {
            await SceneManager.LoadSceneAsync("InfoScreen", LoadSceneMode.Additive);
            string headerText = "";
            InfoManager.Instance.DisplayInfo(headerText, infoText);
        }

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "SETTING UP PLAYER...";

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;

        slider.value = 1;
        progressText.text = "100%";
        loadingText.text = "DONE...";

        await SceneManager.UnloadSceneAsync("LoadingScreen");
    }

    /// <summary>
    ///     This function checks, if a new area has been unlocked since the last reload
    /// </summary>
    /// <param name="unlockedAreasOld">Set of unlocked areas before the reload</param>
    /// <param name="unlockedAreasNew">Set of unlocked areas after the reload</param>
    /// <returns>A info text to display, if a newly unlocked area was found, "" otherwise</returns>
    private string CheckForNewUnlockedArea(AreaLocationDTO[] unlockedAreasOld, AreaLocationDTO[] unlockedAreasNew)
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

    /// <summary>
    ///     This function sets up the progress with the highest unlocked area and the minigame progress in that area
    /// </summary>
    /// <param name="unlockedAreas">All unlocked areas</param>
    private void SetupProgessBar(AreaLocationDTO[] unlockedAreas)
    {
        ProgressBar.Instance.setUnlockedArea(1);
        ProgressBar.Instance.setProgress(0f);
        for (int worldIndex = GameSettings.GetMaxWorlds(); worldIndex > 0; worldIndex--)
        {
            if (isWorldUnlocked(unlockedAreas, worldIndex))
            {
                int dungeonIndex = getHighestUnlockedDungeonIndex(unlockedAreas, worldIndex);
                if (dungeonIndex == 0)
                {
                    ProgressBar.Instance.setUnlockedArea(worldIndex);
                    float progress = DataManager.Instance.GetMinigameProgress(worldIndex);
                    ProgressBar.Instance.setProgress(progress);
                }
                else
                {
                    ProgressBar.Instance.setUnlockedArea(worldIndex, dungeonIndex);
                    float progress = DataManager.Instance.GetMinigameProgress(worldIndex, dungeonIndex);
                    ProgressBar.Instance.setProgress(progress);
                }

                break;
            }
        }
    }

    /// <summary>
    ///     This function checks whether the player has unlocked a world or not
    /// </summary>
    /// <param name="unlockedAreas">All unlocked areas</param>
    /// <param name="worldIndex">The index of the world to be checked</param>
    /// <returns>True, if the world is unlocked, false otherwise</returns>
    private bool isWorldUnlocked(AreaLocationDTO[] unlockedAreas, int worldIndex)
    {
        for (int index = 0; index < unlockedAreas.Length; index++)
        {
            if (unlockedAreas[index].worldIndex == worldIndex && unlockedAreas[index].dungeonIndex == 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This function checks whether the player has unlocked a dungeon or not
    /// </summary>
    /// <param name="unlockedAreas">All unlocked areas</param>
    /// <param name="worldIndex">The index of the world the dungeon to be checked is in</param>
    /// <param name="dungeonIndex">The index of the dungeon to be checked</param>
    /// <returns>True, if the dungeon is unlocked, false otherwise</returns>
    private bool isDungeonUnlocked(AreaLocationDTO[] unlockedAreas, int worldIndex, int dungeonIndex)
    {
        for (int index = 0; index < unlockedAreas.Length; index++)
        {
            if (unlockedAreas[index].worldIndex == worldIndex && unlockedAreas[index].dungeonIndex == dungeonIndex)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This function returns the highest unlocked dungeon of a given world
    /// </summary>
    /// <param name="unlockedAreas">All unlocked areas</param>
    /// <param name="worldIndex">The index of the world</param>
    /// <returns></returns>
    private int getHighestUnlockedDungeonIndex(AreaLocationDTO[] unlockedAreas, int worldIndex)
    {
        for (int dungeonIndex = GameSettings.GetMaxDungeons(); dungeonIndex > 0; dungeonIndex--)
        {
            if (isDungeonUnlocked(unlockedAreas, worldIndex, dungeonIndex))
            {
                return dungeonIndex;
            }
        }

        return 0;
    }

    /// <summary>
    ///     This function unloades every scene that is not the given openScene or player and hud related scenes.
    /// </summary>
    /// <param name="openScene"></param>
    public async void UnloadUnneededScenesExcept(string openScene)
    {
        List<Scene> scenesToUnload = new List<Scene>();
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") &&
                !tempSceneName.Equals(openScene) && !tempSceneName.Equals("LoadingScreen") &&
                !tempSceneName.Equals("OfflineMode"))
            {
                scenesToUnload.Add(SceneManager.GetSceneAt(sceneIndex));
            }
        }

        foreach (Scene scene in scenesToUnload)
        {
            if (scene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    #endregion
}