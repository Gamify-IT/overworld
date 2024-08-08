using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerstatisticDTOArray
{
    #region Attributes

    public PlayerstatisticDTO[] playerstatisticDTOs;

    #endregion

    /// <summary>
    ///     This function converts a json string to a array of <c>PlayerstatisticDTO</c> objects.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A array of <c>PlayerstatisticDTO</c> objects containing the data</returns>
    public static PlayerstatisticDTOArray CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerstatisticDTOArray>(jsonString);
    }

    #region Constructors

    public PlayerstatisticDTOArray(PlayerstatisticDTO[] playerstatisticDTOs)
    {
        this.playerstatisticDTOs = playerstatisticDTOs;
    }

    public PlayerstatisticDTOArray()
    {
    }

    #endregion
}