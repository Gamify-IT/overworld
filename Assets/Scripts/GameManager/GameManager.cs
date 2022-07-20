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
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private MinigameStatus[][] minigameData;
    [SerializeField] private GameObject[][] minigameObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //called by minigame to register
    public void addMinigame(GameObject minigame, int world, int number)
    {
        if(minigame != null)
        {
            minigameObjects[world][number] = minigame;
        }
    }

    //get world status
    private void getStatus(int world)
    {
        minigameData[0][0] = MinigameStatus.active;
    }

    //set world status
    private void setStatus(int world)
    {
        for(int worldIndex = 0; worldIndex < minigameData.Length; worldIndex++)
        {
            for(int minigameIndex = 0; minigameIndex < minigameData[worldIndex].Length; minigameIndex++)
            {
                Minigame minigame = minigameObjects[worldIndex][minigameIndex].GetComponent<Minigame>();
                if(minigame != null)
                {
                    minigame.setup(minigameData[worldIndex][minigameIndex]);
                }
            }
        }
    }
}
