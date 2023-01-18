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

    //Achievements
    [SerializeField] private GameObject achievementNotificationManagerPrefab;

    /// <summary>
    ///     This function checks whether or not a valid courseId was passed or not.
    ///     If a valid id was passed, it gets stored.
    ///     Otherwise, the user is redirected to course selection page.
    /// </summary>
    public async UniTask<bool> ValidateCourseId()
    {
        courseId = Application.absoluteURL.Split("/")[^1];
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

        Optional<PlayerstatisticDTO> playerStatistics =
            await RestRequest.GetRequest<PlayerstatisticDTO>(path + "/playerstatistics/");
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
        }
        else
        {
            GetDummyData();
            DataManager.Instance.ReadTeleporterConfig();
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
            sceneName = "Dungeon " + minigameWorldIndex + "-" + minigameDungeonIndex;
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
    public void ExecuteTeleportation()
    {
        TeleporterSpecificLoading();
    }

    private async void TeleporterSpecificLoading()
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
        //DataManager.Instance.ReadTeleporterConfig();
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
    ///     This functions returns an information text about the barrier.
    /// </summary>
    /// <param name="type">The type of the barrier</param>
    /// <param name="originWorldIndex">The index of the world the barrier is in</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns></returns>
    public string GetBarrierInfoText(BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        if (type == BarrierType.worldBarrier)
        {
            if (DataManager.Instance.GetWorldData(destinationAreaIndex).isActive())
            {
                for (int i = destinationAreaIndex; i > 0; i--)
                {
                    if (!DataManager.Instance.IsWorldUnlocked(destinationAreaIndex - i))
                    {
                        return "YOU HAVE TO UNLOCK WORLD " + (destinationAreaIndex - i) + " FIRST";
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (!DataManager.Instance.IsDungeonUnlocked(originWorldIndex, i))
                    {
                        return "YOU HAVE TO UNLOCK DUNGEON " + originWorldIndex + "-" + (destinationAreaIndex - i) +
                               " FIRST";
                    }
                }
                
                int activeMinigameCount = 0;

                foreach (MinigameData minigameData in DataManager.Instance.GetWorldData(originWorldIndex)
                             .GetMinigameData())
                {
                    if (minigameData.GetStatus() == MinigameStatus.active)
                    {
                        activeMinigameCount++;
                    }
                }

                float uncompletedMinigames = activeMinigameCount -
                                             activeMinigameCount * DataManager.Instance.GetMinigameProgress(originWorldIndex);
                
                return "COMPLETE " + uncompletedMinigames + " MORE MINIGAMES TO UNLOCK THIS AREA.";
            }
            else
            {
                return "NOT UNLOCKABLE IN THIS GAME VERSION";
            }
        }
        else
        {
            if (DataManager.Instance.GetWorldData(originWorldIndex).getDungeonData(destinationAreaIndex).IsActive())
            {
                for (int i = destinationAreaIndex; i > 0; i--)
                {
                    if (!DataManager.Instance.IsDungeonUnlocked(originWorldIndex, destinationAreaIndex - i))
                    {
                        return "YOU HAVE TO UNLOCK DUNGEON " + originWorldIndex + "-" + (destinationAreaIndex - i) +
                               " FIRST";
                    }
                }

                int activeMinigameCount = 0;

                foreach (MinigameData minigameData in DataManager.Instance.GetWorldData(originWorldIndex)
                             .GetMinigameData())
                {
                    if (minigameData.GetStatus() == MinigameStatus.active)
                    {
                        activeMinigameCount++;
                    }
                }

                float uncompletedMinigames = activeMinigameCount -
                        activeMinigameCount * DataManager.Instance.GetMinigameProgress(originWorldIndex);
                return "COMPLETE " + uncompletedMinigames + " MORE MINIGAMES TO UNLOCK THIS AREA.";
            }

            return "NOT UNLOCKABLE IN THIS GAME VERSION";
        }
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
    public void ChangeKeybind(Keybinding keybinding)
    {
        DataManager.Instance.ChangeKeybind(keybinding);
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
        DataManager.Instance.ResetKeybindings();
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
        maxNPCs = GameSettings.GetMaxNpCs();
        maxBooks = GameSettings.GetMaxBooks();
        maxDungeons = GameSettings.GetMaxDungeons();
    }

    /// <summary>
    ///     This function checks, if a user with the found id exists, and if not creates one.
    /// </summary>
    private async UniTask<bool> ValidateUserId()
    {
        string uri = overworldBackendPath + "/courses/" + courseId + "/playerstatistics/" + userId;

        Optional<PlayerstatisticDTO> playerStatistics = await RestRequest.GetRequest<PlayerstatisticDTO>(uri);

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
    }

    /// <summary>
    ///     This function sets up everything with dummy data for the offline mode
    /// </summary>
    private void GetDummyData()
    {
        //worldDTO dummy data
        for (int worldIndex = 0; worldIndex < maxWorld; worldIndex++)
        {
            DataManager.Instance.SetWorldData(worldIndex, new WorldDTO());
        }

        DataManager.Instance.ProcessPlayerStatistics(new PlayerstatisticDTO());
        AchievementStatistic[] achivements = GetDummyAchievements();
        Debug.Log("Game Manager, achievements: " + achivements.Length);
        DataManager.Instance.ProcessAchievementStatistics(achivements);
    }

    private AchievementStatistic[] GetDummyAchievements()
    {
        AchievementStatistic[] statistcs = new AchievementStatistic[1];
        List<string> categories1 = new() { "Blub", "Bla" };
        Achievement achievement1 =
            new Achievement("Achievement 1", "First Achievement", categories1, "achievement1", 5);
        AchievementStatistic achievementStatistic1 = new AchievementStatistic("blub", achievement1, 0, false);
        statistcs[0] = achievementStatistic1;
        return statistcs;
    }
}