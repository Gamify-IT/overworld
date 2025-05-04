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
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button replayButton;
    [SerializeField] private Image gameIconImage;

    [System.Serializable] 
    public class MinigameIcon
    {
        public string gameName;
        public Sprite icon;
    }

    [SerializeField] private MinigameIcon[] minigameIcons;

    [DllImport("__Internal")]
    private static extern string LoadMinigameInIframe(string minigameName, string minigameConfiguration);

    private MinigameData minigameData;

    /// <summary>
    /// Search for the image to use as icon for a respective minigame.
    /// </summary>
    /// <param name="gameName"> The name of the minigame.</param> 
    /// <returns></returns>
    private Sprite GetIconForGame(string gameName)
    {
        foreach (var minigameIcon in minigameIcons)
        {
            if (minigameIcon.gameName == gameName)
            {
                return minigameIcon.icon;
            }
        }
        return null;
    }

    /// <summary>
    ///     Sets up the UI for a replay menu item with icon, 
    ///     name, highscore and replay button.
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
        descriptionText.text = minigame.GetDescription();

        replayButton.onClick.RemoveAllListeners();
        replayButton.onClick.AddListener(() => onReplay?.Invoke());

        if (gameIconImage != null)
        {
            var icon = GetIconForGame(minigame.GetGame());
            if (icon != null)
            {
                gameIconImage.sprite = icon;
            }
            else
            {
                Debug.LogWarning($"No icon found for game {minigame.GetGame()}");
            }
        }
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
