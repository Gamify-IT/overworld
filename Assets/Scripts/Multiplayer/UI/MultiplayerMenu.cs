using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Handles the UI menu and user interaction for the multiplayer.
/// </summary>
public class MultiplayerMenu : MonoBehaviour
{
    [Header("Action Toggle")]
    [SerializeField] private Button toggle;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Connection State Panel")]
    [SerializeField] private TextMeshProUGUI playerNumber;
    [SerializeField] private GameObject statePanel;
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject disconnectionPanel;
    [SerializeField] private GameObject inactivePanel;

    [Header("Feedback Window")]
    [SerializeField] private GameObject feedbackWindow;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject errorButton;
    [SerializeField] private GameObject confirmButton;

    [Header("Sound Effects")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip connectionSound;

    // constant strigs
    private const string errorText = "Cannot contact the server.\r\nPlease try again later.";
    private const string failedReconnectionText = "Reconnection failed.\r\nPlease try again later.";
    private const string disconnectionText = "Connection to the server was lost.\r\nPlease try again later.";
    private const string confirmationText = "Are your sure you want to quit the multiplayer?";

    private void Start()
    {
        UpdateConnectionState();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        playerNumber.text = MultiplayerManager.Instance.GetNumberOfConnectedPlayers().ToString();
        UpdateConnectionState();
    }

    #region button functionality
    /// <summary>
    ///     Starts or quits the multiplayer, depending of the current connection state.
    /// </summary>
    public async void ToggleMultiplayer()
    {
        PlaySound(clickSound);

        if (!MultiplayerManager.Instance.IsConnected())
        {
            bool successful = false;

            try
            {
                successful = await MultiplayerManager.Instance.InitConnection();
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e);
                ShowFeedbackWindow(FeedbackType.Error);
            }

            if (successful)
            {
                MultiplayerManager.Instance.InitializeWebsocket();
                ShowConnectionState(FeedbackType.Connected);
                PlaySound(connectionSound);
            }
            else
            {
                Debug.LogError("Unable to connect to server");
                ShowFeedbackWindow(FeedbackType.Error);
                PlaySound(errorSound);
            }
        }
        else
        {
            ShowFeedbackWindow(FeedbackType.Confirmation);
        }
    }

    /// <summary>
    ///     Quits the multiplayer if the confirm button has been pressed and closes the feedback window.
    /// </summary>
    public void QuitMultiplayer()
    {
        CloseFeedbackWindow();
        MultiplayerManager.Instance.QuitMultiplayer();
        PlaySound(errorSound);
    }

    /// <summary>
    ///     Closes the feedback window.
    /// </summary>
    public void CloseFeedbackWindow()
    {
        feedbackWindow.SetActive(false);
        PlaySound(clickSound);
    }
    #endregion

    #region window and panel handling

    /// <summary>
    ///     Shows the current connection state of the multiplayer connection.
    /// </summary>
    private void UpdateConnectionState()
    {
        if (MultiplayerManager.Instance.IsConnected())
        {
            ShowConnectionState(FeedbackType.Connected);
        }
        else if (MultiplayerManager.Instance.IsInactive())
        {
            ShowConnectionState(FeedbackType.Inactive);
        }
        else
        {
            ShowConnectionState(FeedbackType.Disconnected);
        }
    }

    /// <summary>
    ///     Sets the connection state and toggle according the current multiplayer state.
    /// </summary>
    /// <param name="state">state of the connection</param>
    private void ShowConnectionState(FeedbackType state)
    {
        ColorBlock cb = toggle.colors;

        switch (state)
        {
            case FeedbackType.Connected:
                buttonText.text = "QUIT";
                SetConnectionStatePanel(FeedbackType.Connected);
                cb.highlightedColor = new Color(255, 174, 174, 255);
                toggle.colors = cb;
                return;
            case FeedbackType.Disconnected:
                buttonText.text = "START";
                SetConnectionStatePanel(FeedbackType.Disconnected);
                cb.highlightedColor = new Color(167, 255, 146, 255);
                toggle.colors = cb;
                return;
            case FeedbackType.Inactive:
                buttonText.text = "RECONNECT";
                SetConnectionStatePanel(FeedbackType.Inactive);
                cb.highlightedColor = new Color(255, 174, 174, 255);
                toggle.colors = cb;
                return;
            default:
                return;
        }
    }

    /// <summary>
    ///     Activates the connection state panel of the givne state and deactivates all others.
    /// </summary>
    /// <param name="state">multiplayer connection state</param>
    private void SetConnectionStatePanel(FeedbackType state)
    {
        foreach (Transform item in statePanel.transform)
        {
            if (item.gameObject.name == state.ToString())
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    ///     Opens the feedback window with a specific description.
    /// </summary>
    /// <param name="type">type of the feedback</param>
    public void ShowFeedbackWindow(FeedbackType type)
    {
        feedbackWindow.SetActive(true);
        switch (type)
        {
            case FeedbackType.Confirmation:
                feedbackText.text = confirmationText;
                confirmButton.SetActive(true);
                errorButton.SetActive(false);
                return;
            case FeedbackType.Error:
                feedbackText.text = errorText;
                confirmButton.SetActive(false);
                errorButton.SetActive(true);
                return;
            case FeedbackType.Disconnected:
                feedbackText.text = disconnectionText;
                confirmButton.SetActive(false);
                errorButton.SetActive(true);
                return;
            case FeedbackType.FailedReconnection:
                feedbackText.text = failedReconnectionText;
                confirmButton.SetActive(false);
                errorButton.SetActive(true);
                return;
            default:
                return;
        }
    }
    #endregion

    #region sound effects
    /// <summary>
    ///     Plays the sound effect once.
    /// </summary>
    /// <param name="sound">sound effect to be played</param>
    private void PlaySound(AudioClip sound)
    {
        if (sound != null && audioSource != null)
        {
            audioSource.clip = sound;
            audioSource.PlayOneShot(sound);
        }
    }
    #endregion
}
