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
    public int destinationWorldIndex;
    #endregion

    #region Constructors
    public BarrierSpotDTO() { }

    public BarrierSpotDTO(AreaLocationDTO location, Position position, string type, int destinationWorldIndex)
    {
        this.location = location;
        this.position = position;
        this.type = type;
        this.destinationWorldIndex = destinationWorldIndex;
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
}
