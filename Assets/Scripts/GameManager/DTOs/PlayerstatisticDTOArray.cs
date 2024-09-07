using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerStatisticDTOArray
{
    #region Attributes

    public PlayerStatisticDTO[] playerStatisticDTOs;

    #endregion

    /// <summary>
    ///     This function converts a json string to a array of <c>PlayerStatisticDTO</c> objects.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A array of <c>PlayerStatisticDTO</c> objects containing the data</returns>
    public static PlayerStatisticDTOArray CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerStatisticDTOArray>(jsonString);
    }

    #region Constructors

    public PlayerStatisticDTOArray(PlayerStatisticDTO[] playerstatisticDTOs)
    {
        this.playerStatisticDTOs = playerStatisticDTOs;
    }

    public PlayerStatisticDTOArray()
    {
    }

    #endregion
}