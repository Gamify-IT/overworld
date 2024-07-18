using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

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
public class Minigame : MonoBehaviour, IGameEntity<MinigameData>
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
    private static List<(int, int, int)> unlockedMinigames = new List<(int, int, int)>();
    private static List<(int, int, int)> successfullyCompletedMinigames = new List<(int, int, int)>();
    private static List<(string, string)> alreadyPlayed = new List<(string, string)>();

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
    }

    /// <summary>
    ///     This function removes the minigame from the game manager if it is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            Debug.Log("remove Minigame " + world + "-" + dungeon + "-" + number);
            ObjectManager.Instance.RemoveGameEntity<Minigame, MinigameData>(world, dungeon, number);
        }
    }

    /// <summary>
    ///     This function initializes the <c>Minigame</c> object
    /// </summary>
    /// <param name="areaIdentifier">The area the <c>Minigame</c> is in</param>
    /// <param name="index">The index of the <c>Minigame</c> in its area</param>
    public void Initialize(AreaInformation areaIdentifier, int index)
    {
        world = areaIdentifier.GetWorldIndex();
        dungeon = 0;
        if(areaIdentifier.IsDungeon())
        {
            dungeon = areaIdentifier.GetDungeonIndex();
        }
        number = index;

        RegisterToGameManager();
    }

    /// <summary>
    ///     This function registers the minigame at the game manager
    /// </summary>
    private void RegisterToGameManager()
    {
        Debug.Log("register Minigame " + world + "-" + dungeon + "-" + number);
        ObjectManager.Instance.AddGameEntity<Minigame, MinigameData>(gameObject, world, dungeon, number);
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
            var key = (world,dungeon,number);
            if(!unlockedMinigames.Contains(key))
            {
                unlockedMinigames.Add((world,dungeon,number));
                GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINIGAME_SPOTS_FINDER, 1);
                GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINIGAME_SPOTS_MASTER, 1);
            }
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
        GameManager.Instance.SetReloadLocation(respawnPosition, world, dungeon);
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
                UpdateAchievements();
                break;
        }
    }

    /// <summary>
    ///     This functions updates achievements for each minigame.
    /// </summary>
    private void UpdateAchievements()
    {
        var key = (world,dungeon,number);
        if(!successfullyCompletedMinigames.Contains(key))
        {
            successfullyCompletedMinigames.Add((world,dungeon,number));
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINIGAME_ACHIEVER, 1);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINIGAME_PROFESSIONAL, 1);
        }
        if(game=="CHICKENSHOCK"){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.CHICKENSHOCK_MASTER, 1);
        }
        if(game=="MEMORY"){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MEMORY_MASTER, 1);
        }
        if(game=="FINITEQUIZ"){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.FINITEQUIZ_MASTER, 1);
        }
        if(game=="TOWERCRUSH"){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.TOWERCRUSH_MASTER, 1);
        }
        if(game=="CROSSWORDPUZZLE"){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.CROSSWORDPUZZLE_MASTER, 1);
        }
        if(game=="BUGFINDER"){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.BUGFINDER_MASTER, 1);
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
}