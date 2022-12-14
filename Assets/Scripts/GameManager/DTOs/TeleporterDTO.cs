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

    public string name;
    public AreaLocationDTO area;
    public int index;
    public Vector2 position;
    public bool isEnabled;

    #endregion

    #region Constructors

    public TeleporterDTO(string name, AreaLocationDTO area, int index, Vector2 position, bool isEnabled)
    {
        this.name = name;
        this.area = area;
        this.index = index;
        this.position = position;
        this.isEnabled = isEnabled;
    }

    public TeleporterDTO()
    {
    }

    #endregion
}