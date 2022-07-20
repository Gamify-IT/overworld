using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinigameStatus
{
    notConfigurated,
    active, 
    done
}

public class Minigame : MonoBehaviour
{
    [SerializeField] private int world;
    [SerializeField] private int number;
    [SerializeField] private MinigameStatus status;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //register minigame to game manager
    private void registerToGameManager()
    {
        GameManager.instance.addMinigame(this.gameObject, world, number);
    }

    //setup minigame
    public void setup(MinigameStatus status)
    {
        this.status = status;
    }
}
