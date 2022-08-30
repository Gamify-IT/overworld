using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

/*
 * This enum is used to store the state of a minigame as follows:
 * notConfigurated -> the minigame was not set by the courses and therefor is not part of the game
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
    [DllImport("__Internal")]
    private static extern string LoadMinigameInIframe(string minigameName, string minigameConfiguration);

    #region Attributes
    [SerializeField] private int world;
    [SerializeField] private int dungeon;
    [SerializeField] private int number;
    [SerializeField] private string game;
    [SerializeField] private string configurationID;
    [SerializeField] private MinigameStatus status;
    [SerializeField] private int highscore;
    public SpriteRenderer sprites;
    #endregion

    #region Setup
    /*
     * The Start function is called when the object is initialized and sets up the starting values and state of the object. 
     * The function registers the minigame at the game manager and sets the default status to be notConfigurated.
     */
    void Awake()
    {
        sprites = transform.GetComponent<SpriteRenderer>();
        registerToGameManager();
        updateStatus();
    }

    private void OnDestroy()
    {
        Debug.Log("remove Minigame " + world + "-" + dungeon + "-" + number);
        GameManagerV2.instance.removeMinigame(world, dungeon, number);
    }

    /*
     * This function registers the minigame at the game manager
     */
    private void registerToGameManager()
    {
        Debug.Log("register Minigame " + world + "-" + dungeon + "-" + number);
        GameManagerV2.instance.addMinigame(this.gameObject, world, dungeon, number);
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
        highscore = data.getHighscore();
        
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
            Debug.Log("Player enters minigame " + game + ", config: " + configurationID);
            StartCoroutine(loadMinigameStarting());
        }
    }

    //load minigame starting scene
    private IEnumerator loadMinigameStarting()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync("MinigameStarting Overlay", LoadSceneMode.Additive);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }
        Vector2 respawnPosition = this.transform.position;
        GameManagerV2.instance.setMinigameRespawn(respawnPosition, world, dungeon);
        MinigameStarting.instance.setupMinigame(game, configurationID, highscore);
    }

    /*
     * This function updates the object status.
     */
    private void updateStatus()
    {
        switch(status)
        {
            case MinigameStatus.notConfigurated:
                Debug.Log("Minigame " + world + "-" + dungeon + "-" + number + ": color: none");
                sprites.color = new Color(1f, 1f, 1f, 1f);
                gameObject.SetActive(false);
                break;
            case MinigameStatus.active:
                Debug.Log("Minigame " + world + "-" + dungeon + "-" + number + ": color: red");
                sprites.color = new Color(1f, 0f, 0f, 1f);
                gameObject.SetActive(true);
                break;
            case MinigameStatus.done:
                Debug.Log("Minigame " + world + "-" + dungeon + "-" + number + ": color: blue");
                sprites.color = new Color(0f, 0f, 1f, 1f);
                gameObject.SetActive(true);
                break;
        }
    }

    //returns minigame object info
    public string getInfo()
    {
        string info = world + "-" + dungeon + "-" + number + ": Game: " + game + ", Config: " + configurationID + ", Status: " + status.ToString() + ", Highscore: " + highscore;
        return info;
    }
    #endregion

    #region Getter
    public int getWorldIndex()
    {
        return world;
    }

    public int getDungeonIndex()
    {
        return dungeon;
    }

    public int getIndex()
    {
        return number;
    }
    #endregion
}
