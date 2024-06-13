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
    public TeleporterDTO[] unlockedTeleporters;
    public AreaLocationDTO currentArea;
    public string userId;
    public string username;
    public int knowledge;
    public int rewards;

    #endregion

    #region Constructors

    public PlayerstatisticDTO(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, TeleporterDTO[] unlockedTeleporters,
        AreaLocationDTO currentArea, string userId, string username, int knowledge, int rewards)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = unlockedDungeons;
        this.unlockedTeleporters = unlockedTeleporters;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        this.knowledge = knowledge;
        this.rewards = rewards;
    }

    public PlayerstatisticDTO()
    {
        id = "";
        AreaLocationDTO unlockedWorld = new AreaLocationDTO();
        unlockedWorld.worldIndex = 1;
        unlockedWorld.dungeonIndex = 0;
        unlockedAreas = new AreaLocationDTO[1];
        unlockedAreas[0] = unlockedWorld;
        currentArea = unlockedWorld;
        unlockedDungeons = null;
        unlockedTeleporters = new TeleporterDTO[0];
        userId = "";
        username = "";
        knowledge = 0;
        rewards = 0;
    }

    #endregion
}