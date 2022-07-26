using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private MinigameData[,] minigameData = new MinigameData[maxWorld+1,maxMinigames+1];
    private GameObject[,] minigameObjects = new GameObject[maxWorld+1,maxMinigames+1];
    private BarrierData[,] barrierData = new BarrierData[maxWorld+1, maxWorld+1];
    private GameObject[,] barrierObjects = new GameObject[maxWorld+1, maxWorld+1];
    #endregion

    #region Setup

    /*
     * This function sets up the game manager. 
     */
    private void setupGameManager()
    {
        instance = this;
        for(int i=0; i<maxWorld; i++)
        {
            for(int j=0; j<maxMinigames; j++)
            {
                minigameObjects[i,j] = null;
                minigameData[i,j] = new MinigameData("", "", MinigameStatus.notConfigurated);
            }
            for(int j=0; j<maxWorld; j++)
            {
                barrierObjects[i, j] = null;
                barrierData[i, j] = new BarrierData(true); 
            }
        }
    }

    /*
     * This function registers a minigame at the game manager
     */
    public void addMinigame(GameObject minigame, int world, int number)
    {
        if(minigame != null)
        {
            minigameObjects[world,number] = minigame;
        }
    }

    /*
     * This function registers a barrier at the game manager
     */
    public void addBarrier(GameObject barrier, int worldIndexOrigin, int worldIndexDestination)
    {
        if(barrier != null)
        {
            barrierObjects[worldIndexOrigin, worldIndexDestination] = barrier;
        }
    }
    #endregion

    #region Loading
    public void loadWorld(int world)
    {
        Debug.Log("Update " + world);
        getDataStatic(world);
        setData(world);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            getDataStatic(1);
            setData(1);
        }
    }

    //get world data
    private void getDataStatic(int world)
    {
        minigameData[1,1] = new MinigameData("Moorhuhn", "config" ,MinigameStatus.active);
        minigameData[1,2] = new MinigameData("", "", MinigameStatus.notConfigurated);
        minigameData[3,1] = new MinigameData("Moorhuhn", "config", MinigameStatus.active);
        barrierData[1,2] = new BarrierData(false);
    }

    private void getData(int world)
    {
        //get world data


        //get barrier data
        //-> für jede angelegte Barrier zu world x (barrierObjects[world,x] != null):
        //   get world x, falls active -> barriere ausschalten
        //              , sonst -> barriere anschalten
    }

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
    }
    #endregion
}
