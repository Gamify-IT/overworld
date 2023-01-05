using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class TeleporterDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>TeleporterDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>BookDTO</c> object containing the data</returns>
    public static TeleporterDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TeleporterDTO>(jsonString);
    }

    #region Attributes

    public string id;
    public AreaLocationDTO area;
    public int index;

    #endregion

    #region Constructors

    public TeleporterDTO(string id, AreaLocationDTO area, int index)
    {
        this.id = id;
        this.area = area;
        this.index = index;
    }

    public TeleporterDTO()
    {
    }

    #endregion
}