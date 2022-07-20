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
        registerToGameManager();
        //status = MinigameStatus.notConfigurated;
        updateStatus();
    }

    //register minigame to game manager
    private void registerToGameManager()
    {
        Debug.Log("register Minigame");
        GameManager.instance.addMinigame(this.gameObject, world, number);
    }

    //setup minigame
    public void setup(MinigameData data)
    {
        this.status = data.getStatus();
        updateStatus();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            status = MinigameStatus.done;
            updateStatus();
        }
    }

    private void updateStatus()
    {
        switch(status)
        {
            case MinigameStatus.notConfigurated: 
                this.gameObject.SetActive(false);
                break;
            case MinigameStatus.active: 
                this.gameObject.SetActive(true);
                //right animation
                break;
            case MinigameStatus.done: 
                this.gameObject.SetActive(true);
                //right animation
                break;
        }
    }
}
