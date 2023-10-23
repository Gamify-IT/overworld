using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve minigame spot data from Get Requests.
/// </summary>
[Serializable]
public class MinigameSpotDTO
{
    #region Attributes
    public AreaLocationDTO area;
    public int index;
    public Position position;
    #endregion

    #region Constructors
    public MinigameSpotDTO() { }

    public MinigameSpotDTO(AreaLocationDTO area, int index, Position position)
    {
        this.area = area;
        this.index = index;
        this.position = position;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>MinigameSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>MinigameSpotDTO</c> object containing the data</returns>
    public static MinigameSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MinigameSpotDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>MinigameSpotData</c> object into a <c>MinigameSpotDTO</c> instance
    /// </summary>
    /// <param name="minigameSpotData">The <c>MinigameSpotData</c> object to convert</param>
    /// <returns></returns>
    public static MinigameSpotDTO ConvertDataToDto(MinigameSpotData minigameSpotData)
    {
        AreaLocationDTO areaLocation = new AreaLocationDTO(minigameSpotData.GetArea().GetWorldIndex(), 0);
        if(minigameSpotData.GetArea().IsDungeon())
        {
            areaLocation.dungeonIndex = minigameSpotData.GetArea().GetDungeonIndex();
        }
        int index = minigameSpotData.GetIndex();
        Position position = new Position(minigameSpotData.GetPosition().x, minigameSpotData.GetPosition().y);
        MinigameSpotDTO data = new MinigameSpotDTO(areaLocation, index, position);
        return data;
    }
}
