using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///     Handles logic for a single replay menu item.
/// </summary>
public class ReplayItemManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameNameText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private Button replayButton;

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

        // Set the text fields
        gameNameText.text = minigame.GetGame();
        highscoreText.text = minigame.GetHighscore() + " %";

        // Add button listener
        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(() => onReplay?.Invoke());
    }
   
    public void ReplayButtonPressed()
    {
        MinigameStarting.Instance.SetupMinigame(
            minigameData.GetGame(),
            minigameData.GetConfigurationID(),
            minigameData.GetHighscore()
        );

        // Trigger the actual start process
        MinigameStarting.Instance.StartButtonPressed();

        FindObjectOfType<ReplayMenu>().ToggleReplayMenu(false);
    }

}
