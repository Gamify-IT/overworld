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
    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxDungeons;
    private string courseId;
    private string userId;
    private string username;

    //GameObjects
    private GameObject[,] minigameObjects;
    private GameObject[,] worldBarrierObjects;
    private GameObject[,] dungeonBarrierObjects;
    private GameObject[,] npcObjects;

    //Data
    private WorldData[] worldData;
    private PlayerstatisticDTO playerData;

    //State
    public bool active;

    //Minigame reload
    private Vector2 minigameRespawnPosition;
    private int minigameWorldIndex;
    private int minigameDungeonIndex;

    //can be refactored
    private WorldDTO[] worldDTOs;
    private PlayerTaskStatisticDTO[] playerMinigameStatistics;
    private PlayerNPCStatisticDTO[] playerNPCStatistics;
    public bool loadingError;

    #endregion

    #region Setup

    /// <summary>
    ///     This function initializes the <c>GameManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    private void SetupGameManager()
    {
        Instance = this;

        loadingError = false;

        maxWorld = GameSettings.GetMaxWorlds();
        maxMinigames = GameSettings.GetMaxMinigames();
        maxNPCs = GameSettings.GetMaxNpCs();
        maxDungeons = GameSettings.GetMaxDungeons();

        minigameObjects = new GameObject[maxWorld + 1, maxMinigames + 1];
        worldBarrierObjects = new GameObject[maxWorld + 1, maxWorld + 1];
        dungeonBarrierObjects = new GameObject[maxWorld + 1, maxDungeons + 1];
        npcObjects = new GameObject[maxWorld + 1, maxNPCs + 1];

        worldData = new WorldData[maxWorld + 1];
        playerData = new PlayerstatisticDTO();

        active = true;

        worldDTOs = new WorldDTO[maxWorld + 1];

        for (int worldIndex = 0; worldIndex <= maxWorld; worldIndex++)
        {
            worldData[worldIndex] = new WorldData();
        }
    }

    /// <summary>
    ///     This function checks whether or not a valid courseId was passed or not.
    ///     If a valid id was passed, it gets stored.
    ///     Otherwise, the user is redirected to course selection page.
    /// </summary>
    public async UniTask<bool> GetCourseId()
    {
        courseId = Application.absoluteURL.Split("/")[^1];
        string uri = "/overworld/api/v1/courses/";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + courseId))
        {
            Debug.Log("Checking courseId: " + courseId);
            Debug.Log("Path: " + uri + courseId);

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
                    Debug.LogError(uri + courseId + ": Error: " + webRequest.error);
                    Debug.Log("CourseId " + courseId + " is invalid.");
                    courseId = "";
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + courseId + ":\nReceived: " + webRequest.downloadHandler.text);
                    Debug.Log("CourseId " + courseId + " is valid.");
                    return true;
                    break;
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
        string uri = "/overworld/api/v1/courses/" + courseId + "/playerstatistics/";
        string postUri = "/overworld/api/v1/courses/" + courseId + "/playerstatistics";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + userId))
        {
            Debug.Log("Checking userId: " + userId);
            Debug.Log("Path: " + uri + userId);

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
                    Debug.LogError(uri + userId + ": Error: " + webRequest.error);
                    Debug.Log("UserId " + userId + " does not exist yet.");
                    bool userCreated = await PostUser(postUri);
                    return userCreated;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + courseId + ":\nReceived: " + webRequest.downloadHandler.text);
                    Debug.Log("UserId " + userId + " is valid.");
                    return true;
                    break;
            }

            return false;
        }
    }

    /// <summary>
    ///     This function registers a new minigame at the <c>GameManager</c>
    /// </summary>
    /// <param name="minigame">The minigame gameObject</param>
    /// <param name="world">The index of the world the minigame is in</param>
    /// <param name="dungeon">The index of the dungeon the minigame is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the minigame in its area</param>
    public void AddMinigame(GameObject minigame, int world, int dungeon, int number)
    {
        if (minigame != null)
        {
            if (dungeon == 0)
            {
                minigameObjects[world, number] = minigame;
            }
            else
            {
                minigameObjects[0, number] = minigame;
            }
        }
    }

    /// <summary>
    ///     This function removes a minigame from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the minigame is in</param>
    /// <param name="dungeon">The index of the dungeon the minigame is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the minigame in its area</param>
    public void RemoveMinigame(int world, int dungeon, int number)
    {
        if (dungeon == 0)
        {
            minigameObjects[world, number] = null;
        }
        else
        {
            minigameObjects[0, number] = null;
        }
    }

    /// <summary>
    ///     This function registers a new barrier at the <c>GameManager</c>
    /// </summary>
    /// <param name="barrier">The barrier gameObject</param>
    /// <param name="originWorldIndex">The index of the world which exit the barrier is blocking</param>
    /// <param name="destinationAreaIndex">The index of the world which entry the barrier is blocking</param>
    public void AddBarrier(GameObject barrier, BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        if (barrier != null)
        {
            switch (type)
            {
                case BarrierType.worldBarrier:
                    worldBarrierObjects[originWorldIndex, destinationAreaIndex] = barrier;
                    break;
                case BarrierType.dungeonBarrier:
                    dungeonBarrierObjects[originWorldIndex, destinationAreaIndex] = barrier;
                    break;
            }
        }
    }

    /// <summary>
    ///     This function removes a barrier from the <c>GameManager</c>
    /// </summary>
    /// <param name="worldIndexOrigin">The index of the world which exit the barrier is blocking</param>
    /// <param name="worldIndexDestination">The index of the world which entry the barrier is blocking</param>
    public void RemoveBarrier(BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        switch (type)
        {
            case BarrierType.worldBarrier:
                worldBarrierObjects[originWorldIndex, destinationAreaIndex] = null;
                break;
            case BarrierType.dungeonBarrier:
                dungeonBarrierObjects[originWorldIndex, destinationAreaIndex] = null;
                break;
        }
    }

    /// <summary>
    ///     This function registers a new npc at the <c>GameManager</c>
    /// </summary>
    /// <param name="npc">The npc gameObject</param>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void AddNpc(GameObject npc, int world, int dungeon, int number)
    {
        if (npc != null)
        {
            if (dungeon == 0)
            {
                npcObjects[world, number] = npc;
            }
            else
            {
                npcObjects[0, number] = npc;
            }
        }
    }

    /// <summary>
    ///     This function removes a npc from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void RemoveNpc(int world, int dungeon, int number)
    {
        if (dungeon == 0)
        {
            npcObjects[world, number] = null;
            string[] emptyArray = { "" };
        }
        else
        {
            npcObjects[0, number] = null;
            string[] emptyArray = { "" };
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
    public async UniTask FetchData()
    {
        if (!active)
        {
            return;
        }

        loadingError = false;

        //path to get world data from
        string path = "/overworld/api/v1/courses/" + courseId;

        //get data
        for (int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
        {
            await GetWorldDto(path + "/worlds/", worldIndex);
        }

        await UniTask.WhenAll(
            GetPlayerMinigameStatistics(path + "/playerstatistics/" + userId + "/player-task-statistics"),
            GetPlayerStatistics(path + "/playerstatistics/" + userId),
            GetPlayerNPCStatistics(path + "/playerstatistics/" + userId + "/player-npc-statistics")
        );

        Debug.Log("Got all data.");

        //process Data
        for (int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
        {
            ProcessWorldDto(worldIndex);
        }

        ProcessPlayerTaskStatisitcs(playerMinigameStatistics);
        ProcessPlayerNpcStatistics(playerNPCStatistics);

        Debug.Log("Everything set up");
    }

    /// <summary>
    ///     The <c>Update</c> function is called once every frame.
    ///     This function provides developer shortcuts.
    /// </summary>
    public void Update()
    {
        //toggle game manager
        if (Input.GetKeyDown("b"))
        {
            active = !active;
            Debug.Log("game manager now " + active);
        }

        //print all stored objects
        if (Input.GetKeyDown("j"))
        {
            PrintInfo();
        }
    }

    /// <summary>
    ///     This function prints a detailed log about all stores objects and their status.
    /// </summary>
    private void PrintInfo()
    {
        for (int worldIndex = 0; worldIndex <= maxWorld; worldIndex++)
        {
            Debug.Log("World: " + worldIndex);
            Debug.Log("  Minigames:");
            for (int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
            {
                string status = "";
                if (minigameObjects[worldIndex, minigameIndex] != null)
                {
                    status = minigameObjects[worldIndex, minigameIndex].GetComponent<Minigame>().GetInfo();
                }
                else
                {
                    status = "none";
                }

                Debug.Log("    Minigame slot " + worldIndex + "-" + minigameIndex + " contains minigame: " + status);
            }

            Debug.Log("  World Barriers:");
            for (int barrierIndex = 1; barrierIndex <= maxWorld; barrierIndex++)
            {
                string status = "";
                if (worldBarrierObjects[worldIndex, barrierIndex] != null)
                {
                    status = worldBarrierObjects[worldIndex, barrierIndex].GetComponent<Barrier>().GetInfo();
                }
                else
                {
                    status = "none";
                }

                Debug.Log("    Barrier slot " + worldIndex + "-" + barrierIndex + " contains barrier: " + status);
            }

            Debug.Log("  Dungeon Barriers:");
            for (int barrierIndex = 1; barrierIndex <= maxDungeons; barrierIndex++)
            {
                string status = "";
                if (dungeonBarrierObjects[worldIndex, barrierIndex] != null)
                {
                    status = dungeonBarrierObjects[worldIndex, barrierIndex].GetComponent<Barrier>().GetInfo();
                }
                else
                {
                    status = "none";
                }

                Debug.Log("    Barrier slot " + worldIndex + "-" + barrierIndex + " contains barrier: " + status);
            }

            Debug.Log("  NPCs:");
            for (int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
            {
                string status = "";
                if (npcObjects[worldIndex, npcIndex] != null)
                {
                    status = npcObjects[worldIndex, npcIndex].GetComponent<NPC>().GetInfo();
                }
                else
                {
                    status = "none";
                }

                Debug.Log("    NPC slot " + worldIndex + "-" + npcIndex + " contains NPC: " + status);
            }
        }
    }

    #endregion

    #region GetRequest

    /// <summary>
    ///     This function sends a GET request to the backend to get general data and stores the results in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <param name="worldIndex">The world index to be requested at the backend</param>
    /// <returns></returns>
    private async UniTask<WorldDTO> GetWorldDto(string uri, int worldIndex)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + worldIndex))
        {
            Debug.Log("Get Request for world: " + worldIndex);
            Debug.Log("Path: " + uri + worldIndex);

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
                    Debug.LogError(uri + worldIndex + ": Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + worldIndex + ": HTTP Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + worldIndex + ":\nReceived: " + webRequest.downloadHandler.text);
                    WorldDTO worldDTO = JsonUtility.FromJson<WorldDTO>(webRequest.downloadHandler.text);
                    worldDTOs[worldIndex] = worldDTO;
                    Debug.Log("Got world data.");
                    break;
            }

            return null;
        }
    }

    /// <summary>
    ///     This function sends a GET request to the backend to get player data for minigames and stores the results in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <returns></returns>
    private async UniTask<PlayerTaskStatisticDTO[]> GetPlayerMinigameStatistics(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Player minigame statistics: ");
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
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    PlayerTaskStatisticDTO[] playerTaskStatistics =
                        JsonHelper.GetJsonArray<PlayerTaskStatisticDTO>(webRequest.downloadHandler.text);
                    playerMinigameStatistics = playerTaskStatistics;
                    Debug.Log("Got player minigame data.");
                    break;
            }

            return null;
        }
    }

    /// <summary>
    ///     This function sends a GET request to the backend to get player data for npcs and stores the results in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <returns></returns>
    private async UniTask<PlayerNPCStatisticDTO[]> GetPlayerNPCStatistics(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Player minigame statistics: ");
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
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    PlayerNPCStatisticDTO[] result =
                        JsonHelper.GetJsonArray<PlayerNPCStatisticDTO>(webRequest.downloadHandler.text);
                    playerNPCStatistics = result;
                    Debug.Log("Got player npc data.");
                    break;
            }

            return null;
        }
    }

    /// <summary>
    ///     This function sends a GET request to the backend to get gerneral player data and logs the knowledge to the console
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <returns></returns>
    private async UniTask<PlayerstatisticDTO> GetPlayerStatistics(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Player minigame statistics: ");
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
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    loadingError = true;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    PlayerstatisticDTO playerStatistic =
                        JsonUtility.FromJson<PlayerstatisticDTO>(webRequest.downloadHandler.text);
                    playerData = playerStatistic;
                    Debug.Log("Player knowledge: " + playerStatistic.knowledge);
                    break;
            }

            return null;
        }
    }

    #endregion

    #region PostRequest

    /// <summary>
    ///     This function sends a POST request to the backend to mark an NPC as completed.
    /// </summary>
    /// <param name="uri">The path to send the POST request to</param>
    /// <param name="uuid">The uuid of the NPC</param>
    /// <returns></returns>
    private async UniTask PostNpcCompleted(string uri, string uuid)
    {
        NPCTalkEvent npcData = new NPCTalkEvent(uuid, true, userId);
        string json = JsonUtility.ToJson(npcData, true);

        Debug.Log("Json test: " + json);

        //UnityWebRequest webRequest = UnityWebRequest.Post(uri, json);
        UnityWebRequest webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        byte[] bytes = new UTF8Encoding().GetBytes(json);
        webRequest.uploadHandler = new UploadHandlerRaw(bytes);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        using (webRequest)
        {
            // Request and wait for the desired page.
            var request = webRequest.SendWebRequest();

            while (!request.isDone)
            {
                await UniTask.Yield();
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log("NPC mit uuid " + uuid + " has been talked to.");
                Debug.Log("Post request response code: " + webRequest.responseCode);
                Debug.Log("Post request response text: " + webRequest.downloadHandler.text);
            }
        }
    }

    /// <summary>
    ///     This function creates a user with given id
    /// </summary>
    /// <param name="uri">The path to send the POST request to</param>
    /// <returns></returns>
    private async UniTask<bool> PostUser(string uri)
    {
        UserData userData = new UserData(userId, username);
        string json = JsonUtility.ToJson(userData, true);

        Debug.Log("Json test: " + json);

        //UnityWebRequest webRequest = UnityWebRequest.Post(uri, json);
        UnityWebRequest webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        byte[] bytes = new UTF8Encoding().GetBytes(json);
        webRequest.uploadHandler = new UploadHandlerRaw(bytes);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        using (webRequest)
        {
            // Request and wait for the desired page.
            var request = webRequest.SendWebRequest();

            while (!request.isDone)
            {
                await UniTask.Yield();
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log("User with userId " + userId + " and username " + username + " has been created.");
                Debug.Log("Post request response code: " + webRequest.responseCode);
                Debug.Log("Post request response text: " + webRequest.downloadHandler.text);
                return true;
            }

            return false;
        }
    }

    #endregion

    #region ProcessingData

    /// <summary>
    ///     This function processes the world data returned from the backend and stores the needed data in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="worldIndex">The index of the world the data refers to</param>
    /// <param name="worldDTO">The world data returned from the backend</param>
    private void ProcessWorldDto(int worldIndex)
    {
        if (worldIndex < 1 || worldIndex >= worldDTOs.Length)
        {
            return;
        }

        WorldDTO worldDTO = worldDTOs[worldIndex];
        if (worldDTO == null)
        {
            return;
        }

        string id = worldDTO.id;
        int index = worldDTO.index;
        string staticName = worldDTO.staticName;
        string topicName = worldDTO.topicName;
        bool active = worldDTO.active;

        List<MinigameTaskDTO> minigameDTOs = worldDTO.minigameTasks;
        MinigameData[] minigames = GetMinigameData(minigameDTOs);

        List<NPCDTO> npcDTOs = worldDTO.npcs;
        NPCData[] npcs = GetNpcData(npcDTOs);

        List<DungeonDTO> dungeonDTOs = worldDTO.dungeons;
        DungeonData[] dungeons = GetDungeonData(dungeonDTOs);

        worldData[worldIndex] = new WorldData(id, index, staticName, topicName, active, minigames, npcs, dungeons);
    }

    /// <summary>
    ///     This function converts and list of <c>MinigameTaskDTO</c> into a array of <c>MinigameData</c>
    /// </summary>
    /// <param name="minigameDTOs">The list of <c>minigameDTO</c> to convert</param>
    /// <returns>The converted <c>MinigameData</c> array</returns>
    private MinigameData[] GetMinigameData(List<MinigameTaskDTO> minigameDTOs)
    {
        MinigameData[] minigameData = new MinigameData[maxMinigames + 1];

        foreach (MinigameTaskDTO minigameDTO in minigameDTOs)
        {
            string game;
            string configurationId;
            MinigameStatus status = global::MinigameStatus.notConfigurated;
            int highscore = 0;

            game = minigameDTO.game;
            configurationId = minigameDTO.configurationId;
            if (configurationId != "" && configurationId != null && configurationId != "NONE")
            {
                status = global::MinigameStatus.active;
            }

            MinigameData minigame = new MinigameData(game, configurationId, status, highscore);
            minigameData[minigameDTO.index] = minigame;
        }

        return minigameData;
    }

    /// <summary>
    ///     This function converts and list of <c>NPCDTO</c> into a array of <c>NPCData</c>
    /// </summary>
    /// <param name="npcDTOs">The list of <c>NPCDTO</c> to convert</param>
    /// <returns>The converted <c>NPCData</c> array</returns>
    private NPCData[] GetNpcData(List<NPCDTO> npcDTOs)
    {
        NPCData[] npcData = new NPCData[maxNPCs + 1];

        foreach (NPCDTO npcDTO in npcDTOs)
        {
            string uuid;
            string[] dialogue;
            bool hasBeenTalkedTo = false;

            uuid = npcDTO.id;
            dialogue = npcDTO.text.ToArray();
            if (dialogue.Length == 0)
            {
                string[] dummyText = { "I could tell you something useful...", "...But i don't remember." };
                dialogue = dummyText;
            }

            NPCData npc = new NPCData(uuid, dialogue, hasBeenTalkedTo);
            npcData[npcDTO.index] = npc;
        }

        return npcData;
    }

    /// <summary>
    ///     This function converts and list of <c>DungeonDTO</c> into a array of <c>DungeonData</c>
    /// </summary>
    /// <param name="dungeonDTOs">The list of <c>DungeonDTO</c> to convert</param>
    /// <returns>The converted <c>DungeonData</c> array</returns>
    private DungeonData[] GetDungeonData(List<DungeonDTO> dungeonDTOs)
    {
        DungeonData[] dungeonData = new DungeonData[maxDungeons + 1];

        foreach (DungeonDTO dungeonDTO in dungeonDTOs)
        {
            string id = dungeonDTO.id;
            int index = dungeonDTO.index;
            string staticName = dungeonDTO.staticName;
            string topicName = dungeonDTO.topicName;
            bool active = dungeonDTO.active;

            List<MinigameTaskDTO> minigameDTOs = dungeonDTO.minigameTasks;
            MinigameData[] minigames = GetMinigameData(minigameDTOs);

            List<NPCDTO> npcDTOs = dungeonDTO.npcs;
            NPCData[] npcs = GetNpcData(npcDTOs);

            DungeonData dungeon = new DungeonData(id, index, staticName, topicName, active, minigames, npcs);
            dungeonData[index] = dungeon;
        }

        return dungeonData;
    }

    /// <summary>
    ///     This function processes the player minigame statistics data returned form backend and stores the needed data in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="playerTaskStatistics">The player minigame statistics data returned from the backend</param>
    private void ProcessPlayerTaskStatisitcs(PlayerTaskStatisticDTO[] playerTaskStatistics)
    {
        if (playerTaskStatistics == null)
        {
            return;
        }

        foreach (PlayerTaskStatisticDTO statistic in playerTaskStatistics)
        {
            int worldIndex = statistic.minigameTask.area.worldIndex;

            if (worldIndex < 0 || worldIndex >= worldData.Length)
            {
                break;
            }

            int dungeonIndex = statistic.minigameTask.area.dungeonIndex;
            int index = statistic.minigameTask.index;
            int highscore = statistic.highscore;
            bool completed = statistic.completed;
            MinigameStatus status = global::MinigameStatus.notConfigurated;
            if (MinigameStatus(worldIndex, dungeonIndex, index) != global::MinigameStatus.notConfigurated)
            {
                if (completed)
                {
                    status = global::MinigameStatus.done;
                }
                else
                {
                    status = global::MinigameStatus.active;
                }
            }

            worldData[worldIndex].setMinigameStatus(dungeonIndex, index, status);
            worldData[worldIndex].setMinigameHighscore(dungeonIndex, index, highscore);
        }
    }

    /// <summary>
    ///     This function returns the status of a minigame in a world or dungeon.
    /// </summary>
    /// <param name="worldIndex">The index of the world the minigame is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the minigame is in (0 if in world)</param>
    /// <param name="index">The index of the minigame in its area</param>
    /// <returns>The status of the minigame, <c>notConfigurated if not found</c></returns>
    private MinigameStatus MinigameStatus(int worldIndex, int dungeonIndex, int index)
    {
        if (worldIndex < 0 || worldIndex >= worldData.Length)
        {
            return global::MinigameStatus.notConfigurated;
        }

        if (dungeonIndex != 0)
        {
            return worldData[worldIndex].getMinigameStatus(dungeonIndex, index);
        }

        return worldData[worldIndex].getMinigameStatus(index);
    }

    /// <summary>
    ///     This function processes the player npc statistcs data returned from the backend and stores the needed data in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="playerNPCStatistics">The player npc statistics data returned from the backend</param>
    private void ProcessPlayerNpcStatistics(PlayerNPCStatisticDTO[] playerNPCStatistics)
    {
        if (playerNPCStatistics == null)
        {
            return;
        }

        foreach (PlayerNPCStatisticDTO statistic in playerNPCStatistics)
        {
            int worldIndex = statistic.npc.area.worldIndex;
            int dungeonIndex = statistic.npc.area.dungeonIndex;
            int index = statistic.npc.index;
            bool completed = statistic.completed;

            if (worldIndex < worldData.Length)
            {
                worldData[worldIndex].setNPCStatus(dungeonIndex, index, completed);
            }
        }
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
            SetDungeonData(worldIndex, dungeonIndex);
        }
        else
        {
            Debug.Log("Setting data for world " + worldIndex);
            SetWorldData(worldIndex);
        }
    }

    /// <summary>
    ///     This functions sets the data for a given world.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    private void SetWorldData(int worldIndex)
    {
        if (worldIndex < 1 || worldIndex >= worldData.Length)
        {
            return;
        }

        WorldData data = worldData[worldIndex];

        for (int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData minigameData = data.getMinigameData(minigameIndex);
            if (minigameData == null)
            {
                minigameData = new MinigameData();
            }

            GameObject minigameObject = minigameObjects[worldIndex, minigameIndex];
            if (minigameObject == null)
            {
                continue;
            }

            Minigame minigame = minigameObject.GetComponent<Minigame>();
            if (minigame == null)
            {
                continue;
            }

            minigame.Setup(minigameData);
        }

        for (int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            NPCData npcData = data.getNPCData(npcIndex);
            if (npcData == null)
            {
                npcData = new NPCData();
            }

            GameObject npcObject = npcObjects[worldIndex, npcIndex];
            if (npcObject == null)
            {
                continue;
            }

            NPC npc = npcObject.GetComponent<NPC>();
            if (npc == null)
            {
                continue;
            }

            npc.Setup(npcData);
        }

        for (int barrierDestinationIndex = 1; barrierDestinationIndex <= maxWorld; barrierDestinationIndex++)
        {
            GameObject barrierObject = worldBarrierObjects[worldIndex, barrierDestinationIndex];
            if (barrierObject == null)
            {
                continue;
            }

            Barrier barrier = barrierObject.GetComponent<Barrier>();
            if (barrier == null)
            {
                continue;
            }

            bool activedByLecturer = worldData[barrierDestinationIndex].isActive();
            bool unlockedByPlayer = PlayerHasWorldUnlocked(barrierDestinationIndex);
            bool worldExplorable = activedByLecturer & unlockedByPlayer;
            BarrierData barrierData = new BarrierData(!worldExplorable);
            barrier.Setup(barrierData);
        }

        for (int barrierDestinationIndex = 1; barrierDestinationIndex <= maxDungeons; barrierDestinationIndex++)
        {
            GameObject barrierObject = dungeonBarrierObjects[worldIndex, barrierDestinationIndex];
            if (barrierObject == null)
            {
                continue;
            }

            Barrier barrier = barrierObject.GetComponent<Barrier>();
            if (barrier == null)
            {
                continue;
            }

            bool activedByLecturer = worldData[worldIndex].dungeonIsActive(barrierDestinationIndex);
            bool unlockedByPlayer = PlayerHasDungeonUnlocked(worldIndex, barrierDestinationIndex);
            bool dungeonExplorable = activedByLecturer & unlockedByPlayer;
            BarrierData barrierData = new BarrierData(!dungeonExplorable);
            barrier.Setup(barrierData);
        }
    }

    /// <summary>
    ///     This function checks if a player has unlocked a world.
    /// </summary>
    /// <param name="worldIndex">The index of the world to check</param>
    /// <returns>True, if the player has unlocked the world, false otherwise</returns>
    private bool PlayerHasWorldUnlocked(int worldIndex)
    {
        for (int i = 0; i < playerData.unlockedAreas.Length; i++)
        {
            if (playerData.unlockedAreas[i].worldIndex == worldIndex &&
                playerData.unlockedAreas[i].dungeonIndex == 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This function checks if a player has unlocked a dungeon.
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon to check</param>
    /// <returns>True, if the player has unlocked the dungeon, false otherwise</returns>
    private bool PlayerHasDungeonUnlocked(int worldIndex, int dungeonIndex)
    {
        for (int i = 0; i < playerData.unlockedAreas.Length; i++)
        {
            if (playerData.unlockedAreas[i].worldIndex == worldIndex &&
                playerData.unlockedAreas[i].dungeonIndex == dungeonIndex)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This functions sets the data for a given dungeon.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    private void SetDungeonData(int worldIndex, int dungeonIndex)
    {
        if (worldIndex < 1 || worldIndex >= worldData.Length)
        {
            return;
        }

        DungeonData data = worldData[worldIndex].getDungeonData(dungeonIndex);
        if (data == null)
        {
            return;
        }

        for (int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData minigameData = data.GetMinigameData(minigameIndex);
            if (minigameData == null)
            {
                minigameData = new MinigameData();
            }

            GameObject minigameObject = minigameObjects[0, minigameIndex];
            if (minigameObject == null)
            {
                continue;
            }

            Minigame minigame = minigameObject.GetComponent<Minigame>();
            if (minigame == null)
            {
                continue;
            }

            minigame.Setup(minigameData);
        }

        for (int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            NPCData npcData = data.GetNpcData(npcIndex);
            if (npcData == null)
            {
                npcData = new NPCData();
            }

            GameObject npcObject = npcObjects[0, npcIndex];
            if (npcObject == null)
            {
                continue;
            }

            NPC npc = npcObject.GetComponent<NPC>();
            if (npc == null)
            {
                continue;
            }

            npc.Setup(npcData);
        }
    }

    /// <summary>
    ///     This function marks an NPC as completed.
    /// </summary>
    /// <param name="worldIndex">The index of the world the NPC is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the NPC is in (0 if in world)</param>
    /// <param name="number">The index of the NPC in its area</param>
    /// <param name="uuid">The uuid of the NPC</param>
    public async void MarkNpCasRead(int worldIndex, int dungeonIndex, int number, string uuid)
    {
        if (active)
        {
            string path = "/overworld/api/v1/internal/submit-npc-pass";
            await PostNpcCompleted(path, uuid);

            if (worldIndex <= maxWorld)
            {
                if (dungeonIndex != 0)
                {
                    worldData[worldIndex].npcCompleted(dungeonIndex, number);
                }
                else
                {
                    worldData[worldIndex].npcCompleted(number);
                }
            }
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

    #region GetterAndSetter

    /// <summary>
    ///     This function provides an array of all areas, that the player has unlocked.
    /// </summary>
    /// <returns>A list of unlocked areas</returns>
    public AreaLocationDTO[] GetUnlockedAreas()
    {
        return playerData.unlockedAreas;
    }

    #endregion
}
