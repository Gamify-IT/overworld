using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// The <c>GameManager</c> manages the communication between the overworld backend and frontend. 
/// Every minigame/barrier/npc registers itself in their <c>Start</c> function, so that the <c>GameManager</c> knows about them.
/// If the Player enters an area, the <c>GameManager</c> gets all needed data from the backend and configurates the objects accordingly.
/// </summary>
public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager instance { get; private set; }

    /// <summary>
    /// This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("End Gamemanager init");
            setupGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Attributes
    private static int maxWorld = 5;
    private static int maxMinigames = 12;
    private static int maxNPCs = 10;
    private static int courseId = 15;
    private static int playerId = 1;
    private MinigameData[,] minigameData = new MinigameData[maxWorld + 1, maxMinigames + 1];
    private GameObject[,] minigameObjects = new GameObject[maxWorld + 1, maxMinigames + 1];
    private BarrierData[,] barrierData = new BarrierData[maxWorld + 1, maxWorld + 1];
    private GameObject[,] barrierObjects = new GameObject[maxWorld + 1, maxWorld + 1];
    private NPCData[,] npcData = new NPCData[maxWorld + 1, maxNPCs+1];
    private GameObject[,] npcObjects = new GameObject[maxWorld + 1, maxNPCs+1];
    private bool somethingToUpdate;
    private int currentWorld;
    private int currentDungeon;
    #endregion

    #region Setup
    /// <summary>
    /// This function initializes the <c>GameManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    private void setupGameManager()
    {
        instance = this;
        for (int i = 0; i < maxWorld; i++)
        {
            for (int j = 0; j < maxMinigames; j++)
            {
                minigameObjects[i, j] = null;
                minigameData[i, j] = new MinigameData("", "", MinigameStatus.notConfigurated, 0);
            }
            for (int j = 0; j < maxWorld; j++)
            {
                barrierObjects[i, j] = null;
                barrierData[i, j] = new BarrierData(true);
            }
            for(int j = 0; j < maxNPCs; j++)
            {
                npcObjects[i, j] = null;
                string[] emptyArray = {""};
                npcData[i, j] = new NPCData("", emptyArray, true);
            }
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
            if(dungeon == 0)
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
    /// This function registers a new npc at the <c>GameManager</c>
    /// </summary>
    /// <param name="npc">The npc gameObject</param>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void addNPC(GameObject npc, int world, int dungeon, int number)
    {
        if(npc != null)
        {
            if(dungeon == 0)
            {
                npcObjects[world, number] = npc;
            }
            else
            {
                npcObjects[0, number] = npc;
            }
        }
    }
    #endregion

    #region Loading
    /// <summary>
    /// This function loads all needed data for a given area
    /// </summary>
    /// <param name="world">The index of the world to load</param>
    /// <param name="dungeon">The index of the dungeon to load (0, if a world should be loaded)</param>
    public void loadWorld(int world, int dungeon)
    {
        currentWorld = world;
        currentDungeon = dungeon;
        if(dungeon == 0)
        {
            Debug.Log("Update world: " + world);
            resetDungeonSlot();
            fetchWorldData(world);
        }
        else
        {
            Debug.Log("Update dungeon: " + world + "-" + dungeon);
            fetchDungeonData(world, dungeon);
        }
    }

    /// <summary>
    /// This function resets saved dungeon objects after exit to reuse the variables for another dungeon
    /// </summary>
    private void resetDungeonSlot()
    {
        for(int minigameIndex = 0; minigameIndex <= maxMinigames; minigameIndex++)
        {
            minigameObjects[0,minigameIndex] = null;
            minigameData[0,minigameIndex] = new MinigameData("", "", MinigameStatus.notConfigurated, 0);
        }
        for(int npcIndex = 0; npcIndex <= maxNPCs; npcIndex++)
        {
            npcObjects[0, npcIndex] = null;
            npcData[0, npcIndex] = new NPCData("", null, true);
        }
    }

    /// <summary>
    /// This function checks, if there is new data and if so, it configurates the registers objects accordingly
    /// </summary>
    public void Update()
    {
        if (somethingToUpdate)
        {
            somethingToUpdate = false;
            if(currentDungeon == 0)
            {
                Debug.Log("Setting Data for World " + currentWorld);
                setData(currentWorld);
            }
            else
            {
                Debug.Log("Setting Data for Dungeon " + currentWorld + "-" + currentDungeon);
                setData(0);
            }
        }

        if (Input.GetKeyDown("h"))
        {
            loadWorld(currentWorld, currentDungeon);
        }
    }

    /// <summary>
    /// This function loads all needed data for a given world
    /// </summary>
    /// <param name="worldIndex">The index of the world to load</param>
    private void fetchWorldData(int worldIndex)
    {
        //path to get world data from
        string path = "/overworld/api/v1/courses/" + courseId + "/worlds/";

        //get world data        
        StartCoroutine(GetWorldDTO(path, worldIndex));

        //get barrier data
        for(int worldIndexDestination=1; worldIndexDestination<=maxWorld; worldIndexDestination++)
        {
            if(barrierObjects[worldIndex, worldIndexDestination] != null)
            {
                StartCoroutine(GetBarrierData(path, worldIndexDestination, worldIndex));
            }
        }

        //get player minigame data
        path = "/overworld/api/v1/courses/" + courseId + "/playerstatistics/" + playerId + "/player-task-statistics";
        StartCoroutine(GetPlayerMinigameStatistics(path));

        //get player data
        path = "/overworld/api/v1/courses/" + courseId + "/playerstatistics/" + playerId;
        StartCoroutine(GetPlayerStatistics(path));

        //get player npc data
        path = "/overworld/api/v1/courses/" + courseId + "/playerstatistics/" + playerId + "/player-npc-statistics";
        StartCoroutine(GetPlayerNPCStatistics(path));
    }

    /// <summary>
    /// This function loads all needed data for a given dungeon
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon to load</param>
    private void fetchDungeonData(int worldIndex, int dungeonIndex)
    {
        //path to get world from (../world/)
        //int courseId = 1;
        string path = "/overworld/api/v1/courses/" + courseId + "/worlds/" + worldIndex + "/dungeons/";

        //get world data        
        StartCoroutine(GetWorldDTO(path, dungeonIndex));
    }
    #endregion

    #region GetRequests
    /// <summary>
    /// This function sends a GET request to the backend to get general data and stores the results in the <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <param name="worldIndex">The world index to be requested at the backend</param>
    /// <returns></returns>
    private IEnumerator GetWorldDTO(String uri, int worldIndex)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + worldIndex))
        {
            Debug.Log("Get Request for world: " + worldIndex);
            Debug.Log("Path: " + uri + worldIndex);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

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
                    processWorldDTO(worldIndex, worldDTO);
                    break;
            }
        }
        somethingToUpdate = true;
    }

    /// <summary>
    /// This function sends a GET request to the backend to get barrier data and stores the results in the <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <param name="worldIndexDestination">The index of the world which entry the barrier is blocking</param>
    /// <param name="worldIndexOrigin">The index of the world which exit the barrier is blocking</param>
    /// <returns></returns>
    private IEnumerator GetBarrierData(String uri, int worldIndexDestination, int worldIndexOrigin)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri + worldIndexDestination))
        {
            Debug.Log("Get Barrier data to world: " + worldIndexDestination);
            Debug.Log("Path: " + uri + worldIndexDestination);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri + worldIndexDestination + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + worldIndexDestination + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + worldIndexDestination + ":\nReceived: " + webRequest.downloadHandler.text);
                    WorldDTO worldDTODestination = JsonUtility.FromJson<WorldDTO>(webRequest.downloadHandler.text);
                    setupBarrier(worldIndexOrigin, worldDTODestination);
                    break;
            }
        }
        somethingToUpdate = true;
    }
    
    /// <summary>
    /// This function sends a GET request to the backend to get player data for minigames and stores the results in the <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <returns></returns>
    private IEnumerator GetPlayerMinigameStatistics(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Player minigame statistics: ");
            Debug.Log("Path: " + uri);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

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
                    processPlayerTaskStatisitcs(playerTaskStatistics);
                    break;
            }
        }
        somethingToUpdate = true;
    }

    /// <summary>
    /// This function sends a GET request to the backend to get gerneral player data and logs the knowledge to the console
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <returns></returns>
    private IEnumerator GetPlayerStatistics(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Player minigame statistics: ");
            Debug.Log("Path: " + uri);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

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
                    Debug.Log("Player knowledge: " + playerStatistic.knowledge);
                    break;
            }
        }
    }

    /// <summary>
    /// This function sends a GET request to the backend to get player data for npcs and stores the results in the <c>GameManager</c>
    /// </summary>
    /// <param name="uri">The path to send the GET request to</param>
    /// <returns></returns>
    private IEnumerator GetPlayerNPCStatistics(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("Get Player minigame statistics: ");
            Debug.Log("Path: " + uri);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

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
                    PlayerNPCStatisticDTO[] playerNPCStatistics = JsonHelper.getJsonArray<PlayerNPCStatisticDTO>(webRequest.downloadHandler.text);
                    processPlayerNPCStatistics(playerNPCStatistics);
                    break;
            }
        }
        somethingToUpdate = true;
    }
    #endregion

    #region PostRequests
    /// <summary>
    /// This function sends a POST request to the backend set, if a npc has been talked to or not
    /// </summary>
    /// <param name="uri">The path to send the POST request to</param>
    /// <param name="uuid">The uuid of the npc</param>
    /// <param name="completed">if the npc has been talked to or not</param>
    /// <returns></returns>
    private IEnumerator PostNPCCompleted(string uri, string uuid, bool completed)
    {
        NPCTalkEvent npcData = new NPCTalkEvent(uuid, completed, playerId.ToString());
        string json = JsonUtility.ToJson(npcData, true);

        UnityWebRequest www = UnityWebRequest.Post(uri, json);
        www.SetRequestHeader("Content-Type", "application/json");
        using (www)
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("NPC mit uuid: " + uuid +  " updated to new status: " + completed);
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
    private void processWorldDTO(int worldIndex, WorldDTO worldDTO)
    {
        Debug.Log("static name: " + worldDTO.getStaticName());
        Debug.Log("topic name: " + worldDTO.getTopicName());
        List<MinigameTaskDTO> minigames = worldDTO.getMinigameTasks();
        foreach(MinigameTaskDTO minigameTask in minigames)
        {
            if(minigameTask.getConfigurationId() == null)
            {
                Debug.Log("Minigame " + minigameTask.getIndex() + " is null");
            }

            if(minigameTask.getConfigurationId() == "" || minigameTask.getConfigurationId() == null)
            {
                minigameData[worldIndex, minigameTask.getIndex()].setStatus(MinigameStatus.notConfigurated);
                Debug.Log("Minigame " + minigameTask.getIndex() + ", status: not configurated");
            }
            else
            {
                minigameData[worldIndex, minigameTask.getIndex()].setStatus(MinigameStatus.active);
                Debug.Log("Minigame " + minigameTask.getIndex() + ", status: active");
            }
            minigameData[worldIndex, minigameTask.getIndex()].setGame(minigameTask.getGame());
            Debug.Log("Minigame " + minigameTask.getIndex() + ", game: " + minigameTask.getGame());
            minigameData[worldIndex, minigameTask.getIndex()].setConfigurationID(minigameTask.getConfigurationId());
            Debug.Log("Minigame " + minigameTask.getIndex() + ", config: " + minigameTask.getConfigurationId());
        }
        List<NPCDTO> npcs = worldDTO.getNPCs();
        foreach(NPCDTO npc in npcs)
        {
            string[] dialogue = npc.getText().ToArray();
            Debug.Log("NPC " + npc.getIndex() + ", text: " + dialogue[0]);
            npcData[worldIndex, npc.getIndex()].setDialogue(dialogue);
            npcData[worldIndex, npc.getIndex()].setUUID(npc.getId());
            string[] newDialogue = npcData[worldIndex, npc.getIndex()].getDialogue();
            Debug.Log("NPC " + npc.getIndex() + ", npcData text: " + newDialogue[0]);
        }
    }

    /// <summary>
    /// This function processes the world data returned from the backend and stores the needed data in the <c>GameManager</c>
    /// </summary>
    /// <param name="worldIndexOrigion">The world index which exit the barrier is blocking</param>
    /// <param name="worldDTODestination">The world data returned from the backend for the world which entry the barrier is blocking</param>
    private void setupBarrier(int worldIndexOrigion, WorldDTO worldDTODestination)
    {
        int worldIndexDestination = worldDTODestination.getIndex();
        if(worldDTODestination.getActive())
        {
            barrierData[worldIndexOrigion, worldIndexDestination].setIsActive(false);
            Debug.Log("Barrier " + worldIndexOrigion + "->" + worldIndexDestination + ": inactive");
        }
        else
        {
            barrierData[worldIndexOrigion, worldIndexDestination].setIsActive(true);
            Debug.Log("Barrier " + worldIndexOrigion + "->" + worldIndexDestination + ": active");
        }
    }
    
    /// <summary>
    /// This function processes the player minigame statistics data returned form backend and stores the needed data in the <c>GameManager</c>
    /// </summary>
    /// <param name="playerTaskStatistics">The player minigame statistics data returned from the backend</param>
    private void processPlayerTaskStatisitcs(PlayerTaskStatisticDTO[] playerTaskStatistics)
    {
        Debug.Log("processing minigame player data");
        foreach(PlayerTaskStatisticDTO statistic in playerTaskStatistics)
        {
            int worldIndex = statistic.minigameTask.area.worldIndex;
            int dungeonIndex = statistic.minigameTask.area.dungeonIndex;
            int index = statistic.minigameTask.index;
            int highscore = statistic.highscore;
            bool completed = statistic.completed;
            MinigameStatus status = MinigameStatus.active;
            if(completed)
            {
                status = MinigameStatus.done;
            }
            if(dungeonIndex == 0)
            {
                minigameData[worldIndex, index].setHighscore(highscore);
                minigameData[worldIndex, index].setStatus(status);
            }
            else
            {
                minigameData[0, index].setHighscore(highscore);
                minigameData[0, index].setStatus(status);
            }
            Debug.Log("Update minigame " + worldIndex + "-" + dungeonIndex + "-" + index + ": Highscore: " + highscore + ", Status: " + status.ToString());
        }
    }
    
    /// <summary>
    /// This function processes the player npc statistcs data returned from the backend and stores the needed data in the <c>GameManager</c>
    /// </summary>
    /// <param name="playerNPCStatistics">The player npc statistics data returned from the backend</param>
    private void processPlayerNPCStatistics(PlayerNPCStatisticDTO[] playerNPCStatistics)
    {
        Debug.Log("processing npc player data");
        foreach(PlayerNPCStatisticDTO statistic in playerNPCStatistics)
        {
            int worldIndex = statistic.npc.area.worldIndex;
            int dungeonIndex = statistic.npc.area.dungeonIndex;
            int index = statistic.npc.index;
            bool hasBeenTalkedTo = statistic.completed;
            if(dungeonIndex == 0)
            {
                npcData[worldIndex, index].setHasBeenTalkedTo(hasBeenTalkedTo);
            }
            else
            {
                npcData[0, index].setHasBeenTalkedTo(hasBeenTalkedTo);
            }
            Debug.Log("Update npc " + "-" + worldIndex + "-" + dungeonIndex + "-" + index + ": hasBeenTalkedTo: " + hasBeenTalkedTo);
        }
    }
    #endregion

    #region SettingData
    /// <summary>
    /// This function sends all stores data for a given world form the <c>GameManager</c> to the corresponding objects
    /// </summary>
    /// <param name="world">The world index which data should be send</param>
    private void setData(int world)
    {
        for(int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData mingameDataToProceed = minigameData[world,minigameIndex];

            if(minigameObjects[world,minigameIndex] != null)
            {
                Minigame minigame = minigameObjects[world,minigameIndex].GetComponent<Minigame>();
                if(minigame != null && mingameDataToProceed.getGame() != null && mingameDataToProceed.getConfigurationID() != null && mingameDataToProceed.getStatus() != null)
                {
                    Debug.Log("Setup Minigame in " + world + "," + minigameIndex);
                    minigame.setup(mingameDataToProceed);
                }else
                {
                    Debug.Log("Minigame " + minigameIndex + " data is not completely loaded");
                }
            }
        }

        for(int barrierIndex = 1; barrierIndex <= maxWorld; barrierIndex++)
        {
            if(barrierObjects[world, barrierIndex] != null)
            {
                Barrier barrier = barrierObjects[world,barrierIndex].GetComponent<Barrier>();
                if(barrier != null && barrierData[world,barrierIndex].getIsActive() != null)
                {
                    barrier.setup(barrierData[world,barrierIndex]);
                }else
                {
                    Debug.Log("Barrier " + barrierIndex + " data is not completely loaded");
                }
            }
        }

        for(int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            if(npcObjects[world, npcIndex] != null)
            {
                NPC npc = npcObjects[world, npcIndex].GetComponent<NPC>();
                if(npc != null)
                {
                    Debug.Log("Setup NPC in " + world + "," + npcIndex);
                    npc.setup(npcData[world, npcIndex]);
                }else
                {
                    Debug.Log("NPC " + npcIndex + " data is not completely loaded");
                }
            }else
            {
                Debug.Log("NpcObject is null");
            }
        }
    }

    /// <summary>
    /// This function sends a POST request to the backend to set, if a npc has been talked to or not
    /// </summary>
    /// <param name="uuid">The uuid of the npc</param>
    /// <param name="completed">if the npc has been talked to or not</param>
    public void markNPCasRead(string uuid, bool completed)
    {
        string path = "/overworld/api/v1/internal/submit-npc-pass";
        StartCoroutine(PostNPCCompleted(path, uuid, completed));
    }
    #endregion
}
