using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Test
public class Leaderboard : MonoBehaviour
{

    public string playerName;
    public string rewardPoints;
    

    public Leaderboard(string playerName, string rewardPoints)
    {
        this.playerName = playerName;
        this.rewardPoints = rewardPoints;
    }

    public Leaderboard() { }

    /// <summary>
    ///     This function converts a json string to a <c>Achievement</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>Achievement</c> object containing the data</returns>
    public static Achievement CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Achievement>(jsonString);
    }
}

