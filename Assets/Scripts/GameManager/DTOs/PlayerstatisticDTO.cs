using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to retrieve data from Get Requests.
/// </summary>
[System.Serializable]
public class PlayerstatisticDTO
{
    #region Attributes
    public string id;
    public AreaLocationDTO[] unlockedAreas;
    public AreaLocationDTO[] unlockedDungeons;
    public AreaLocationDTO currentArea;
    public string userId;
    public string username;
    public int knowledge;
    #endregion

    #region Constructors
    public PlayerstatisticDTO(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, AreaLocationDTO currentArea, string userId, string username, int knowledge)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = unlockedDungeons;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        this.knowledge = knowledge;
    }

    public PlayerstatisticDTO() 
    {
        id = "";
        AreaLocationDTO unlockedArea = new AreaLocationDTO();
        unlockedAreas = new AreaLocationDTO[1];
        unlockedAreas[0] = unlockedArea;
        currentArea = unlockedArea;
        unlockedDungeons = null;
        userId = "";
        username = "";
        knowledge = 0;
    }
    #endregion

    /// <summary>
    /// This function converts a json string to a <c>PlayerstatisticDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>PlayerstatisticDTO</c> object containing the data</returns>
    public static PlayerstatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerstatisticDTO>(jsonString);
    }
}
