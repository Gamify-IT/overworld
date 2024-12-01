using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProgressBar : MonoBehaviour
{
    #region Attributes
    [SerializeField] private Image slider;
    [SerializeField] private TextMeshProUGUI unlockedAreaText;
    [SerializeField] private GameObject taskScreen;
    #endregion

    #region Singleton

    public static ProgressBar Instance { get; private set; }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>Instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        if (GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            taskScreen.SetActive(true);
        }
    }

    /// <summary>
    /// This function sets the progress bar at the given value.
    /// </summary>
    /// <param name="progress">The progress in percent (must be between 0 and 1)</param>
    public void setProgress(float progress)
    {
        if(0f <= progress && progress <= 1f)
        {
            slider.fillAmount = progress;
        }
    }

    /// <summary>
    /// This function sets the unlocked area for a world and updates achievements if new world is unlocked
    /// </summary>
    /// <param name="worldIndex">The index of the unlocked world</param>
    public void setUnlockedArea(int worldIndex)
    {
        // display no world index for tutorial mode
        if (GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            unlockedAreaText.text = "TUTORIAL";
            unlockedAreaText.fontSize = 5;
            return;
        }

        unlockedAreaText.text = worldIndex.ToString();

        if (worldIndex == 1 || worldIndex == 2)
        {
            GameManager.Instance.UpdateAchievement(AchievementTitle.LEVEL_UP, worldIndex, null);
        }
        if (worldIndex != 1)
        {
            GameManager.Instance.UpdateAchievement((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"OPENER_WORLD_{worldIndex}"), 1, null);
        }
    }

    /// <summary>
    /// This function sets the unlocked area for a dungeon and updates achievements if new dungeon is unlocked
    /// </summary>
    /// <param name="worldIndex">The index of the world the unlocked dungeon is in</param>
    /// <param name="dungeonIndex">The index of the unlocked dungeon</param>
    public void setUnlockedArea(int worldIndex, int dungeonIndex)
    {
        unlockedAreaText.text = worldIndex + "-" + dungeonIndex;
        GameManager.Instance.UpdateAchievement((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"MINER_WORLD_{worldIndex}"), 1, null);
        
    }

    #region tutorial
    /// <summary>
    ///     Makes the task screen below the minimap in tutorial mode visible
    /// </summary>
    public void SetupTutorial()
    {
        taskScreen.SetActive(true);
    }

    /// <summary>
    ///     Displays the given text on the task screen
    /// </summary>
    /// <param name="text">description of the task</param>
    public void DisplayTaskOnScreen(string text)
    {
        taskScreen.transform.GetChild(1).GetComponent<TMP_Text>().text = text;
    }
    #endregion
}
