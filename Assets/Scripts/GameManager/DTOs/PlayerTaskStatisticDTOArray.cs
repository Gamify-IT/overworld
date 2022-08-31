using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerTaskStatisticDTOArray
{
    #region Attributes

    public PlayerTaskStatisticDTO[] playerTaskStatisticDTOs;

    #endregion

    /// <summary>
    ///     This function converts a json string to a array of<c>PlayerTaskStatisticDTO</c> objects.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A array of <c>PlayerTaskStatisticDTO</c> objects containing the data</returns>
    public static PlayerTaskStatisticDTOArray CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerTaskStatisticDTOArray>(jsonString);
    }

    #region Constructors

    public PlayerTaskStatisticDTOArray(PlayerTaskStatisticDTO[] playerTaskStatisticDTOs)
    {
        this.playerTaskStatisticDTOs = playerTaskStatisticDTOs;
    }

    public PlayerTaskStatisticDTOArray()
    {
    }

    #endregion
}