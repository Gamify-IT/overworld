using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        Debug.Log("Start Gamemanager init");
        if (instance == null)
        {
            setupGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("End Gamemanager init");
    }
    #endregion

    private static int maxWorld = 5;
    private static int maxMinigames = 12;
    [SerializeField] private MinigameStatus[,] minigameData = new MinigameStatus[maxWorld,maxMinigames];
    [SerializeField] private GameObject[,] minigameObjects = new GameObject[maxWorld,maxMinigames];
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            getStatus(1);
            //wait for answer
            setStatus(1);
        }
    }

    private void setupGameManager()
    {
        instance = this;
        for(int i=0; i<maxWorld; i++)
        {
            for(int j=0; j<maxMinigames; j++)
            {
                minigameObjects[i,j] = null;
                minigameData[i,j] = MinigameStatus.notConfigurated;
            }
        }
    }

    //called by minigame to register
    public void addMinigame(GameObject minigame, int world, int number)
    {
        if(minigame != null)
        {
            minigameObjects[world,number] = minigame;
        }
    }

    //get world status
    private void getStatus(int world)
    {
        minigameData[0,0] = MinigameStatus.active;
        minigameData[0,1] = MinigameStatus.notConfigurated;
    }

    //set world status
    private void setStatus(int world)
    {
        for(int worldIndex = 0; worldIndex < maxWorld; worldIndex++)
        {
            for(int minigameIndex = 0; minigameIndex < maxMinigames; minigameIndex++)
            {
                if(minigameObjects[worldIndex,minigameIndex] != null)
                {
                    Minigame minigame = minigameObjects[worldIndex,minigameIndex].GetComponent<Minigame>();
                    if(minigame != null)
                    {
                        minigame.setup(minigameData[worldIndex,minigameIndex]);
                    }
                }
            }
        }
    }
}
