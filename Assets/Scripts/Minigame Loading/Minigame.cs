using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This enum is used to store the state of a minigame as follows:
///     notConfigurated -> the minigame was not set by the courses and therefor is not part of the game
///     active -> the minigame can be played but was not completed yet
///     done -> the minigame was completed
/// </summary>
public enum MinigameStatus
{
    notConfigurated,
    active,
    done
}

/// <summary>
///     This script defines a minigame spot in a world or dungeon.
/// </summary>
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

    /// <summary>
    ///     The Start function is called when the object is initialized and sets up the starting values and state of the
    ///     object.
    ///     The function registers the minigame at the game manager and sets the default status to be notConfigurated.
    /// </summary>
    private void Awake()
    {
        sprites = transform.GetComponent<SpriteRenderer>();
        RegisterToGameManager();
        UpdateStatus();
    }

    /// <summary>
    ///     This function removes the minigame from the game manager if it is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("remove Minigame " + world + "-" + dungeon + "-" + number);
        GameManager.instance.removeMinigame(world, dungeon, number);
    }

    /// <summary>
    ///     This function registers the minigame at the game manager
    /// </summary>
    private void RegisterToGameManager()
    {
        Debug.Log("register Minigame " + world + "-" + dungeon + "-" + number);
        GameManager.instance.addMinigame(gameObject, world, dungeon, number);
    }

    #endregion

    #region Functionality

    /// <summary>
    ///     This functions configurates the minigame with the given data and updates the object.
    /// </summary>
    /// <param name="data">the data to be set</param>
    public void Setup(MinigameData data)
    {
        status = data.GetStatus();
        game = data.GetGame();
        configurationID = data.GetConfigurationID();
        highscore = data.GetHighscore();

        UpdateStatus();
    }

    /// <summary>
    ///     This function manages what happenes when the player enters the minigame spot.
    ///     Currently, the minigame status gets set to be done.
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player enters minigame " + game + ", config: " + configurationID);
            StartCoroutine(LoadMinigameStarting());
        }
    }

    /// <summary>
    ///     This function loads the minigame starting scene.
    ///     After this it sets the respawnPostion and sends this to the GameManager.
    ///     Also it sets up the minigame starting.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadMinigameStarting()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync("MinigameStarting Overlay", LoadSceneMode.Additive);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }

        Vector2 respawnPosition = transform.position;
        GameManager.instance.setMinigameRespawn(respawnPosition, world, dungeon);
        MinigameStarting.Instance.SetupMinigame(game, configurationID, highscore);
    }

    /// <summary>
    ///     This function updates the object status.
    /// </summary>
    private void UpdateStatus()
    {
        switch (status)
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

    /// <summary>
    ///     This function returns the info of the minigame object.
    /// </summary>
    /// <returns>info</returns>
    public string GetInfo()
    {
        string info = world + "-" + dungeon + "-" + number + ": Game: " + game + ", Config: " + configurationID +
                      ", Status: " + status + ", Highscore: " + highscore;
        return info;
    }

    #endregion

    #region Getter

    /// <summary>
    ///     This function returns the world of the minigame.
    /// </summary>
    /// <returns>world</returns>
    public int GetWorldIndex()
    {
        return world;
    }

    /// <summary>
    ///     This function returns the dungeon of the minigame.
    /// </summary>
    /// <returns>dungeon</returns>
    public int GetDungeonIndex()
    {
        return dungeon;
    }

    /// <summary>
    ///     This function returns the number of the minigame.
    /// </summary>
    /// <returns>number</returns>
    public int GetIndex()
    {
        return number;
    }

    #endregion
}