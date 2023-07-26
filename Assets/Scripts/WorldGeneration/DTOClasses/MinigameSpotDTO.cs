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
    public int worldIndex;
    public int dungeonIndex;
    public int index;
    public float positionX;
    public float positionY;
    #endregion

    #region Constructors
    public MinigameSpotDTO() { }

    public MinigameSpotDTO(int worldIndex, int dungeonIndex, int index, float positionX, float positionY)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.index = index;
        this.positionX = positionX;
        this.positionY = positionY;
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
