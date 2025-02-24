using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///     Handles the UI menu and user interaction for the multiplayer.
/// </summary>
public class MultiplayerUI : MonoBehaviour
{
    [Header("Action Toggle")]
    [SerializeField] private Button toggle;
    [SerializeField] private TextMeshProUGUI buttonText;
    [Header("Connection Status Panel")]
    [SerializeField] private TextMeshProUGUI playerNumber;
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject disconnectionPanel;
    [Header("Feedback Window")]
    [SerializeField] private GameObject feedbackWindow;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject errorButton;
    [SerializeField] private GameObject confirmButton;

    private const string multiplayerBackendPath = "/multiplayer/api/v1";
    private const string errorText = "Cannot contact the server.\r\nPlease try again later.";
    private const string confirmationText = "Are your sure you want to quit the multiplayer?";

    private void Start()
    {
        SetConnectionStatus(MultiplayerManager.Instance.IsConnected());
    }

    private void Update()
    {
        playerNumber.text = MultiplayerManager.Instance.GetNumberOfConnectedPlayers().ToString();
    }

    #region button functions
    /// <summary>
    ///     Starts or quits the multiplayer, depending of the current connection state.
    /// </summary>
    public async void ToggleMultiplayer()
    {
        if (!MultiplayerManager.Instance.IsConnected())
        {
            bool successful = false;

            try
            {
                successful = await InitConnection();
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e);
                ShowFeedbackWindow(FeedbackType.Error);
            }

            if (successful)
            {
                MultiplayerManager.Instance.Initialize();
                UpdateToggle();
            }
            else
            {
                Debug.LogError("Unable to connect to server");
                ShowFeedbackWindow(FeedbackType.Error);
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
        MultiplayerManager.Instance.QuitMultiplayer();
    }

    /// <summary>
    ///     Closes the feedback window.
    /// </summary>
    public void CloseFeedbackWindow()
    {
        feedbackWindow.SetActive(false);
    }
    #endregion

    /// <summary>
    ///     Contacts the multiplayer server to establish a new connection.
    /// </summary>
    /// <returns>was the connection successful?</returns>
    private async UniTask<bool> InitConnection()
    {
        ConnectionDTO connectionDTO = new(GameManager.Instance.GetUserId(), GameManager.Instance.GetCourseId());
        string basePath = multiplayerBackendPath + "/join";
        string json = JsonUtility.ToJson(connectionDTO, true);

        Optional<ResponseDTO> data = await RestRequest.PostRequest<ResponseDTO>(basePath, json);

        if (data.IsPresent())
        {
            MultiplayerManager.Instance.SetPlayerId(data.Value().playerId);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///     Updates the multiplayer toggle button.
    /// </summary>
    private void UpdateToggle()
    {
        SetConnectionStatus(MultiplayerManager.Instance.IsConnected());
    }

    /// <summary>
    ///     Sets the connection state panel and toggle according the current multiplayer status.
    /// </summary>
    /// <param name="isConnected">is the connection open?</param>
    private void SetConnectionStatus(bool isConnected)
    {
        if (isConnected)
        {
            buttonText.text = "QUIT";
            disconnectionPanel.SetActive(false);
            connectionPanel.SetActive(true);
            ColorBlock cb = toggle.colors;
            cb.highlightedColor = new Color(255, 174, 174, 255);
            toggle.colors = cb;
        }
        else
        {
            buttonText.text = "START";
            connectionPanel.SetActive(false);
            disconnectionPanel.SetActive(true);
            ColorBlock cb = toggle.colors;
            cb.highlightedColor = new Color(167, 255, 146, 255);
            toggle.colors = cb;
        }
    }

    /// <summary>
    ///     Opens the feedback window with a description.
    /// </summary>
    /// <param name="type">type of the feedback</param>
    public void ShowFeedbackWindow(FeedbackType type)
    {
        feedbackWindow.SetActive(true);
        switch (type)
        {
            case FeedbackType.Confirmation:
                feedbackText.text = confirmationText;
                errorButton.SetActive(false);
                confirmButton.SetActive(true);
                break;
            case FeedbackType.Error:
                feedbackText.text = errorText;
                confirmButton.SetActive(false);
                errorButton.SetActive(true);
                break;
        }
    }
}
