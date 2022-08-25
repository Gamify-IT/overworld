using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class GameManagerV2 : MonoBehaviour
{
    #region Singleton
    public static GameManagerV2 instance { get; private set; }

    /// <summary>
    /// This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            setupGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Attributes
    //werden ausgelagert
    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxDungeons;
    private int courseId = 1;
    private int playerId = 1;

    //GameObjects
    private GameObject[,] minigameObjects;
    private GameObject[,] barrierObjects;
    private GameObject[,] npcObjects;

    //Data
    private WorldData[] worldData;
    private PlayerstatisticDTO playerData;

    //State
    public bool active;

    //kann eigentlich weg, wenn wir den return value vom uni task haben
    private WorldDTO[] worldDTOs;
    private PlayerTaskStatisticDTO[] playerMinigameStatistics;
    private PlayerNPCStatisticDTO[] playerNPCStatistics;

    //kann eigentlich weg, nur für manuell laden
    int currentWorld;
    int currentDungeon;
    #endregion

    #region Setup
    /// <summary>
    /// This function initializes the <c>GameManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    private void setupGameManager()
    {
        instance = this;

        maxWorld = GameSettings.getMaxWorlds();
        maxMinigames = GameSettings.getMaxMinigames();
        maxNPCs = GameSettings.getMaxNPCs();
        maxDungeons = GameSettings.getMaxDungeons();

        minigameObjects = new GameObject[maxWorld + 1, maxMinigames + 1];
        barrierObjects = new GameObject[maxWorld + 1, maxWorld + 1];
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
    /// This function registers a new minigame at the <c>GameManager</c>
    /// </summary>
    /// <param name="minigame">The minigame gameObject</param>
    /// <param name="world">The index of the world the minigame is in</param>
    /// <param name="dungeon">The index of the dungeon the minigame is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the minigame in its area</param>
    public void addMinigame(GameObject minigame, int world, int dungeon, int number)
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
    /// This function removes a minigame from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the minigame is in</param>
    /// <param name="dungeon">The index of the dungeon the minigame is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the minigame in its area</param>
    public void removeMinigame(int world, int dungeon, int number)
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
    /// This function registers a new barrier at the <c>GameManager</c>
    /// </summary>
    /// <param name="barrier">The barrier gameObject</param>
    /// <param name="worldIndexOrigin">The index of the world which exit the barrier is blocking</param>
    /// <param name="worldIndexDestination">The index of the world which entry the barrier is blocking</param>
    public void addBarrier(GameObject barrier, int worldIndexOrigin, int worldIndexDestination)
    {
        if (barrier != null)
        {
            barrierObjects[worldIndexOrigin, worldIndexDestination] = barrier;
        }
    }

    /// <summary>
    /// This function removes a barrier from the <c>GameManager</c>
    /// </summary>
    /// <param name="worldIndexOrigin">The index of the world which exit the barrier is blocking</param>
    /// <param name="worldIndexDestination">The index of the world which entry the barrier is blocking</param>
    public void removeBarrier(int worldIndexOrigin, int worldIndexDestination)
    {
        barrierObjects[worldIndexOrigin, worldIndexDestination] = null;
    }

    /// <summary>
    /// This function registers a new npc at the <c>GameManager</c>
    /// </summary>
    /// <param name="npc">The npc gameObject</param>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void addNPC(GameObject npc, int world, int dungeon, int number)
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
    /// This function removes a npc from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void removeNPC(int world, int dungeon, int number)
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
    //läd alle daten vom backend und verarbeitet und setzt in worldData
    //wird von gameStart oder minigameEnde aufgerufen
    public async UniTask fetchData()
    {
        if(!active)
        {
            return;
        }

        //path to get world data from
        string path = "/overworld/api/v1/courses/" + courseId;

        //get data
        for(int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
        {
            await GetWorldDTO(path + "/worlds/", worldIndex);
        }

        await UniTask.WhenAll(
            GetPlayerMinigameStatistics(path + "/playerstatistics/" + playerId + "/player-task-statistics"),
            GetPlayerStatistics(path + "/playerstatistics/" + playerId),
            GetPlayerNPCStatistics(path + "/playerstatistics/" + playerId + "/player-npc-statistics")
        );

        Debug.Log("Got all data.");

        //process Data
        for (int worldIndex = 1; worldIndex <= maxWorld; worldIndex++)
        {
            processWorldDTO(worldIndex);
        }

        processPlayerTaskStatisitcs(playerMinigameStatistics);
        processPlayerNPCStatistics(playerNPCStatistics);

        Debug.Log("Everything set up");
    }

    //short cuts, kann irgendwann eigentlich weg
    public void Update()
    {
        //toggle game manager
        if (Input.GetKeyDown("b"))
        {
            active = !active;
            Debug.Log("game manager now " + active);
        }

        //manuell load
        if (Input.GetKeyDown("h"))
        {
            Debug.Log("world: " + currentWorld + ", dungeon: " + currentDungeon);
            manualLoad();
        }

        //print all stored objects
        if (Input.GetKeyDown("j"))
        {
            printInfo();
        }
    }

    //load data and set current area
    private async void manualLoad()
    {
        Debug.Log("Loading Data");
        await fetchData();
        Debug.Log("Finished Loading");
        currentWorld = 1;
        currentDungeon = 0;
        setData(currentWorld, currentDungeon);
        Debug.Log("Finished relaod");
    }

    //print stored objects info
    private void printInfo()
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
                    status = minigameObjects[worldIndex, minigameIndex].GetComponent<Minigame>().getInfo();
                }
                else
                {
                    status = "none";
                }
                Debug.Log("    Minigame slot " + worldIndex + "-" + minigameIndex + " contains minigame: " + status);
            }

            Debug.Log("  Barriers:");
            for (int barrierIndex = 1; barrierIndex <= maxWorld; barrierIndex++)
            {
                string status = "";
                if (barrierObjects[worldIndex, barrierIndex] != null)
                {
                    status = barrierObjects[worldIndex, barrierIndex].GetComponent<Barrier>().getInfo();
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
                    status = npcObjects[worldIndex, npcIndex].GetComponent<NPC>().getInfo();
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
    /// This function sends a GET request to the backend to get general data and stores the results in the <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <param name="worldIndex">The world index to be requested at the backend</param>
    /// <returns></returns>
    private async UniTask<WorldDTO> GetWorldDTO(String uri, int worldIndex)
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
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + worldIndex + ": HTTP Error: " + webRequest.error);
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
    /// This function sends a GET request to the backend to get player data for minigames and stores the results in the <c>GameManager</c>
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
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    PlayerTaskStatisticDTO[] playerTaskStatistics = JsonHelper.getJsonArray<PlayerTaskStatisticDTO>(webRequest.downloadHandler.text);
                    playerMinigameStatistics = playerTaskStatistics;
                    Debug.Log("Got player minigame data.");
                    break;
            }
            return null;
        }
    }

    /// <summary>
    /// This function sends a GET request to the backend to get player data for npcs and stores the results in the <c>GameManager</c>
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
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    PlayerNPCStatisticDTO[] result = JsonHelper.getJsonArray<PlayerNPCStatisticDTO>(webRequest.downloadHandler.text);
                    playerNPCStatistics = result;
                    Debug.Log("Got player npc data.");
                    break;
            }
            return null;
        }
    }

    /// <summary>
    /// This function sends a GET request to the backend to get gerneral player data and logs the knowledge to the console
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
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                    PlayerstatisticDTO playerStatistic = JsonUtility.FromJson<PlayerstatisticDTO>(webRequest.downloadHandler.text);
                    playerData = playerStatistic;
                    Debug.Log("Player knowledge: " + playerStatistic.knowledge);
                    break;
            }
            return null;
        }
    }
    #endregion

    #region PostRequest
    private async UniTask postNPCCompleted(string uri, string uuid)
    {
        NPCTalkEvent npcData = new NPCTalkEvent(uuid, true, playerId.ToString());
        string json = JsonUtility.ToJson(npcData, true);

        Debug.Log("Json test: " + json);

        //UnityWebRequest webRequest = UnityWebRequest.Post(uri, json);
        UnityWebRequest webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        byte[] bytes = new System.Text.UTF8Encoding().GetBytes(json);
        webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bytes);
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
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
    #endregion

    #region ProcessingData
    /// <summary>
    /// This function processes the world data returned from the backend and stores the needed data in the <c>GameManager</c>
    /// </summary>
    /// <param name="worldIndex">The index of the world the data refers to</param>
    /// <param name="worldDTO">The world data returned from the backend</param>
    private void processWorldDTO(int worldIndex)
    {
        if(worldIndex < 1 || worldIndex >= worldDTOs.Length)
        {
            return;
        }

        WorldDTO worldDTO = worldDTOs[worldIndex];
        if(worldDTO == null)
        {
            return;
        }

        string id = worldDTO.id;
        int index = worldDTO.index;
        string staticName = worldDTO.staticName;
        string topicName = worldDTO.topicName;
        bool active = worldDTO.active;

        List<MinigameTaskDTO> minigameDTOs = worldDTO.getMinigameTasks();
        MinigameData[] minigames = getMinigameData(minigameDTOs);

        List<NPCDTO> npcDTOs = worldDTO.getNPCs();
        NPCData[] npcs = getNPCData(npcDTOs);

        List<DungeonDTO> dungeonDTOs = worldDTO.getDungeons();
        DungeonData[] dungeons = getDungeonData(dungeonDTOs);

        worldData[worldIndex] = new WorldData(id, index, staticName, topicName, active, minigames, npcs, dungeons);
    }

    //converts MinigameTaskDTOs in minigameData
    private MinigameData[] getMinigameData(List<MinigameTaskDTO> minigameDTOs)
    {
        MinigameData[] minigameData = new MinigameData[maxMinigames+1];

        foreach (MinigameTaskDTO minigameDTO in minigameDTOs)
        {
            string game;
            string configurationId;
            MinigameStatus status = MinigameStatus.notConfigurated;
            int highscore = 0;

            game = minigameDTO.game;
            configurationId = minigameDTO.configurationId;
            if(configurationId != "")
            {
                status = MinigameStatus.active;
            }
            MinigameData minigame = new MinigameData(game, configurationId, status, highscore);
            minigameData[minigameDTO.index] = minigame;
        }

        return minigameData;
    }

    //converts NPCDTOs in npcData
    private NPCData[] getNPCData(List<NPCDTO> npcDTOs)
    {
        NPCData[] npcData = new NPCData[maxNPCs+1];

        foreach (NPCDTO npcDTO in npcDTOs)
        {
            string uuid;
            string[] dialogue;
            bool hasBeenTalkedTo = false;

            uuid = npcDTO.id;
            dialogue = npcDTO.text.ToArray();

            NPCData npc = new NPCData(uuid, dialogue, hasBeenTalkedTo);
            npcData[npcDTO.index] = npc;
        }

        return npcData;
    }

    //converts dungeonDTO in dungeonData
    private DungeonData[] getDungeonData(List<DungeonDTO> dungeonDTOs)
    {
        DungeonData[] dungeonData = new DungeonData[maxDungeons+1];

        foreach(DungeonDTO dungeonDTO in dungeonDTOs)
        {
            string id = dungeonDTO.id;
            int index = dungeonDTO.index;
            string staticName = dungeonDTO.staticName;
            string topicName = dungeonDTO.topicName;
            bool active = dungeonDTO.active;

            List<MinigameTaskDTO> minigameDTOs = dungeonDTO.getMinigameTasks();
            MinigameData[] minigames = getMinigameData(minigameDTOs);

            List<NPCDTO> npcDTOs = dungeonDTO.getNPCs();
            NPCData[] npcs = getNPCData(npcDTOs);

            DungeonData dungeon = new DungeonData(id, index, staticName, topicName, active, minigames, npcs);
            dungeonData[index] = dungeon;
        }

        return dungeonData;
    }

    /// <summary>
    /// This function processes the player minigame statistics data returned form backend and stores the needed data in the <c>GameManager</c>
    /// </summary>
    /// <param name="playerTaskStatistics">The player minigame statistics data returned from the backend</param>
    private void processPlayerTaskStatisitcs(PlayerTaskStatisticDTO[] playerTaskStatistics)
    {
        if(playerTaskStatistics == null)
        {
            return;
        }
        foreach(PlayerTaskStatisticDTO statistic in playerTaskStatistics)
        {
            int worldIndex = statistic.minigameTask.area.worldIndex;
            
            if(worldIndex < 0 || worldIndex >= worldData.Length)
            {
                break;
            }

            int dungeonIndex = statistic.minigameTask.area.dungeonIndex;
            int index = statistic.minigameTask.index;
            int highscore = statistic.highscore;
            bool completed = statistic.completed;
            MinigameStatus status = MinigameStatus.notConfigurated;
            if(minigameStatus(worldIndex, dungeonIndex, index) != MinigameStatus.notConfigurated)
            {
                if(completed)
                {
                    status = MinigameStatus.done;
                }
                else
                {
                    status = MinigameStatus.active;
                }
            }
            worldData[worldIndex].setMinigameStatus(dungeonIndex, index, status);
            worldData[worldIndex].setMinigameHighscore(dungeonIndex, index, highscore);
        }
    }

    //returns the status of a minigame
    private MinigameStatus minigameStatus(int worldIndex, int dungeonIndex, int index)
    {
        if(worldIndex < 0 || worldIndex >= worldData.Length)
        {
            return MinigameStatus.notConfigurated;
        }

        if(dungeonIndex != 0)
        {
            return worldData[worldIndex].getMinigameStatus(dungeonIndex, index);
        }
        else
        {
            return worldData[worldIndex].getMinigameStatus(index);
        }
    }

    /// <summary>
    /// This function processes the player npc statistcs data returned from the backend and stores the needed data in the <c>GameManager</c>
    /// </summary>
    /// <param name="playerNPCStatistics">The player npc statistics data returned from the backend</param>
    private void processPlayerNPCStatistics(PlayerNPCStatisticDTO[] playerNPCStatistics)
    {
        if(playerNPCStatistics == null)
        {
            return;
        }
        foreach(PlayerNPCStatisticDTO statistic in playerNPCStatistics)
        {
            int worldIndex = statistic.npc.area.worldIndex;
            int dungeonIndex = statistic.npc.area.dungeonIndex;
            int index = statistic.npc.index;
            bool completed = statistic.completed;

            if(worldIndex < worldData.Length)
            {
                worldData[worldIndex].setNPCStatus(dungeonIndex, index, completed);
            }
        }
    }
    #endregion

    #region SettingData
    //sets data to objects, called by Scene transition or loading screen
    public void setData(int worldIndex, int dungeonIndex)
    {
        currentWorld = worldIndex;
        currentDungeon = dungeonIndex;
        if(dungeonIndex != 0)
        {
            Debug.Log("Setting data for dungeon " + worldIndex + "-" + dungeonIndex);
            setDungeonData(worldIndex, dungeonIndex);
        }
        else
        {
            Debug.Log("Setting data for world " + worldIndex);
            setWorldData(worldIndex);
        }
    }

    //sets world data
    private void setWorldData(int worldIndex)
    {
        if(worldIndex < 1 || worldIndex >= worldData.Length)
        {
            return;
        }
        WorldData data = worldData[worldIndex];

        for(int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData minigameData = data.getMinigameData(minigameIndex);
            if(minigameData == null)
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
            minigame.setup(minigameData);
        }

        for(int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            NPCData npcData = data.getNPCData(npcIndex);
            if(npcData == null)
            {
                npcData = new NPCData();
            }
            GameObject npcObject = npcObjects[worldIndex, npcIndex];
            if (npcObject == null)
            {
                continue;
            }
            NPC npc = npcObject.GetComponent<NPC>();
            if(npc == null)
            {
                continue;
            }
            npc.setup(npcData);
        }

        for(int barrierDestinationIndex = 1; barrierDestinationIndex <= maxWorld; barrierDestinationIndex++)
        {
            GameObject barrierObject = barrierObjects[worldIndex, barrierDestinationIndex];
            if(barrierObject == null)
            {
                continue;
            }
            Barrier barrier = barrierObject.GetComponent<Barrier>();
            if(barrier == null)
            {
                continue;
            }
            bool activedByLecturer = worldData[barrierDestinationIndex].isActive();
            bool unlockedByPlayer = playerHasWorldUnlocked(barrierDestinationIndex);
            bool worldExplorable = activedByLecturer & unlockedByPlayer;
            BarrierData barrierData = new BarrierData(!worldExplorable);
            barrier.setup(barrierData);
        }
    }

    //has player unlocked the world
    private bool playerHasWorldUnlocked(int worldIndex)
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

    //setup dungeon data
    private void setDungeonData(int worldIndex, int dungeonIndex)
    {
        if (worldIndex < 1 || worldIndex >= worldData.Length)
        {
            return;
        }

        DungeonData data = worldData[worldIndex].getDungeonData(dungeonIndex);
        if(data == null)
        {
            return;
        }

        for (int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData minigameData = data.getMinigameData(minigameIndex);
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
            minigame.setup(minigameData);
        }

        for (int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            NPCData npcData = data.getNPCData(npcIndex);
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
            npc.setup(npcData);
        }
    }
    
    //mark npc as read
    public async void markNPCasRead(string uuid)
    {
        if (active)
        {
            string path = "/overworld/api/v1/internal/submit-npc-pass";
            await postNPCCompleted(path, uuid);
        }
    }
    #endregion
}
