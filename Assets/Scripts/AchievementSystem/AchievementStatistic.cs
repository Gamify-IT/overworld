using UnityEngine;

/// <summary>
///     This class is used to retrieve <c>AchievementStatistic</c> data from Get Requests.
/// </summary>
public class AchievementStatistic
{
    public string id;
    public Achievement achievement;
    public int progress;
    public bool completed;

    public AchievementStatistic(string id, Achievement achievement, int progress, bool completed)
    {
        this.id = id;
        this.achievement = achievement;
        this.progress = progress;
        this.completed = completed;
    }

    public AchievementStatistic() { }

    /// <summary>
    ///     This function converts a json string to a <c>AchievementStatistic</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>AchievementStatistic</c> object containing the data</returns>
    public static AchievementStatistic CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AchievementStatistic>(jsonString);
    }
}
