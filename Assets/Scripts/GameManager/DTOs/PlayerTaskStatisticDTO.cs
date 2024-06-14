using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerTaskStatisticDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>PlayerTaskStatisticDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>PlayerTaskStatisticDTO</c> object containing the data</returns>
    public static PlayerTaskStatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerTaskStatisticDTO>(jsonString);
    }

    #region Attributes

    public string id;
    public int highscore;
    public bool completed;
    public MinigameTaskDTO minigameTask;


    public int rewards;
    public int totalRewards;

    #endregion

    #region Constructors

    public PlayerTaskStatisticDTO(string id, int highscore, bool completed, MinigameTaskDTO minigameTask, int rewards)
    {
        this.id = id;
        this.highscore = highscore;
        this.completed = completed;
        this.minigameTask = minigameTask;
        this.rewards = rewards;
    }

    public PlayerTaskStatisticDTO()
    {
    }

    #endregion
}