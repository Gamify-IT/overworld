using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
///     The replay menu displays completed minigames
///     and allows you to replay them. 
/// </summary>
public class ReplayMenu : MonoBehaviour
{
    [SerializeField] private GameObject replayMenuUI;
    [SerializeField] private Transform replayListContainer;
    [SerializeField] private GameObject replayItemPrefab;

    private List<MinigameData> completedMinigames;

    private void Start()
    {
        completedMinigames = GetCompletedMinigamesData();
        PopulateReplayMenu();
    }

    /// <summary>
    ///     Populates the replay menu with completed mini-games.
    /// </summary>
    private void PopulateReplayMenu()
    {
        foreach (Transform child in replayListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var minigame in completedMinigames)
        {
            GameObject newItem = Instantiate(replayItemPrefab, replayListContainer);
            var itemManager = newItem.GetComponent<ReplayItemManager>();
            if (itemManager != null)
            {
                itemManager.Setup(minigame, itemManager.ReplayButtonPressed);
            }
            else
            {
                Debug.LogError("ReplayItemPrefab is missing the ReplayItemUI component.");
            }
        }
    }

    /// <summary>
    ///     Loads the list of completed mini-games from the game data.
    /// </summary>
    /// <returns>List of completed MinigameData objects.</returns>
    private List<MinigameData> GetCompletedMinigamesData()
    {
        List<MinigameData> completedMinigamesData = new List<MinigameData>();
        Minigame[] allMinigames = FindObjectsOfType<Minigame>();

        foreach (Minigame minigame in allMinigames)
        {
            if (minigame.GetStatus() == MinigameStatus.done)
            {
                MinigameData data = new MinigameData(
                    minigame.GetGame(),
                    minigame.GetConfigurationID(),
                    minigame.GetStatus(),
                    minigame.GetHighscore(),
                    minigame.GetDescription()
                );
                completedMinigamesData.Add(data);
            }
        }

        return completedMinigamesData;
    }

    /// <summary>
    ///     Toggles the visibility of the replay menu.
    /// </summary>
    /// <param name="isVisible">True to show the menu, false to hide it.</param>
    public void ToggleReplayMenu(bool isVisible)
    {
        replayMenuUI.SetActive(isVisible);
        // Pause the game when the menu is open.
        Time.timeScale = isVisible ? 0f : 1f;
    }
}
