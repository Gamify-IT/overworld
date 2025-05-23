using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
///     The <c>GameManager</c> retrievs all needed data from the backend, stores it in the <c>DataManager</c> and sets up
///     the objects via the <c>ObjectMananger</c> depending on those data.
/// </summary>
public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetToken(string tokenName);

    //Singleton
    public static GameManager Instance { get; private set; }

    //Game settigs
    private string overworldBackendPath;
    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxBooks;
    private int maxDungeons;
    private string courseId;
    private string userId;
    private string username;

    //Minigame reload
    private string sceneName;
    private Vector2 minigameRespawnPosition;
    private int minigameWorldIndex;
    private int minigameDungeonIndex;

    public AudioClip achievementNotificationSound;
    private AudioSource audioSource;

    //Achievements
    [SerializeField] private GameObject achievementNotificationManagerPrefab;
    private HashSet<AchievementData> unlockedAchievements = new HashSet<AchievementData>();

    //Game status
    private bool isPaused = false;
    private bool justLoaded = true;

    //Constants
    private const string coursesPath = "/courses/";
    private const string playerStatisticsPath = "/playerstatistics/";
    private const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private const string playersPath = "/players/";

    #region save player data
    /// <summary>
    ///     This function saves all important player data when the player is logging out
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> SavePlayerData()
    {
#if !UNITY_EDITOR
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            return await SavePlayerStatistic() && await SaveAchievements();
        }

        return false;
