using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
///     The <c>GameManager</c> retrievs all needed data from the backend and sets up the objects depending on those data.
/// </summary>
public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetToken(string tokenName);

    #region Singleton

    public static GameManager Instance { get; private set; }

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

    #endregion

    #region Attributes

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
    private Vector2 minigameRespawnPosition;
    private int minigameWorldIndex;
    private int minigameDungeonIndex;

    //public bool loadingError;

    #endregion

    #region Setup

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
    ///     This function checks, if a user with the found id exists, and if not creates one.
    /// </summary>
    private async UniTask<bool> ValidateUserId()
    {
        string uri = overworldBackendPath + "/course/" + courseId + "/playerstatistics/" + userId;

        Optional<PlayerstatisticDTO> playerStatistics = await RestRequest.GetRequest<PlayerstatisticDTO>(uri);

        if(playerStatistics.Enabled())
        {
            return true;
        }
        else
        {
            string postUri = overworldBackendPath + "/course/" + courseId + "/playerstatistics";
            UserData userData = new UserData(userId, username);
            string json = JsonUtility.ToJson(userData, true);
            bool userCreated = await RestRequest.PostRequest(postUri, json);
            return userCreated;
        }
    }

    #endregion

    #region Loading

    /// <summary>
    ///     This function stores the data needed after a reload.
    /// </summary>
    /// <param name="respawnLocation">The position the player has to be in</param>
    /// <param name="worldIndex">The index of the world the minigame is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the minigame is in (0 if in world)</param>
    public void SetMinigameRespawn(Vector2 respawnLocation, int worldIndex, int dungeonIndex)
    {
        minigameRespawnPosition = respawnLocation;
        minigameWorldIndex = worldIndex;
        minigameDungeonIndex = dungeonIndex;
        Debug.Log("Setup minigame respawn at: " + minigameRespawnPosition.x + ", " + minigameRespawnPosition.y);
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
    ///     This function reloads the game.
    /// </summary>
    private async void Reload()
    {
        await SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        await LoadingManager.Instance.ReloadData(minigameWorldIndex, minigameDungeonIndex, minigameRespawnPosition);
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
            if (!dto.Enabled())
            {
                loadingError = true;
            }
        }

        Optional<PlayerstatisticDTO> playerStatistics = await RestRequest.GetRequest<PlayerstatisticDTO>(path + "/playerstatistics/");
        if (!playerStatistics.Enabled())
        {
            loadingError = true;
        }

        Optional<PlayerTaskStatisticDTO[]> minigameStatistics = await RestRequest.GetArrayRequest<PlayerTaskStatisticDTO>(path + "/playerstatistics/player-task-statistics");
        if(!minigameStatistics.Enabled())
        {
            loadingError = true;
        }

        Optional<PlayerNPCStatisticDTO[]> npcStatistics = await RestRequest.GetArrayRequest<PlayerNPCStatisticDTO>(path + "/playerstatistics/player-npc-statistics");
        if(!npcStatistics.Enabled())
        {
            loadingError = true;
        }

        Debug.Log("Got all data.");

        if(!loadingError)
        {
            for(int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
            {
                DataManager.Instance.SetData(worldIndex, worldDTOs[worldIndex].Value());
            }
            DataManager.Instance.ProcessPlayerStatistics(playerStatistics.Value());
            DataManager.Instance.ProcessMinigameStatisitcs(minigameStatistics.Value());
            DataManager.Instance.ProcessNpcStatistics(npcStatistics.Value());
        }
        else
        {
            GetDummyData();
        }
        Debug.Log("Everything set up");

        return loadingError;
    }

    /// <summary>
    ///     This function sets up everything with dummy data for the offline mode
    /// </summary>
    private void GetDummyData()
    {
        //worldDTO dummy data
        for(int worldIndex = 0; worldIndex<maxWorld; worldIndex++)
        {
            DataManager.Instance.SetData(worldIndex, new WorldDTO());
        }
        DataManager.Instance.ProcessPlayerStatistics(new PlayerstatisticDTO());
    }

    #endregion

    #region SettingData

    /// <summary>
    ///     This function sets the data for the given area.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <param name="dungeonIndex">The index of the dungeon (0 if world)</param>
    public void SetData(int worldIndex, int dungeonIndex)
    {
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

        if(successful)
        {
            DataManager.Instance.CompleteNPC(worldIndex, dungeonIndex, number);
        }
    }

    #endregion

    #region InfoScreen

    /// <summary>
    ///     This functions returns an information text about the barrier.
    /// </summary>
    /// <param name="type">The type of the barrier</param>
    /// <param name="originWorldIndex">The index of the world the barrier is in</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns></returns>
    public string GetBarrierInfoText(BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        string info = "NOT UNLOCKED YET";
        return info;
    }

    #endregion

}
