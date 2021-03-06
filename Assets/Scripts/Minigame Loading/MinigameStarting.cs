using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;


public class MinigameStarting : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string LoadMinigameInIframe(string minigameName, string minigameConfiguration);

    #region Singleton
    public static MinigameStarting instance { get; private set; }

    /*
     * The Awake function is called after an object is initialized and before the Start function.
     * It sets up the Singleton. 
     */
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region Attributes
    [SerializeField] private TextMeshProUGUI gameText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private string game;
    private string configurationId;
    private int highscore;
    #endregion

    // Update is called once per frame
    void Update()
    {
        //esc handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitMinigame();
        }
    }

    //called by minigame to load canvas
    public void setupMinigame(string game, string configurationId, int highscore)
    {
        Animation.instance.setBusy(true);
        Animation.instance.disableMovement();

        this.game = game;
        this.configurationId = configurationId;
        this.highscore = highscore;

        setupPanel();
    }

    //setup canvas
    private void setupPanel()
    {
        gameText.text = game;
        highscoreText.text = highscore.ToString();
    }

    //start minigame
    public void startButtonPressed()
    {
        LoadMinigameInIframe(game, configurationId);
        quitMinigame();
    }

    //quit minigame
    public void quitButtonPressed()
    {
        quitMinigame();
    }

    private void quitMinigame()
    {
        Reset();
        Animation.instance.setBusy(false);
        Animation.instance.enableMovement();
        SceneManager.UnloadSceneAsync("MinigameStarting Overlay");
    }

    //reset
    private void Reset()
    {
        gameText.text = "";
        highscoreText.text = "";
    }

}
