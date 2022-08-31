using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class NPCDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>NPCDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>NPCDTO</c> object containing the data</returns>
    public static NPCDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NPCDTO>(jsonString);
    }

    #region Attributes

    public string id;
    public AreaLocationDTO area;
    public int index;
    public List<string> text;

    #endregion

    #region Constructors

    public NPCDTO(string id, AreaLocationDTO area, int index, List<string> text)
    {
        this.id = id;
        this.area = area;
        this.index = index;
        this.text = text;
    }

    public NPCDTO()
    {
    }

    #endregion
}