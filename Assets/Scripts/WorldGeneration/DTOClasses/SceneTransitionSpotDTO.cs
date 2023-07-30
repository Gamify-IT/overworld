using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve scene transition spot data from Get Requests.
/// </summary>
[Serializable]
public class SceneTransitionSpotDTO
{
    #region Attribute
    public AreaLocationDTO location;
    public Position position;
    public Position size;
    public string sceneToLoad;
    public AreaLocationDTO areaToLoad;
    public Position playerPosition;
    public string facingDirection;
    #endregion

    #region Constructors
    public SceneTransitionSpotDTO() { }

    public SceneTransitionSpotDTO(AreaLocationDTO location, Position position, Position size, string sceneToLoad, AreaLocationDTO areaToLoad, Position playerPosition, string facingDirection)
    {
        this.location = location;
        this.position = position;
        this.size = size;
        this.sceneToLoad = sceneToLoad;
        this.areaToLoad = areaToLoad;
        this.playerPosition = playerPosition;
        this.facingDirection = facingDirection;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>SceneTransitionSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>SceneTransitionSpotDTO</c> object containing the data</returns>
    public static SceneTransitionSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<SceneTransitionSpotDTO>(jsonString);
    }
}
