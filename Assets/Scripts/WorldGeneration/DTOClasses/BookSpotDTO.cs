using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve minigame spot data from Get Requests.
/// </summary>
[Serializable]
public class BookSpotDTO
{
    #region Attributes
    public AreaLocationDTO location;
    public int index;
    public Position position;
    public string name;
    #endregion

    #region Constructors
    public BookSpotDTO() { }

    public BookSpotDTO(AreaLocationDTO location, int index, Position position, string name)
    {
        this.location = location;
        this.index = index;
        this.position = position;
        this.name = name;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>BookSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>BookSpotDTO</c> object containing the data</returns>
    public static NpcSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NpcSpotDTO>(jsonString);
    }
}
