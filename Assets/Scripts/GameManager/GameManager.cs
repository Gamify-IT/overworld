using System;
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

    //Game status
    public bool isPaused = false;

    //Player 
    [SerializeField] private Transform playerTransform;

    /// <summary>
    ///     This function loads the last known position after the player logged out
    /// </summary
    public async UniTask<bool> LoadLastPlayerPosition()
    {
#if UNITY_EDITOR
        //skip loading in editor mode
        Debug.Log("Use demo values, due to Unity Editor mode");
        return false;
#endif
        string path = GameSettings.GetOverworldBackendPath() + "/courses/" + courseId + "playerstatistics";

        Optional<PlayerStatisticDTO> playerStatisticDTO = await RestRequest.GetRequest<PlayerStatisticDTO>(path);

        if (playerStatisticDTO.IsPresent())
        {
            PlayerStatisticData playerStatistic = PlayerStatisticData.ConvertDtoToData(playerStatisticDTO.Value());

            // update values with those from backend 
            DataManager.Instance.SetLogoutPositionX(playerStatistic.GetLogoutPositionX());
            DataManager.Instance.SetLogoutPositionY(playerStatistic.GetLogoutPositionY());
            DataManager.Instance.SetLogoutScene(playerStatistic.GetLogoutScene());
            DataManager.Instance.SetLogoutWorldIndex(playerStatistic.GetLogoutWorldIndex());
            DataManager.Instance.SetLogoutDungeonIndex(playerStatistic.GetLogoutDungeonIndex());

            return true;
        }

        else
        {
            Debug.Log("Player position data could not be loaded.");
            return false;
        }
    }

    /// <summary>
    ///     This function saves the last known position of the player in the backend when the player logs out 
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> SavePlayerPosition()
    {
        string path = GameSettings.GetOverworldBackendPath() + "/courses/" + courseId + "/playerstatistics" + userId;

        PlayerStatisticData playerStatistic = PlayerStatisticData.ConvertDtoToData(DataManager.Instance.GetPlayerData());

        playerStatistic.SetLogoutPositionX(playerTransform.position.x);
        playerStatistic.SetLogoutPositionY(playerTransform.position.y);
        playerStatistic.SetLogoutScene(DataManager.Instance.GetCurrentSceneName());
        playerStatistic.SetLogoutWorldIndex(DataManager.Instance.GetCurrentWorldIndex());
        playerStatistic.SetLogoutDungeonIndex(DataManager.Instance.GetCurrentDungeonIndex());

        string json = JsonUtility.ToJson(playerStatistic, true);
        bool succesful = await RestRequest.PutRequest(path, json);

        if (succesful)
        {
            Debug.Log("Updated player position for " + playerStatistic.GetLogoutPositionX() + " ," + playerStatistic.GetLogoutPositionY() + " ,"
                + playerStatistic.GetLogoutScene() + " ," + playerStatistic.GetLogoutWorldIndex() + " ," + playerStatistic.GetLogoutDungeonIndex() +
                " successfully.");
            return true;
        }
        else
        {
            Debug.Log("Could not update player position for " + playerStatistic.GetLogoutPositionX() + " ," + playerStatistic.GetLogoutPositionY() + " ,"
                + playerStatistic.GetLogoutScene() + " ," + playerStatistic.GetLogoutWorldIndex() + " ," + playerStatistic.GetLogoutDungeonIndex() +
                " successfully.");
            return false;
        }

    }

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

        string uri = overworldBackendPath + "/courses/" + courseId;

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
        bool validUserId = await ValidateUserId();
        return validUserId;
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
        string path = overworldBackendPath + "/courses/" + courseId;

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
            await RestRequest.GetRequest<PlayerStatisticDTO>(path + "/playerstatistics/");
        if (!playerStatistics.IsPresent())
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

        string playerPath = overworldBackendPath + "/players/" + userId;

        Optional<AchievementStatistic[]> achievementStatistics =
            await RestRequest.GetArrayRequest<AchievementStatistic>(playerPath + "/achievements");
        if(!achievementStatistics.IsPresent())
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
            DataManager.Instance.ProcessPlayerStatistics(playerStatistics.Value());
            DataManager.Instance.ProcessMinigameStatisitcs(minigameStatistics.Value());
            DataManager.Instance.ProcessNpcStatistics(npcStatistics.Value());
            DataManager.Instance.ProcessAchievementStatistics(achievementStatistics.Value());
            DataManager.Instance.ProcessKeybindings(keybindings.Value());
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
        Reload();
    }

    /// <summary>
    ///     This function is used by a teleporter to update the position of the player.
    /// </summary>
    public async UniTask ExecuteTeleportation()
    {
        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        LoadingManager.Instance.UnloadUnneededScenesExcept("no exceptions in this case ;)");
        LoadingManager.Instance.setup(sceneName, minigameWorldIndex, minigameDungeonIndex, minigameRespawnPosition);
        await LoadingManager.Instance.LoadScene();
    }

    /// <summary>
    ///     This function sets the data for the given area.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <param name="dungeonIndex">The index of the dungeon (0 if world)</param>
    public void SetData(int worldIndex, int dungeonIndex)
    {
        DataManager.Instance.SetCurrentWorldIndex(worldIndex);
        DataManager.Instance.SetCurrentDungeonIndex(dungeonIndex)
        //DataManager.Instance.ReadTeleporterConfig();
        if (dungeonIndex != 0)
        {
            Debug.Log("Setting data for dungeon " + worldIndex + "-" + dungeonIndex);
            DungeonData data = DataManager.Instance.GetDungeonData(worldIndex, dungeonIndex);
            ObjectManager.Instance.SetDungeonData(worldIndex, dungeonIndex, data);
            DataManager.Instance.SetCurrentSceneName("Dungeon " + worldIndex + "-" + dungeonIndex);
        }
        else
        {
            Debug.Log("Setting data for world " + worldIndex);
            WorldData data = DataManager.Instance.GetWorldData(worldIndex);
            ObjectManager.Instance.SetWorldData(worldIndex, data);
            DataManager.Instance.SetCurrentSceneName("World " + worldIndex);
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
        string path = overworldBackendPath + "/courses/" + courseId + "/teleporters";

        TeleporterUnlockedEvent teleporterData = new TeleporterUnlockedEvent(worldIndex, dungeonIndex, number, userId);
        string json = JsonUtility.ToJson(teleporterData, true);

        DataManager.Instance.ActivateTeleporter(worldIndex, dungeonIndex, number);
        bool successful = await RestRequest.PostRequest(path, json);

        if (!successful)
        {
            Debug.LogError("Teleporter unlocking could not be transfered to Backend.");
        }
    }

    /// <summary>
    ///     This function updates the progress of an achievement
    /// </summary>
    /// <param name="title">The title of the achievement</param>
    /// <param name="newProgress">The new progress of the achievement</param>
    public async void UpdateAchievement(AchievementTitle title, int newProgress)
    {
        bool unlocked = DataManager.Instance.UpdateAchievement(title, newProgress);
        if (unlocked)
        {
            AchievementData achievement = DataManager.Instance.GetAchievement(title);
            if (achievement == null)
            {
                return;
            }

            EarnAchievement(achievement);
        }
    }

    /// <summary>
    ///     This function increases an achievements progress by a given increment
    /// </summary>
    /// <param name="title">The title of the achievement</param>
    /// <param name="increment">The amount to increase the progress</param>
    /// <returns>True if the acheivement is now completed, false otherwise</returns>
    public async void IncreaseAchievementProgress(AchievementTitle title, int increment)
    {
        bool unlocked = DataManager.Instance.IncreaseAchievementProgress(title, increment);
        if (unlocked)
        {
            AchievementData achievement = DataManager.Instance.GetAchievement(title);
            if (achievement == null)
            {
                return;
            }

            EarnAchievement(achievement);
        }
    }

    /// <summary>
    ///     This function saves all achievements, which made progress in the current session
    /// </summary>
    public async UniTask<bool> SaveAchievements()
    {
        List<AchievementData> achievements = DataManager.Instance.GetAchievements();
        string basePath = overworldBackendPath + "/players/" + userId + "/achievements/";

        bool savingSuccessful = true;

        foreach (AchievementData achievementData in achievements)
        {
            if(achievementData.isUpdated())
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
        if(keyChanged)
        {
            string binding = keybinding.GetBinding().ToString();
            string key = keybinding.GetKey().ToString();
            KeybindingDTO keybindingDTO = new KeybindingDTO(userId, binding, key);

            string json = JsonUtility.ToJson(keybindingDTO, true);
            string path = overworldBackendPath + "/players/" + userId + "/keybindings/" + binding;

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
        string uri = overworldBackendPath + "/courses/" + courseId + "/playerstatistics/" + userId;

        Optional<PlayerStatisticDTO> playerStatistics = await RestRequest.GetRequest<PlayerStatisticDTO>(uri);

        if (playerStatistics.IsPresent())
        {
            return true;
        }

        string postUri = overworldBackendPath + "/courses/" + courseId + "/playerstatistics";
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
        await LoadingManager.Instance.ReloadData(sceneName, minigameWorldIndex, minigameDungeonIndex,
            minigameRespawnPosition);
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


private void PlayAchievementNotificationSound(){
    if(achievementNotificationSound!=null){
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

        DataManager.Instance.ProcessPlayerStatistics(new PlayerStatisticDTO());
        AchievementStatistic[] achivements = GetDummyAchievements();
        DataManager.Instance.ProcessAchievementStatistics(achivements);
        ResetKeybindings();
    }

    private AchievementStatistic[] GetDummyAchievements()
    {
        AchievementStatistic[] statistcs = new AchievementStatistic[2];
        string[] categories1 = { "Exploring" };
        Achievement achievement1 =
            new Achievement("GO_FOR_A_WALK", "Walk 10 tiles", categories1, "achievement2", 10);
        AchievementStatistic achievementStatistic1 = new AchievementStatistic(username, achievement1, 0, false);
        Achievement achievement2 =
            new Achievement("GO_FOR_A_LONGER_WALK", "Walk 1000 tiles", categories1, "achievement2", 1000);
        AchievementStatistic achievementStatistic2 = new AchievementStatistic(username, achievement2, 0, false);
        statistcs[0] = achievementStatistic1;
        statistcs[1] = achievementStatistic2;
        return statistcs;
    }
}