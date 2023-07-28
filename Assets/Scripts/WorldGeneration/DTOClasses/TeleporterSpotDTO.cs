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
}
