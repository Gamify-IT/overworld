using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This class is used to retrieve <c>AchievementStatistic</c> data from Get Requests.
/// </summary>
[Serializable]
public class AchievementStatistic
{
    public string id;
    public Achievement achievement;
    public int progress;
    public bool completed;
    public List<IntTupel> interactedObjects;

     public AchievementStatistic(string id, Achievement achievement, int progress, bool completed, List<IntTupel> interactedObjects)
    {
        this.id = id;
        this.achievement = achievement;
        this.progress = progress;
        this.completed = completed;
        this.interactedObjects = interactedObjects;
        
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

    public static List<(int, int, int)> ConvertFromListIntTupel(List<IntTupel> list)
    {
        List<(int, int, int)> result = new List<(int, int, int)>();
        foreach (var item in list)
        {
            result.Add((item.first, item.second, item.third));
        }
        return result;
    }

    public static List<IntTupel> ConvertToListIntTupel(List<(int, int, int)> list)
    {
        List<IntTupel> result = new List<IntTupel>();
        foreach (var item in list)
        {
            result.Add(new IntTupel(item.Item1, item.Item2, item.Item3));
        }
        return result;
    }
}
