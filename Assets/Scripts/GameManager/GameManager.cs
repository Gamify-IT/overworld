using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance { get; private set; }

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
    private MinigameData[,] minigameData = new MinigameData[maxWorld,maxMinigames];
    private GameObject[,] minigameObjects = new GameObject[maxWorld,maxMinigames];
    #endregion

    #region Setup
    private void setupGameManager()
    {
        instance = this;
        for(int i=0; i<maxWorld; i++)
        {
            for(int j=0; j<maxMinigames; j++)
            {
                minigameObjects[i,j] = null;
                minigameData[i,j] = new MinigameData(MinigameStatus.notConfigurated);
            }
        }
    }

    //called by minigame to register
    public void addMinigame(GameObject minigame, int world, int number)
    {
        if(minigame != null)
        {
            minigameObjects[world-1,number-1] = minigame;
        }
    }
    #endregion

    #region Loading
    public void loadWorld(int world)
    {
        Debug.Log("Update " + world);
        getStatus(world);
        setStatus(world);
    }

    //get world status
    private void getStatus(int world)
    {
        minigameData[0,0] = new MinigameData(MinigameStatus.active);
        minigameData[0,1] = new MinigameData(MinigameStatus.notConfigurated);
        minigameData[2,0] = new MinigameData(MinigameStatus.active);
    }

    //set world status
    private void setStatus(int world)
    {

            for(int minigameIndex = 0; minigameIndex < maxMinigames; minigameIndex++)
            {
                if(minigameObjects[world-1,minigameIndex] != null)
                {
                    Minigame minigame = minigameObjects[world-1,minigameIndex].GetComponent<Minigame>();
                    if(minigame != null)
                    {
                        minigame.setup(minigameData[world-1,minigameIndex]);
                    }
                }
            }
    }
    #endregion
}
