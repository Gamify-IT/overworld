using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

/*
 * This script defines the game manager. 
 * If the player enteres a world or a dungeon, all data needed gets loaded from the backend and 
 * all minigames, barriers and NPCs get configurated accordingly. 
 */
public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager instance { get; private set; }

    /*
     * The Awake function is called after an object is initialized and before the Start function.
     * It sets up the Singleton. 
     */
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
    private static int lectureID = 1;
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

    /*
     * This function sets up the game manager. 
     */
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
                npcData[i, j] = new NPCData("", null, true);
            }
        }
    }

    /*
     * This function registers a minigame at the game manager
     */
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

    /*
     * This function registers a barrier at the game manager
     */
    public void addBarrier(GameObject barrier, int worldIndexOrigin, int worldIndexDestination)
    {
        if (barrier != null)
        {
            barrierObjects[worldIndexOrigin, worldIndexDestination] = barrier;
        }
    }

    /*
     * This function registers a npc at the game manager
     */
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
    //world: worldIndex,0
    //dungeon: worldIndex,dungeonIndex
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

    //reset dungeon entries 
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
    }

    //get all needed data for a given world
    private void fetchWorldData(int worldIndex)
    {
        //path to get world from (../world/)
        //int lectureID = 1;
        string path = "/overworld/api/v1/lectures/" + lectureID + "/worlds/";

        //get world data        
        StartCoroutine(GetWorldDTO(path, worldIndex));

        //get barrier data
        //-> für jede angelegte Barrier zu world x (barrierObjects[world,x] != null):
        //   get world x, falls active -> barriere ausschalten
        //              , sonst -> barriere anschalten
        for(int worldIndexDestination=1; worldIndexDestination<=maxWorld; worldIndexDestination++)
        {
            if(barrierObjects[worldIndex, worldIndexDestination] != null)
            {
                StartCoroutine(GetBarrierData(path, worldIndexDestination, worldIndex));
            }
        }

        //get player minigame data
        //int playerId = 1;
        path = "/overworld/api/v1/lectures/" + lectureID + "/playerstatistics/" + playerId + "/player-task-statistics";
        StartCoroutine(GetPlayerMinigameStatistics(path));

        //get player data
        path = "/overworld/api/v1/lectures/" + lectureID + "/playerstatistics/" + playerId;
        StartCoroutine(GetPlayerStatistics(path));

        //get player npc data
        path = "/overworld/api/v1/lectures/" + lectureID + "/playerstatistics/" + playerId + "/player-npc-statistics";
        StartCoroutine(GetPlayerNPCStatistics(path));
        
    }

    private void fetchDungeonData(int worldIndex, int dungeonIndex)
    {
        //path to get world from (../world/)
        //int lectureID = 1;
        string path = "/overworld/api/v1/lectures/" + lectureID + "/worlds/" + worldIndex + "/dungeons/";

        //get world data        
        StartCoroutine(GetWorldDTO(path, dungeonIndex));
    }
    #endregion

    #region GetRequests
    //get world data
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

    //get barrier data
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
    
    //get player minigame data
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
    private IEnumerator PostNPCCompleted(string uri, string uuid, bool completed)
    {
        NPCTalkEvent npcData = new NPCTalkEvent(uuid, completed, playerId.ToString());
        string json = JsonUtility.ToJson(npcData);

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
    //process a worldDTO to the minigame data 
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
            string[] dialogue = new string[1];
            dialogue[0] = npc.getText();
            Debug.Log("NPC " + npc.getIndex() + ", text: " + dialogue[0]);
            npcData[worldIndex, npc.getIndex()].setDialogue(dialogue);
            npcData[worldIndex, npc.getIndex()].setUUID(npc.getId());
            string[] newDialogue = npcData[worldIndex, npc.getIndex()].getDialogue();
            Debug.Log("NPC " + npc.getIndex() + ", npcData text: " + newDialogue[0]);
        }
    }

    //setup a barrier from world origion to world destination
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
    
    private void processPlayerTaskStatisitcs(PlayerTaskStatisticDTO[] playerTaskStatistics)
    {
        Debug.Log("processing minigame player data");
        foreach(PlayerTaskStatisticDTO statistic in playerTaskStatistics)
        {
            int worldIndex = statistic.minigameTask.areaLocation.worldIndex;
            int dungeonIndex = statistic.minigameTask.areaLocation.dungeonIndex;
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
    
    private void processPlayerNPCStatistics(PlayerNPCStatisticDTO[] playerNPCStatistics)
    {
        Debug.Log("processing npc player data");
        foreach(PlayerNPCStatisticDTO statistic in playerNPCStatistics)
        {
            int worldIndex = statistic.npc.areaLocation.worldIndex;
            int dungeonIndex = statistic.npc.areaLocation.dungeonIndex;
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
    //set world data
    private void setData(int world)
    {
        for(int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            if(minigameObjects[world,minigameIndex] != null)
            {
                Minigame minigame = minigameObjects[world,minigameIndex].GetComponent<Minigame>();
                if(minigame != null)
                {
                    Debug.Log("Setup Minigame in " + world + "," + minigameIndex);
                    minigame.setup(minigameData[world,minigameIndex]);
                }
            }
        }

        for(int barrierIndex = 1; barrierIndex <= maxWorld; barrierIndex++)
        {
            if(barrierObjects[world, barrierIndex] != null)
            {
                Barrier barrier = barrierObjects[world,barrierIndex].GetComponent<Barrier>();
                if(barrier != null)
                {
                    barrier.setup(barrierData[world,barrierIndex]);
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
                }
            }
        }
    }
    
    public void markNPCasRead(string uuid, bool completed)
    {
        string path = "/overworld/api/v1/internal/submit-npc-pass";
        StartCoroutine(PostNPCCompleted(path, uuid, completed));
    }
    #endregion
}
