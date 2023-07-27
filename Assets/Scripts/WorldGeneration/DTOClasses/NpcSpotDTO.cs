using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve minigame spot data from Get Requests.
/// </summary>
[Serializable]
public class NpcSpotDTO
{
    #region Attributes
    public AreaLocationDTO location;
    public int index;
    public Position position;
    public string name;
    public string spriteName;
    public string iconName;
    #endregion

    #region Constructors
    public NpcSpotDTO() { }

    public NpcSpotDTO(AreaLocationDTO location, int index, Position position, string name, string spriteName, string iconName)
    {
        this.location = location;
        this.index = index;
        this.position = position;
        this.name = name;
        this.spriteName = spriteName;
        this.iconName = iconName;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>NpcSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>NpcSpotDTO</c> object containing the data</returns>
    public static NpcSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NpcSpotDTO>(jsonString);
    }
}
