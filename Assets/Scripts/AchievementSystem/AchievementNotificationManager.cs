using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementNotificationManager : MonoBehaviour
{
    public static AchievementNotificationManager Instance { get; private set; }

    [SerializeField] private GameObject achievementPrefab;

    private Queue<AchievementData> achievements;

    /// <summary>
    ///     This function adds an achievement and destroys it after five seconds
    /// </summary>
    /// <param name="achievement"></param>
    public void AddAchievement(AchievementData achievement)
    {
        achievements.Enqueue(achievement);
        GameObject achievementObject = InstantiateAchievement(achievement);
        StartCoroutine(DeleteAchievement(achievementObject));
    }

    /// <summary>
    ///     This function creates an <c>GameObject</c> of the given achievement which is displayed
    /// </summary>
    /// <param name="achievement">The achievement to display</param>
    /// <returns>The created <c>GameObject</c></returns>
    private GameObject InstantiateAchievement(AchievementData achievement)
    {
        GameObject achievementObject = Instantiate(achievementPrefab, this.transform, false);
        AchievementUIElement achievementUIElement = achievementObject.GetComponent<AchievementUIElement>();
        if (achievementUIElement != null)
        {
            string title = achievement.GetTitle();
            string description = achievement.GetDescription();
            Sprite image = achievement.GetImage();
            int progress = achievement.GetProgress();
            int amountRequired = achievement.GetAmountRequired();
            bool completed = achievement.IsCompleted();
            achievementUIElement.Setup(title, description, image, progress, amountRequired, completed);
        }
        return achievementObject;
    }

    /// <summary>
    ///     This Coroutine destroys the achievement after 5 seconds, and if no achievements are left closes the <c>AchievementNotificationManager</c>
    /// </summary>
    /// <param name="achievement">The achievement to destroy</param>
    private IEnumerator DeleteAchievement(GameObject achievement)
    {
        yield return new WaitForSeconds(5);
        Destroy(achievement);
        achievements.Dequeue();
        if(achievements.Count == 0)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            achievements = new Queue<AchievementData>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
