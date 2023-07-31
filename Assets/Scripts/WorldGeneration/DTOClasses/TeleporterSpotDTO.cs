using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve teleporter spot data from Get Requests.
/// </summary>
[Serializable]
public class TeleporterSpotDTO
{
    #region Attributes
    public AreaLocationDTO location;
    public int index;
    public Position position;
    public string name;
    #endregion

    #region Constructors
    public TeleporterSpotDTO() { }

    public TeleporterSpotDTO(AreaLocationDTO location, int index, Position position, string name)
    {
        this.location = location;
        this.index = index;
        this.position = position;
        this.name = name;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>TeleporterSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>TeleporterSpotDTO</c> object containing the data</returns>
    public static TeleporterSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TeleporterSpotDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>TeleporterSpotData</c> object into a <c>TeleporterSpotDTO</c> instance
    /// </summary>
    /// <param name="teleporterSpotData">The <c>TeleporterSpotData</c> object to convert</param>
    /// <returns></returns>
    public static TeleporterSpotDTO ConvertDataToDto(TeleporterSpotData teleporterSpotData)
    {
        AreaLocationDTO areaLocation = new AreaLocationDTO(teleporterSpotData.GetArea().GetWorldIndex(), 0);
        if (teleporterSpotData.GetArea().IsDungeon())
        {
            areaLocation.dungeonIndex = teleporterSpotData.GetArea().GetDungeonIndex();
        }
        int index = teleporterSpotData.GetIndex();
        Position position = new Position(teleporterSpotData.GetPosition().x, teleporterSpotData.GetPosition().y);
        string name = teleporterSpotData.GetName();
        TeleporterSpotDTO data = new TeleporterSpotDTO(areaLocation, index, position, name);
        return data;
    }
}
