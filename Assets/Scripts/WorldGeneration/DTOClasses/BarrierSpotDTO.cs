using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve barrier spot data from Get Requests.
/// </summary>
[Serializable]
public class BarrierSpotDTO
{
    #region Attribute
    public AreaLocationDTO location;
    public Position position;
    public string type;
    public int destinationAreaIndex;
    #endregion

    #region Constructors
    public BarrierSpotDTO() { }

    public BarrierSpotDTO(AreaLocationDTO location, Position position, string type, int destinationAreaIndex)
    {
        this.location = location;
        this.position = position;
        this.type = type;
        this.destinationAreaIndex = destinationAreaIndex;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>BarrierSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>BarrierSpotDTO</c> object containing the data</returns>
    public static BarrierSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BarrierSpotDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>BarrierSpotData</c> object into a <c>BarrierSpotDTO</c> instance
    /// </summary>
    /// <param name="barrierSpotData">The <c>BarrierSpotData</c> object to convert</param>
    /// <returns></returns>
    public static BarrierSpotDTO ConvertDataToDto(BarrierSpotData barrierSpotData)
    {
        AreaLocationDTO areaLocation = new AreaLocationDTO(barrierSpotData.GetArea().GetWorldIndex(), 0);
        if (barrierSpotData.GetArea().IsDungeon())
        {
            areaLocation.dungeonIndex = barrierSpotData.GetArea().GetDungeonIndex();
        }
        Position position = new Position(barrierSpotData.GetPosition().x, barrierSpotData.GetPosition().y);
        string type = barrierSpotData.GetBarrierType().ToString();
        int destinationWorldIndex = barrierSpotData.GetDestinationAreaIndex();
        BarrierSpotDTO data = new BarrierSpotDTO(areaLocation, position, type, destinationWorldIndex);
        return data;
    }
}
