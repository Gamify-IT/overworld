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
    public List<IntTuple> interactedObjects;

     public AchievementStatistic(string id, Achievement achievement, int progress, bool completed, List<IntTuple> interactedObjects)
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

    /// <summary>
    ///     This method converts a list of IntTuple objects into a list of tuples containing three integers.
    /// </summary>
    /// <param name="list">The list of IntTuple objects to be converted</param>
    /// <returns>A list of tuples where each tuple contains worldId, dungeonId, and numberId.</returns>
    public static List<(int, int, int)> ConvertFromListIntTuple(List<IntTuple> list)
    {
        List<(int, int, int)> result = new List<(int, int, int)>();
        foreach (var item in list)
        {
            result.Add((item.worldId, item.dungeonId, item.numberId));
        }
        return result;
    }

    /// <summary>
    ///     This method converts a list of tuples with three integers into a list of IntTuple objects.
    /// </summary>
    /// <param name="list">The list of tuples to be converted</param>
    /// <returns>A list of IntTuple objects</returns>
    public static List<IntTuple> ConvertToListIntTuple(List<(int, int, int)> list)
    {
        List<IntTuple> result = new List<IntTuple>();
        foreach (var item in list)
        {
            result.Add(new IntTuple(item.Item1, item.Item2, item.Item3));
        }
        return result;
    }
}
