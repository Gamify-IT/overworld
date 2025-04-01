using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
///     Handles logic for a single replay menu item.
/// </summary>
public class ReplayItemManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameNameText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private Button replayButton;

    [DllImport("__Internal")]
    private static extern string LoadMinigameInIframe(string minigameName, string minigameConfiguration);

    private MinigameData minigameData;

    /// <summary>
    ///     Sets up the UI for a replay menu item.
    /// </summary>
    /// <param name="minigame">The mini-game data.</param>
    /// <param name="onReplay">Callback for when the replay button is clicked.</param>
    public void Setup(MinigameData minigame, System.Action onReplay)
    {
        minigameData = minigame;

        if (gameNameText == null || highscoreText == null || replayButton == null)
        {
            Debug.LogError("ReplayItemUI is missing required UI components.");
            return;
        }

        gameNameText.text = minigame.GetGame();
        highscoreText.text = minigame.GetHighscore() + " %";

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(() => onReplay?.Invoke());
    }
    /// <summary>
    ///     Starts loading the minigame from the replaymenu when the ReplayButton is pressed.
    /// </summary>
    public void ReplayButtonPressed()
    {
        LoadMinigame();
        LoadMinigameInIframe(minigameData.GetGame() , minigameData.GetConfigurationID());
        Vector2 respawnPosition = new Vector2(1.185f, 14.185f);
        GameManager.Instance.SetReloadLocation(respawnPosition, 1, 0);
        FindObjectOfType<ReplayMenu>().ToggleReplayMenu(false);
    }

    /// <summary>
    ///     Method to perform the actual loading of the minigame.
    /// </summary>
    private IEnumerator LoadMinigame()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync("MinigameStarting Overlay", LoadSceneMode.Additive);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }
        MinigameStarting.Instance.SetupMinigame(
            minigameData.GetGame(),
            minigameData.GetConfigurationID(),
            minigameData.GetHighscore()
        );
    }
}
