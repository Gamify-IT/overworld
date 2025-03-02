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

    public AudioClip clickSound;
    private AudioSource audioSource;

    private void Start()
    {
        cancel = GameManager.Instance.GetKeyCode(Binding.CANCEL);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;

        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("Music/click");
        audioSource.clip = clickSound;
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
            PlayClickSound();
            Invoke("QuitMinigame", 0.3f);
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
    public async void StartButtonPressed()
    {
#if UNITY_EDITOR
        GameManager.Instance.MinigameDone();
#else
        await GameManager.Instance.SavePlayerData();
        await MultiplayerManager.Instance.PauseMultiplayer();
        LoadMinigameInIframe(game, configurationId);
#endif
        PlayClickSound();
        Invoke("QuitMinigame", 0.3f);
    }

    /// <summary>
    ///     This function quits the minigame starting screen on button press.
    /// </summary>
    public void QuitButtonPressed()
    {
        PlayClickSound();
        PlayerAnimation.Instance.EnableMovement();
        Invoke("QuitMinigame", 0.3f);
    }

    /// <summary>
    ///     This function quits the minigame starting screen.
    /// </summary>
    private void QuitMinigame()
    {
        Reset();
        PlayerAnimation.Instance.playerAnimator.enabled = true;
        PlayerAnimation.Instance.SetBusy(false);
        SceneManager.UnloadSceneAsync("MinigameStarting Overlay");
        PlayerAnimation.Instance.EnableMovement();
        Time.timeScale = 1f;
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
    /// <summary>
    ///     This function plays click sound 
    /// </summary>
    private void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
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