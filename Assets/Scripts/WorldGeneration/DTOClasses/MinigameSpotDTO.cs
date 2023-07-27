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
    public AreaLocationDTO location;
    public int index;
    public Position position;
    #endregion

    #region Constructors
    public MinigameSpotDTO() { }

    public MinigameSpotDTO(AreaLocationDTO location, int index, Position position)
    {
        this.location = location;
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
}
