using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class manages the minigame starting.
/// </summary>
public class MinigameStarting : MonoBehaviour
{
    //KeyCodes
    private KeyCode cancel;

    private void Start()
    {
        cancel = GameManager.Instance.GetKeyCode(Binding.CANCEL);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
    }

    /// <summary>
    ///     This function resets the game- & hightscoreText.
    /// </summary>
    private void Reset()
    {
        gameText.text = "";
        highscoreText.text = "";
    }

    /// <summary>
    ///     This function is called every frame.
    ///     It checks if the player has pressed esc and if so it quits the minigame starting screen.
    /// </summary>
    private void Update()
    {
        //esc handling
        if (Input.GetKeyDown(cancel))
        {
            QuitMinigame();
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    [DllImport("__Internal")]
    private static extern string LoadMinigameInIframe(string minigameName, string minigameConfiguration);

    /// <summary>
    ///     This function disables movement and starts 'SetupPanel()';
    /// </summary>
    /// <param name="game">Name of the minigame</param>
    /// <param name="configurationId">configurationId of the minigame</param>
    /// <param name="highscore">highscore of the minigame for this configurationId</param>
    public void SetupMinigame(string game, string configurationId, int highscore)
    {
        PlayerAnimation.Instance.playerAnimator.enabled = false;

        PlayerAnimation.Instance.SetBusy(true);
        PlayerAnimation.Instance.DisableMovement();

        this.game = game;
        this.configurationId = configurationId;
        this.highscore = highscore;

        SetupPanel();
    }

    /// <summary>
    ///     Setups the minigame name & highscore.
    /// </summary>
    private void SetupPanel()
    {
        gameText.text = game;
        highscoreText.text = highscore + " %";
    }

    /// <summary>
    ///     This function starts the minigame.
    /// </summary>
    public void StartButtonPressed()
    {
        GameManager.Instance.SaveAchievements();
        LoadMinigameInIframe(game, configurationId);
        QuitMinigame();
    }

    /// <summary>
    ///     This function quits the minigame starting screen on button press.
    /// </summary>
    public void QuitButtonPressed()
    {
        QuitMinigame();
    }

    /// <summary>
    ///     This function quits the minigame starting screen.
    /// </summary>
    private void QuitMinigame()
    {
        Reset();
        PlayerAnimation.Instance.playerAnimator.enabled = true;
        PlayerAnimation.Instance.SetBusy(false);
        PlayerAnimation.Instance.EnableMovement();
        SceneManager.UnloadSceneAsync("MinigameStarting Overlay");
    }

    /// <summary>
    ///     This function updates the keybindings
    /// </summary>
    /// <param name="binding">The binding that changed</param>
    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.CANCEL)
        {
            cancel = GameManager.Instance.GetKeyCode(Binding.CANCEL);
        }
    }

    #region Singleton

    public static MinigameStarting Instance { get; private set; }

    /// <summary>
    ///     The Awake function is called after an object is initialized and before the Start function.
    ///     It sets up the Singleton.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Attributes

    [SerializeField] private TextMeshProUGUI gameText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private string game;
    private string configurationId;
    private int highscore;

    #endregion
}