using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class BookDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>BookDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>BookDTO</c> object containing the data</returns>
    public static BookDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BookDTO>(jsonString);
    }

    #region Attributes

    public string id;
    public AreaLocationDTO area;
    public int index;
    public string text;

    #endregion

    #region Constructors

    public BookDTO(string id, AreaLocationDTO area, int index, string text)
    {
        this.id = id;
        this.area = area;
        this.index = index;
        this.text = text;
    }

    public BookDTO()
    {
    }

    #endregion
}