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
    ///     IntTuple is a helper class for storing the unique identifier of objects in the game.
    ///     This method converts list that contains objects from IntTuple class into list of tuples.
    ///     Each tuple will contain worldId, dungeonId, and numberId as the unique identifier of objects in the game.
    /// </summary>
    /// <param name="listOfIntTupleObjects">The list of IntTuple objects to be converted.</param>
    /// <returns>A list of tuples where each tuple contains worldId, dungeonId, and numberId.</returns>
    public static List<(int, int, int)> ConvertFromIntTuple(List<IntTuple> listOfIntTupleObjects)
    {
        List<(int, int, int)> listOfTuples = new List<(int, int, int)>();
        foreach (var item in listOfIntTupleObjects)
        {
            listOfTuples.Add((item.worldId, item.dungeonId, item.numberId));
        }
        return listOfTuples;
    }

    /// <summary>
    ///     This method converts a list of tuples, where each tuple contains worldId, dungeonId, and numberId as the unique identifier of objects,
    ///     to list that contains objects from IntTuple class.
    /// </summary>
    /// <param name="listOfTuples">The list of tuples to be converted.</param>
    /// <returns>A list that contains objects from IntTuple class.</returns>
    public static List<IntTuple> ConvertToIntTuple(List<(int, int, int)> listOfTuples)
    {
        List<IntTuple> listOfIntTupleObjects = new List<IntTuple>();
        foreach (var (worldId, dungeonId, numberId) in listOfTuples)
        {
            listOfIntTupleObjects.Add(new IntTuple(worldId, dungeonId, numberId));
        }
        return listOfIntTupleObjects;
    }
}
