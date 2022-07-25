using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This enum is used to store the state of a minigame as follows:
 * notConfigurated -> the minigame was not set by the lectures and therefor is not part of the game
 * active -> the minigame can be played but was not completed yet
 * done -> the minigame was completed
 */
public enum MinigameStatus
{
    notConfigurated,
    active, 
    done
}

/*
 * This script defines a minigame spot in a world or dungeon. 
 */
public class Minigame : MonoBehaviour
{
    #region Attributes
    [SerializeField] private int world;
    [SerializeField] private int number;
    [SerializeField] private string game;
    [SerializeField] private string configurationID;
    [SerializeField] private MinigameStatus status;
    #endregion

    #region Setup
    /*
     * The Start function is called when the object is initialized and sets up the starting values and state of the object. 
     * The function registers the minigame at the game manager and sets the default status to be notConfigurated.
     */
    void Start()
    {
        registerToGameManager();
        updateStatus();
    }

    /*
     * This function registers the minigame at the game manager
     */
    private void registerToGameManager()
    {
        Debug.Log("register Minigame " + world + " - " + number);
        GameManager.instance.addMinigame(this.gameObject, world, number);
    }
    #endregion

    #region Functionality
    /*
     * This functions configurates the minigame with the given data and updates the object.
     * @param data: the data to be set
     */
    public void setup(MinigameData data)
    {
        status = data.getStatus();
        game = data.getGame();
        configurationID = data.getConfigurationID();
        updateStatus();
    }

    /*
     * This function manages what happenes when the player enters the minigame spot.
     * Currently, the minigame status gets set to be done.
     */
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            status = MinigameStatus.done;
            updateStatus();
        }
    }

    /*
     * This function updates the object status.
     */
    private void updateStatus()
    {
        switch(status)
        {
            case MinigameStatus.notConfigurated: 
                gameObject.SetActive(false);
                break;
            case MinigameStatus.active: 
                gameObject.SetActive(true);
                //right animation
                break;
            case MinigameStatus.done: 
                gameObject.SetActive(true);
                //right animation
                break;
        }
    }
    #endregion
}
