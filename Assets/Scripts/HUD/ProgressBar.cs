using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    #region Attributes
    [SerializeField] private Image slider;
    [SerializeField] private TextMeshProUGUI unlockedAreaText;
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
    /// This function sets the unlocked area for a world
    /// </summary>
    /// <param name="worldIndex">The index of the unlocked world</param>
    public void setUnlockedArea(int worldIndex)
    {
        unlockedAreaText.text = worldIndex.ToString();
        if (worldIndex == 2)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.OPENER_WORLD_2, 1);
        }
        if (worldIndex == 3)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.OPENER_WORLD_3, 1);
        }
        if (worldIndex == 4)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.OPENER_WORLD_4, 1);
        }
    }

    /// <summary>
    /// This function sets the unlocked area for a dungeon
    /// </summary>
    /// <param name="worldIndex">The index of the world the unlocked dungeon is in</param>
    /// <param name="dungeonIndex">The index of the unlocked dungeon</param>
    public void setUnlockedArea(int worldIndex, int dungeonIndex)
    {
        unlockedAreaText.text = worldIndex + "-" + dungeonIndex;
        if (worldIndex == 1)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINER_WORLD_1, 1);
            Debug.Log("розблоковано");
        }
        if (worldIndex == 2)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINER_WORLD_2, 1);
        }
        if (worldIndex == 3)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINER_WORLD_3, 1);
        }
        if (worldIndex == 4)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.MINER_WORLD_4, 1);
        }
    }
}
