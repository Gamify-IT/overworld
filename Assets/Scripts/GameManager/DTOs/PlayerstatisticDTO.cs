using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerstatisticDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>PlayerstatisticDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>PlayerstatisticDTO</c> object containing the data</returns>
    public static PlayerstatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerstatisticDTO>(jsonString);
    }

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

    public PlayerstatisticDTO(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons,
        AreaLocationDTO currentArea, string userId, string username, int knowledge)
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
        AreaLocationDTO unlockedWorld = new AreaLocationDTO();
        unlockedWorld.worldIndex = 1;
        unlockedWorld.dungeonIndex = 0;
        AreaLocationDTO unlockedDungeon = new AreaLocationDTO();
        unlockedDungeon.worldIndex = 1;
        unlockedDungeon.dungeonIndex = 1;
        unlockedAreas = new AreaLocationDTO[2];
        unlockedAreas[0] = unlockedWorld;
        unlockedAreas[1] = unlockedDungeon;
        currentArea = unlockedWorld;
        unlockedDungeons = null;
        userId = "";
        username = "";
        knowledge = 0;
    }

    #endregion
}