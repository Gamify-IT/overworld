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
    private static int maxNPCs;
    private MinigameData[,] minigameData = new MinigameData[maxWorld + 1, maxMinigames + 1];
    private GameObject[,] minigameObjects = new GameObject[maxWorld + 1, maxMinigames + 1];
    private BarrierData[,] barrierData = new BarrierData[maxWorld + 1, maxWorld + 1];
    private GameObject[,] barrierObjects = new GameObject[maxWorld + 1, maxWorld + 1];
    private NPCData[,] npcData = new NPCData[maxWorld + 1, maxNPCs];
    private GameObject[,] npcObjects = new GameObject[maxWorld + 1, maxNPCs];
    private bool somethingToUpdate;
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
                npcData[i, j] = new NPCData(null);
            }
        }
    }

    /*
     * This function registers a minigame at the game manager
     */
    public void addMinigame(GameObject minigame, int world, int number)
    {
        if (minigame != null)
        {
            minigameObjects[world, number] = minigame;
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
    public void addNPC(GameObject npc, int world, int number)
    {
        if(npc != null)
        {
            npcObjects[world, number] = npc;
        }
    }
    #endregion

    #region Loading
    public void loadWorld(int world)
    {
        Debug.Log("Update " + world);
        fetchData(world);
    }

    public void Update()
    {
        if (somethingToUpdate)
        {
            somethingToUpdate = false;
            for (int i=1; i<=maxWorld; i++)
            {
                setData(i);
            }
        }
    }

    //get all needed data for a given world
    private void fetchData(int worldIndex)
    {
        //path to get world from (../world/)
        int lectureID = 1;
        string path = "/overworld/api/v1/lectures/" + lectureID + "/worlds/";

        //get world data        
        StartCoroutine(GetWorldDTO(path, worldIndex));

        //get barrier data
        //-> f�r jede angelegte Barrier zu world x (barrierObjects[world,x] != null):
        //   get world x, falls active -> barriere ausschalten
        //              , sonst -> barriere anschalten
        for(int worldIndexDestination=1; worldIndexDestination<=maxWorld; worldIndexDestination++)
        {
            if(barrierObjects[worldIndex, worldIndexDestination] != null)
            {
                StartCoroutine(GetBarrierData(path, worldIndexDestination, worldIndex));
            }
        }
    }
    #endregion

    #region GetRequest
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
        }
    }

    //setup a barrier from world origion to world destination
    private void setupBarrier(int worldIndexOrigion, WorldDTO worldDestination)
    {
        int worldIndexDestination = worldDestination.getIndex();
        if(worldDestination.getActive())
        {
            barrierData[worldIndexOrigion, worldIndexDestination].setIsActive(true);
            Debug.Log("Barrier " + worldIndexOrigion + "->" + worldIndexDestination + ": active");
        }
        else
        {
            barrierData[worldIndexOrigion, worldIndexDestination].setIsActive(false);
            Debug.Log("Barrier " + worldIndexOrigion + "->" + worldIndexDestination + ": inactive");
        }
    }

    #endregion

    #region SettingData
    //set world data
    private void setData(int world)
    {
        for(int minigameIndex = 1; minigameIndex < maxMinigames; minigameIndex++)
        {
            if(minigameObjects[world,minigameIndex] != null)
            {
                Minigame minigame = minigameObjects[world,minigameIndex].GetComponent<Minigame>();
                if(minigame != null)
                {
                    minigame.setup(minigameData[world,minigameIndex]);
                }
            }
        }

        for(int barrierIndex = 1; barrierIndex < maxWorld; barrierIndex++)
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

        for(int npcIndex = 1; npcIndex < maxNPCs; npcIndex++)
        {
            if(npcObjects[world, npcIndex] != null)
            {
                NPC npc = npcObjects[world, npcIndex].GetComponent<NPC>();
                if(npc != null)
                {
                    npc.setup(npcData[world, npcIndex]);
                }
            }
        }
    }
    #endregion
}