#endif
        return true;
    }

    /// <summary>
    ///     This function saves the last known position of the player in the backend when the player logs out 
    /// </summary>
    /// <returns></returns>
    private async UniTask<bool> SavePlayerStatistic()
    {
        Debug.Log("Start saving player statistic");

        string path = GameSettings.GetOverworldBackendPath() + coursesPath + courseId + playerStatisticsPath + userId;
        Debug.Log("path: " + path);

        PlayerStatisticData playerStatistic = DataManager.Instance.GetPlayerData();

        playerStatistic.SetLastActive(DateTime.Now.ToString(dateTimeFormat));
        playerStatistic.SetLogoutPositionX(GameObject.FindGameObjectWithTag("Player").transform.position.x);
        playerStatistic.SetLogoutPositionY(GameObject.FindGameObjectWithTag("Player").transform.position.y);

        PlayerStatisticDTO playerStatisticDTO = PlayerStatisticDTO.ConvertDataToDTO(playerStatistic);

        string json = JsonUtility.ToJson(playerStatisticDTO, true);

        bool succesful = await RestRequest.PutRequest(path, json);

        if (succesful)
        {
            Debug.Log("Updated player statistic successfully");
            return true;
        }
        else
        {
            Debug.Log("Could not update player statistic");
            return false;
        }

    }

    /// <summary>
    ///     This function saves all achievements, which made progress in the current session
    /// </summary>
    public async UniTask<bool> SaveAchievements()
    {
        List<AchievementData> achievements = DataManager.Instance.GetAchievements();
        string basePath = overworldBackendPath + playersPath + userId + "/achievements/";

        bool savingSuccessful = true;
        foreach (AchievementData achievementData in achievements)
        {
            if (achievementData.isUpdated())
            {
                AchievementStatistic achievementStatistic = AchievementData.ConvertToAchievmentStatistic(achievementData);

                string path = basePath + achievementData.GetTitle();
                string json = JsonUtility.ToJson(achievementStatistic, true);
                bool successful = await RestRequest.PutRequest(path, json);

                if (successful)
                {
                    Debug.Log("Updated achievement progress for " + achievementStatistic.achievement.achievementTitle + " in the overworld backend");
                }
                else
                {
                    savingSuccessful = false;
                    Debug.Log("Could not update the achievement progress for " + achievementStatistic.achievement.achievementTitle + " in the overworld backend");
                }
            }
        }

        return savingSuccessful;
    }
    #endregion

    /// <summary>
    ///     This function checks whether or not a valid courseId was passed or not.
    ///     If a valid id was passed, it gets stored.
    ///     Otherwise, the user is redirected to course selection page.
    /// </summary>
    public async UniTask<bool> ValidateCourseId()
    {

#if UNITY_EDITOR
        //skip loading in editor mode
        Debug.Log("Skip loading, due to Unity Editor mode");
        return false;
#endif
        courseId = Application.absoluteURL.Split("/")[^1];
        courseId = courseId.Split("&")[^2];
        GameSettings.SetCourseID(courseId);

        string uri = overworldBackendPath + coursesPath + courseId;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Checking courseId: " + courseId);
            Debug.Log("Path: " + uri);

            // Request and wait for the desired page.
            var request = webRequest.SendWebRequest();

            while (!request.isDone)
            {
                await UniTask.Yield();
            }

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    Debug.Log("CourseId " + courseId + " is invalid.");
                    courseId = "";
                    GameSettings.SetCourseID("");
                    return false;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    Debug.Log("CourseId " + courseId + " is valid.");
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     This function reads the userId from where it is stored and returns it.
    /// </summary>
    /// <returns>true if userId is valid, false otherwise</returns>
    public async UniTask<bool> GetUserData()
    {
        try
        {
            userId = GetToken("userId");
            username = GetToken("username");
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogError("Function not found: " + e);
            userId = "";
            username = "";
            return false;
        }

        Debug.Log("UserId from token: " + userId);
        Debug.Log("Username from token: " + username);

        if (GameSettings.GetGamemode() != Gamemode.TUTORIAL)
        {
            bool validUserId = await ValidateUserId();
            return validUserId;
        }

        return true;
    }
    
    /// <summary>
    ///     This function loads all needed data from the backend an converts the data into usable formats.
    ///     If an error accours while loading, the <c>loadingError</c> flag is set.
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> FetchData()
    {
        bool loadingError = false;

        //path to get world data from
        string path = overworldBackendPath + coursesPath + courseId;

        //get data
        Optional<WorldDTO>[] worldDTOs = new Optional<WorldDTO>[maxWorld + 1];
        for (int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
        {
            Optional<WorldDTO> dto = await RestRequest.GetRequest<WorldDTO>(path + "/worlds/" + worldIndex);
            worldDTOs[worldIndex] = dto;
            if (!dto.IsPresent())
            {
                loadingError = true;
            }
        }

        Optional<PlayerStatisticDTO> playerStatistics =
            await RestRequest.GetRequest<PlayerStatisticDTO>(path + playerStatisticsPath);
        if (!playerStatistics.IsPresent())
        {
            loadingError = true;
        }

        Optional<PlayerStatisticDTO[]> allPlayerStatistics =
           await RestRequest.GetArrayRequest<PlayerStatisticDTO>(path + "/playerstatistics/allPlayerStatistics");

        if (!allPlayerStatistics.IsPresent())
        {
            loadingError = true;
        }

        Optional<PlayerTaskStatisticDTO[]> minigameStatistics =
            await RestRequest.GetArrayRequest<PlayerTaskStatisticDTO>(path +
                                                                      "/playerstatistics/player-task-statistics");
        if (!minigameStatistics.IsPresent())
        {
            loadingError = true;
        }

        Optional<PlayerNPCStatisticDTO[]> npcStatistics =
            await RestRequest.GetArrayRequest<PlayerNPCStatisticDTO>(path + "/playerstatistics/player-npc-statistics");
        if (!npcStatistics.IsPresent())
        {
            loadingError = true;
        }

        Optional<ShopItem[]> shopItems =
            await RestRequest.GetArrayRequest<ShopItem>(overworldBackendPath + playersPath + userId + coursesPath + courseId + "/shop");
        if (!shopItems.IsPresent())
        {
            loadingError = true;
        }

        string playerPath = overworldBackendPath + playersPath + userId;

        Optional<AchievementStatistic[]> achievementStatistics =
            await RestRequest.GetArrayRequest<AchievementStatistic>(playerPath + "/achievements");

        if (!achievementStatistics.IsPresent())
        {
            loadingError = true;
        }

        Optional<KeybindingDTO[]> keybindings =
            await RestRequest.GetArrayRequest<KeybindingDTO>(playerPath + "/keybindings");

        Debug.Log("Got all data.");

        if (!loadingError)
        {
            for (int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
            {
                DataManager.Instance.SetWorldData(worldIndex, worldDTOs[worldIndex].Value());
            }

            DataManager.Instance.ReadTeleporterConfig();
            DataManager.Instance.ProcessShopItem(shopItems.Value());
            DataManager.Instance.ProcessMinigameStatisitcs(minigameStatistics.Value());
            DataManager.Instance.ProcessNpcStatistics(npcStatistics.Value());
            DataManager.Instance.ProcessAchievementStatistics(achievementStatistics.Value());
            DataManager.Instance.ProcessKeybindings(keybindings.Value());             
            DataManager.Instance.ProcessAllPlayerStatistics(allPlayerStatistics.Value());
            DataManager.Instance.ProcessPlayerStatistics(playerStatistics.Value());
        }

        Debug.Log("Everything set up");
        DataManager.Instance.SetupMinimap();

        return loadingError;
    }

    public async UniTask<bool> FetchUserData()
    {
        bool loadingError = false;

        //path to get world data from
        string path = overworldBackendPath + coursesPath + courseId;

        Optional<PlayerStatisticDTO> playerStatistics =
            await RestRequest.GetRequest<PlayerStatisticDTO>(path + playerStatisticsPath);
        if (!playerStatistics.IsPresent())
        {
            loadingError = true;
        }

        Optional<PlayerStatisticDTO[]> allPlayerStatistics =
           await RestRequest.GetArrayRequest<PlayerStatisticDTO>(path + "/playerstatistics/allPlayerStatistics");

        if (!allPlayerStatistics.IsPresent())
        {
            loadingError = true;
        }

        Debug.Log("Got all data.");

        if (!loadingError)
        {            

            DataManager.Instance.ProcessAllPlayerStatistics(allPlayerStatistics.Value());
            DataManager.Instance.ProcessPlayerStatistics(playerStatistics.Value());
        }

        Debug.Log("Everything set up");

        return loadingError;
    }

    /// <summary>
    ///     This function stores the data needed after a reload.
    /// </summary>
    /// <param name="respawnLocation">The position the player has to be in</param>
    /// <param name="worldIndex">The index of the world the minigame is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the minigame is in (0 if in world)</param>
    public void SetReloadLocation(Vector2 respawnLocation, int worldIndex, int dungeonIndex)
    {
        minigameRespawnPosition = respawnLocation;
        minigameWorldIndex = worldIndex;
        minigameDungeonIndex = dungeonIndex;
        sceneName = BuildSceneName();
        Debug.Log("Setup minigame respawn at: " + minigameRespawnPosition.x + ", " + minigameRespawnPosition.y);
    }

    private string BuildSceneName()
    {
        string sceneName;
        if (minigameDungeonIndex == 0)
        {
            sceneName = "World " + minigameWorldIndex;
        }
        else
        {
            sceneName = "Dungeon";
        }

        Debug.Log(sceneName);
        return sceneName;
    }

    /// <summary>
    ///     This function is called from JS.
    ///     This functions reloads the game after a minigame is completed.
    /// </summary>
    public void MinigameDone()
    {
        Debug.Log("Start minigame respawn at: " + minigameRespawnPosition.x + ", " + minigameRespawnPosition.y);
        Application.runInBackground = true;
        Reload();
        PlayerAnimation.Instance.EnableMovement();
    }

    /// <summary>
    ///     This function is used by a teleporter to update the position of the player.
    /// </summary>
    public async UniTask ExecuteTeleportation()
    {
        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        LoadingManager.Instance.UnloadUnneededScenesExcept("no exceptions in this case ;)");
        LoadingManager.Instance.Setup(sceneName, minigameWorldIndex, minigameDungeonIndex, minigameRespawnPosition);
        await LoadingManager.Instance.LoadScene();
    }

    /// <summary>
    ///     This function sets the data for the given area.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <param name="dungeonIndex">The index of the dungeon (0 if world)</param>
    public void SetData(int worldIndex, int dungeonIndex)
    {
        Debug.Log("SetData: " + worldIndex + "-" + dungeonIndex);
        
        if (dungeonIndex != 0)
        {
            Debug.Log("Setting data for dungeon " + worldIndex + "-" + dungeonIndex);
            DungeonData data = DataManager.Instance.GetDungeonData(worldIndex, dungeonIndex);
            ObjectManager.Instance.SetDungeonData(worldIndex, dungeonIndex, data);
        }
        else
        {
            Debug.Log("Setting data for world " + worldIndex);
            WorldData data = DataManager.Instance.GetWorldData(worldIndex);
            ObjectManager.Instance.SetWorldData(worldIndex, data);
        }
    }

    /// <summary>
    ///     This function marks an NPC as completed.
    /// </summary>
    /// <param name="worldIndex">The index of the world the NPC is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the NPC is in (0 if in world)</param>
    /// <param name="number">The index of the NPC in its area</param>
    /// <param name="uuid">The uuid of the NPC</param>
    public async void CompleteNPC(int worldIndex, int dungeonIndex, int number, string uuid)
    {
        string path = overworldBackendPath + "/internal/submit-npc-pass";

        NPCTalkEvent npcData = new NPCTalkEvent(uuid, true, userId);
        string json = JsonUtility.ToJson(npcData, true);

        bool successful = await RestRequest.PostRequest(path, json);

        if (successful)
        {
            DataManager.Instance.CompleteNPC(worldIndex, dungeonIndex, number);
        }
    }

    /// <summary>
    ///     This function activates a teleporter by saving the data in the backend as well as the data manager.
    /// </summary>
    /// <param name="worldIndex"></param>
    /// <param name="dungeonIndex"></param>
    /// <param name="number"></param>
    /// <param name="uuid"></param>
    public async void ActivateTeleporter(int worldIndex, int dungeonIndex, int number)
    {
        string path = overworldBackendPath + coursesPath + courseId + "/teleporters";

        TeleporterUnlockedEvent teleporterData = new TeleporterUnlockedEvent(worldIndex, dungeonIndex, number, userId);
        string json = JsonUtility.ToJson(teleporterData, true);

        DataManager.Instance.ActivateTeleporter(worldIndex, dungeonIndex, number);

        if (GameSettings.GetGamemode() != Gamemode.TUTORIAL)
        {
            bool successful = await RestRequest.PostRequest(path, json);

            if (!successful)
            {
                Debug.LogError("Teleporter unlocking could not be transfered to Backend.");
            }
        }    
    }

    /// <summary>
    ///     This function updates the progress of an achievement
    /// </summary>
    /// <param name="title">The title of the achievement</param>
    /// <param name="newProgress">The new progress of the achievement</param>
    /// <param name="interactedObjects">Updated list with interacted objects regarding achievement for which this method is called</param>
    /// <returns>True if the acheivement is now completed, false otherwise</returns>
    public async void UpdateAchievement(AchievementTitle title, int newProgress, List<(int, int, int)> interactedObjects)
    {
    #if !UNITY_EDITOR
        bool unlocked = DataManager.Instance.UpdateAchievement(title, newProgress, interactedObjects);
        if (unlocked)
        {
            AchievementData achievement = DataManager.Instance.GetAchievement(title);
            if (achievement == null)
            {
                return;
            }
            if (!unlockedAchievements.Any(achievementInListUnlockedAchievements => achievementInListUnlockedAchievements.GetTitle() == achievement.GetTitle()))
            {
                EarnAchievement(achievement);
                unlockedAchievements.Add(achievement);
            }

        }
    #endif
    }

    /// <summary>
    ///     This function increases an achievements progress by a given increment
    /// </summary>
    /// <param name="title">The title of the achievement</param>
    /// <param name="increment">The amount to increase the progress</param>
    /// <param name="interactedObjects">Updated list with interacted objects regarding achievement for which this method is called</param>
    /// <returns>True if the acheivement is now completed, false otherwise</returns>
    public async void IncreaseAchievementProgress(AchievementTitle title, int increment, List<(int, int, int)> interactedObjects)
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            bool unlocked = DataManager.Instance.IncreaseAchievementProgress(title, increment, interactedObjects);
            if (unlocked)
            {
                AchievementData achievement = DataManager.Instance.GetAchievement(title);
                if (achievement == null)
                {
                    return;
                }

                if (!unlockedAchievements.Any(achievementInListUnlockedAchievements => achievementInListUnlockedAchievements.GetTitle() == achievement.GetTitle()))
                {
                    EarnAchievement(achievement);
                    unlockedAchievements.Add(achievement);
                }
            }
        }
    }

    /// <summary>
    ///     This function saves all shop items, which made progress in the current session
    /// </summary>
    public async UniTask<bool> SaveShopItem()
    {
        List<ShopItemData> shopItems = DataManager.Instance.GetShopItems();
        string basePath = overworldBackendPath + playersPath + userId + coursesPath + courseId + "/shop/";

        bool savingSuccessful = true;

        foreach (ShopItemData shopItemData in shopItems.Where(item => item.isUpdated()))
        {
            ShopItem shopItem = ShopItemData.ConvertToShopItem(shopItemData);

            string path = basePath + shopItemData.GetTitle();
            string json = JsonUtility.ToJson(shopItem, true);
            bool successful = await RestRequest.PutRequest(path, json);
            if (successful)
            {
                Debug.Log("Updated shop item status for " + shopItem.shopItemID + " in the overworld backend");

            }
            else
            {
                savingSuccessful = false;
                Debug.Log("Could not update the shop item status for " + shopItem.shopItemID + " in the overworld backend");
            }
        }
        return savingSuccessful;
    }

    

    /// <summary>
    ///     This functions returns an information text about the barrier.
    /// </summary>
    /// <param name="type">The type of the barrier</param>
    /// <param name="originWorldIndex">The index of the world the barrier is in</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns></returns>
    public string GetBarrierInfoText(BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        return DataManager.Instance.GetBarrierInfoText(type, originWorldIndex, destinationAreaIndex);
    }

    /// <summary>
    ///     This function returns all keybings
    /// </summary>
    /// <returns>A List containing all keybindings</returns>
    public List<Keybinding> GetKeybindings()
    {
        return DataManager.Instance.GetKeybindings();
    }

    /// <summary>
    ///     This function changes the keybind of the given <c>Binding</c> to the given <c>KeyCode</c>
    /// </summary>
    /// <param name="keybinding">The binding to change</param>
    public async void ChangeKeybind(Keybinding keybinding)
    {
        bool keyChanged = DataManager.Instance.ChangeKeybind(keybinding);

        if(keyChanged && GameSettings.GetGamemode() != Gamemode.TUTORIAL)
        {
            string binding = keybinding.GetBinding().ToString();
            string key = keybinding.GetKey().ToString();

            if (keybinding.GetBinding() == Binding.VOLUME_LEVEL)
            {
                key = DataManager.Instance.ConvertKeyCodeToInt(keybinding.GetKey()).ToString();
            }

            KeybindingDTO keybindingDTO = new KeybindingDTO(userId, binding, key);

            string json = JsonUtility.ToJson(keybindingDTO, true);
            string path = overworldBackendPath + playersPath + userId + "/keybindings/" + binding;

            bool successful = await RestRequest.PutRequest(path, json);
            if(successful)
            {
                Debug.Log("Updated the binding (" + binding + " -> " + key + ") in the overworld backend");
            }
            else
            {
                Debug.Log("Could not update the binding (" + binding + " -> " + key + ") in the overworld backend");
            }
        }
    }

    /// <summary>
    ///     This function returns the <c>KeyCode</c> for the given <c>Binding</c>
    /// </summary>
    /// <param name="binding">The binding the <c>KeyCode</c> should be returned for</param>
    /// <returns>The <c>KeyCode</c> of the binding if present, KeyCode.NONE otherwise</returns>
    public KeyCode GetKeyCode(Binding binding)
    {
        return DataManager.Instance.GetKeyCode(binding);
    }

    /// <summary>
    ///     This function resets the keybindings to the default ones
    /// </summary>
    public void ResetKeybindings()
    {
        Keybinding moveUp = new Keybinding(Binding.MOVE_UP, KeyCode.W);
        ChangeKeybind(moveUp);

        Keybinding moveLeft = new Keybinding(Binding.MOVE_LEFT, KeyCode.A);
        ChangeKeybind(moveLeft);

        Keybinding moveDown = new Keybinding(Binding.MOVE_DOWN, KeyCode.S);
        ChangeKeybind(moveDown);

        Keybinding moveRight = new Keybinding(Binding.MOVE_RIGHT, KeyCode.D);
        ChangeKeybind(moveRight);

        Keybinding sprint = new Keybinding(Binding.SPRINT, KeyCode.LeftShift);
        ChangeKeybind(sprint);

        Keybinding interact = new Keybinding(Binding.INTERACT, KeyCode.E);
        ChangeKeybind(interact);

        Keybinding cancel = new Keybinding(Binding.CANCEL, KeyCode.Escape);
        ChangeKeybind(cancel);

        Keybinding minimapZoomIn = new Keybinding(Binding.MINIMAP_ZOOM_IN, KeyCode.P);
        ChangeKeybind(minimapZoomIn);

        Keybinding minimapZoomOut = new Keybinding(Binding.MINIMAP_ZOOM_OUT, KeyCode.O);
        ChangeKeybind(minimapZoomOut);

        Keybinding gameZoomIn = new Keybinding(Binding.GAME_ZOOM_IN, KeyCode.Alpha0);
        ChangeKeybind(gameZoomIn);

        Keybinding gameZoomOut = new Keybinding(Binding.GAME_ZOOM_OUT, KeyCode.Alpha9);
        ChangeKeybind(gameZoomOut);
    }

    /// <summary>
    ///     This function updates the volume level and applies the changes to all audio in the game
    /// </summary>
    public void UpdateVolume(int volumeLevel)
    {
        float volume = 0f;
        switch (volumeLevel)
        {
            case 0:
                volume = 0f;
                break;
            case 1:
                volume = 0.5f;
                break;
            case 2:
                volume = 1f;
                break;
            case 3:
                volume = 2f;
                break;
        }
        AudioListener.volume = volume;
    }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    ///     This function initializes the <c>GameManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    private void SetupGameManager()
    {
        //loadingError = false;

        overworldBackendPath = GameSettings.GetOverworldBackendPath();
        maxWorld = GameSettings.GetMaxWorlds();
        maxMinigames = GameSettings.GetMaxMinigames();
        maxNPCs = GameSettings.GetMaxNpcs();
        maxBooks = GameSettings.GetMaxBooks();
        maxDungeons = GameSettings.GetMaxDungeons();
    }

    /// <summary>
    ///     This function checks, if a user with the found id exists, and if not creates one.
    /// </summary>
    private async UniTask<bool> ValidateUserId()
    {
        string uri = overworldBackendPath + coursesPath + courseId + playerStatisticsPath + userId;

        Optional<PlayerStatisticDTO> playerStatistics = await RestRequest.GetRequest<PlayerStatisticDTO>(uri);

        if (playerStatistics.IsPresent())
        {
            return true;
        }

        string postUri = overworldBackendPath + coursesPath + courseId + "/playerstatistics";
        UserData userData = new UserData(userId, username);
        string json = JsonUtility.ToJson(userData, true);
        bool userCreated = await RestRequest.PostRequest(postUri, json);
        return userCreated;
    }

    /// <summary>
    ///     This function reloads the game.
    /// </summary>
    private async void Reload()
    {
        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        await LoadingManager.Instance.ReloadData(sceneName, minigameWorldIndex, minigameDungeonIndex, minigameRespawnPosition);
    }

    /// <summary>
    ///     This function creates an <c>AchievementNotificationManager</c>, if needed, and adds the given achievement to be
    ///     displayed
    /// </summary>
    /// <param name="achievement">The achievement to be displayed</param>
    private void EarnAchievement(AchievementData achievement)
    {
        if (AchievementNotificationManager.Instance == null)
        {
            Instantiate(achievementNotificationManagerPrefab, transform, false);
        }

        AchievementNotificationManager.Instance.AddAchievement(achievement);

        audioSource=GetComponent<AudioSource>();
        if(audioSource==null){
            audioSource=gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip=achievementNotificationSound;
        audioSource.playOnAwake=false;
        PlayAchievementNotificationSound();
    }

    /// <summary>
    ///     This function plays the achievement notification sound.
    /// </summary>
    private void PlayAchievementNotificationSound()
    {
        if(achievementNotificationSound != null)
        {
            audioSource.PlayOneShot(achievementNotificationSound);
        }
    }

    /// <summary>
    ///     This function sets up everything with dummy data for the offline mode
    /// </summary>
    public void GetDummyData()
    {
        DataManager.Instance.ReadTeleporterConfig();
        //worldDTO dummy data
        for (int worldIndex = 0; worldIndex < maxWorld; worldIndex++)
        {
            DataManager.Instance.SetWorldData(worldIndex, new WorldData());
        }

        PlayerStatisticDTO[] allPlayers = GetDummyDataRewards();
        DataManager.Instance.ProcessAllPlayerStatistics(allPlayers);

        PlayerStatisticDTO ownPlayer = GetOwnDummyData();
        DataManager.Instance.ProcessPlayerStatistics(ownPlayer);

        AchievementStatistic[] achivements = GetDummyAchievements();
        DataManager.Instance.ProcessAchievementStatistics(achivements);

        ShopItem[] allItems = GetDummyShopItems();
        DataManager.Instance.ProcessShopItem(allItems);

        ResetKeybindings();
    }

    private AchievementStatistic[] GetDummyAchievements()
    {
        AchievementStatistic[] statistcs = new AchievementStatistic[2];
        string[] categories1 = { "Exploring" };
        Achievement achievement1 =
            new Achievement("GO_FOR_A_WALK", "Walk 10 tiles", categories1, "achievement2", 10);
        AchievementStatistic achievementStatistic1 = new AchievementStatistic(username, achievement1, 0, false, new List<IntTuple>());
        Achievement achievement2 =
            new Achievement("GO_FOR_A_LONGER_WALK", "Walk 1000 tiles", categories1, "achievement2", 1000);
        AchievementStatistic achievementStatistic2 = new AchievementStatistic(username, achievement2, 0, false, new List<IntTuple>());
        statistcs[0] = achievementStatistic1;
        statistcs[1] = achievementStatistic2;
        return statistcs;
    }


    public PlayerStatisticDTO[] GetDummyDataRewards()
    {
        int playerCount = 30;
        PlayerStatisticDTO[] allStatistics = new PlayerStatisticDTO[32];
        System.Random random = new System.Random();
        List<string> names = new List<string> {
        "John", "Alice", "Bob", "Eve", "Charlie", "Dave", "Mallory", "Trent", "Peggy", "Victor",
        "Walter", "Grace", "Hank", "Ivy", "Justin", "Karen", "Leo", "Monica", "Nina", "Oscar",
        "Paula", "Quentin", "Rachel", "Steve", "Tom", "Uma", "Vince", "Wendy", "Xander", "Yara"
        };

        for (int i = 0; i < playerCount; i++)
        {
            string id = (i + 1).ToString();
            string userId = "Id" + id;
            string username = names[i];
            int knowledge = random.Next(0, 501);
            int rewards = random.Next(0, 501);
            bool showRewards = true;
            int worldIndex = random.Next(1, 5);
            int dungeonIndex = random.Next(1, 5);
            AreaLocationDTO currentArea = new AreaLocationDTO(worldIndex, dungeonIndex);
            AreaLocationDTO[] unlockedAreas = { currentArea };
            AreaLocationDTO[] unlockedDungeons = { currentArea };
            TeleporterDTO teleporter = new TeleporterDTO("1", currentArea, 1);
            TeleporterDTO[] unlockedTeleporters = { teleporter };

            string lastActive = DateTime.Now.AddMinutes(-random.Next(0, 1440)).ToString(dateTimeFormat);  
            float logoutPositionX = (float)random.NextDouble() * 100;  
            float logoutPositionY = (float)random.NextDouble() * 100;  
            string logoutScene = "Scene" + random.Next(1, 5);  
            int currentCharacterIndex = random.Next(0, 5);  
            int volumeLevel = random.Next(0, 101);  
            int credit = random.Next(0, 1001); 
            string pseudonym = "Pseudonym" + random.Next(1, 100);  

            PlayerStatisticDTO player = new PlayerStatisticDTO(
                id, unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, userId, username,
                lastActive, logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex, volumeLevel,
                knowledge, rewards, showRewards, credit, pseudonym, "character_default", "none"
            );
            allStatistics[i] = player;
        }

        int worldIndex1 = 2;
        int dungeonIndex1 = 1;
        AreaLocationDTO currentArea1 = new AreaLocationDTO(worldIndex1, dungeonIndex1);
        AreaLocationDTO[] unlockedAreas1 = { currentArea1 };
        AreaLocationDTO[] unlockedDungeons1 = { currentArea1 };
        TeleporterDTO teleporter1 = new TeleporterDTO("1", currentArea1, 1);
        TeleporterDTO[] unlockedTeleporters1 = { teleporter1 };
        PlayerStatisticDTO player31 = new PlayerStatisticDTO(
            "Id32", unlockedAreas1, unlockedDungeons1, unlockedTeleporters1, currentArea1, "Id32", "Marco",
            DateTime.Now.ToString(dateTimeFormat), 25.5f, 12.3f, "Scene3", 2, 50, 200, 170, true, 500, "TheoPro", "character_default", "none"
        );
        allStatistics[30] = player31;
        PlayerStatisticDTO ownPlayer = GetOwnDummyData();
        allStatistics[31] = ownPlayer;

        return allStatistics;
    }

    public PlayerStatisticDTO GetOwnDummyData()
    {
        PlayerStatisticDTO ownPlayerData;

        if (GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            int worldIndex = 0;
            int dungeonIndex = 0;
            AreaLocationDTO currentArea = new AreaLocationDTO(worldIndex, dungeonIndex);
            AreaLocationDTO[] unlockedAreas = { currentArea };
            AreaLocationDTO[] unlockedDungeons = { };
            TeleporterDTO[] unlockedTeleporters = { };

            ownPlayerData = new PlayerStatisticDTO(
                "31", unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, "Id31", "Aki",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 20.0f, 15.0f, "Tutorial", 1, 75, 200, 170, false, 100, "Beginner", "character_default", "none"
            );
        }
        else
        {
            int worldIndex = 1;
            int dungeonIndex = 0;
            AreaLocationDTO currentArea = new AreaLocationDTO(worldIndex, dungeonIndex);
            AreaLocationDTO[] unlockedAreas = { currentArea };
            AreaLocationDTO[] unlockedDungeons = { currentArea };
            TeleporterDTO teleporter = new TeleporterDTO("1", currentArea, 1);
            TeleporterDTO[] unlockedTeleporters = { teleporter };

            ownPlayerData = new PlayerStatisticDTO(
                "31", unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, "Id31", "Aki",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 20.0f, 15.0f, "World 1", 1, 75, 200, 170, false, 100, "PSEProfi", "character_default", "none"
            );
        }

        return ownPlayerData;
    }

    public ShopItem[] GetDummyShopItems()
    {
        int itemCount = 14; 
        ShopItem[] shopItems = new ShopItem[itemCount];
        System.Random random = new System.Random();

        List<string> titles = new List<string> {
        "FLAME_HAT", "HEART_GLASSES", "GLOBE_HAT", "SUIT", "SANTA_COSTUME",
        "COOL_GLASSES", "RETRO_GLASSES", "SAFETY_HELMET", "CINEMA_GLASSES",
        "TITANIUM_KNIGHT", "SPORTS", "LONGHAIR", "BLUE_SHIRT", "BLONDE"
    };

        List<int> costs = new List<int> {
        10, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1
    };

        List<string> imageNames = new List<string> {
        "hat0", "glasses2", "hat1", "character4", "character9",
        "glasses1", "glasses3", "hat2", "glasses0", "character6",
        "character8", "character7", "character5", "character3"
    };

        List<string> categories = new List<string> {
        "ACCESSORIES", "ACCESSORIES", "ACCESSORIES", "OUTFIT", "OUTFIT",
        "ACCESSORIES", "ACCESSORIES", "ACCESSORIES", "ACCESSORIES",
        "ACCESSORIES", "OUTFIT", "OUTFIT", "OUTFIT", "OUTFIT"
    };

        for (int i = 0; i < itemCount; i++)
        {
            string title = titles[i];
            int cost = costs[i];
            string imageName = imageNames[i];
            string category = categories[i];
            bool bought = false;

            ShopItem shopItem = new ShopItem(title, cost, imageName, category, bought);
            shopItems[i] = shopItem;
        }

        return shopItems;
    }

    /// <summary>
    ///     This function sets if the game has just been loaded
    /// </summary>
    /// <param name="status">has the game just been loaded</param>
    public void SetJustLoaded(bool status)
    {
        justLoaded = status;
    }

    /// <summary>
    ///     This function gets the if the game juts has loaded 
    /// </summary>
    /// <returns>whether game just has been loaded</returns>
    public bool GetJustLoaded()
    {
        return justLoaded;
    }

    /// <summary>
    ///     This function decides if the game should be paused
    /// </summary>
    /// <param name="status">current game status</param>
    public void SetIsPaused(bool status)
    {
        isPaused = status;
    }

    /// <summary>
    ///     This function gets the current game status, i.e., paused or not.
    /// </summary>
    /// <returns>whether game is paused</returns>
    public bool GetIsPaused()
    {
        return isPaused;
    }

    public string GetUserId()
    {
        return userId;
    }

    public string GetCourseId()
    {
        return courseId;
    }
}