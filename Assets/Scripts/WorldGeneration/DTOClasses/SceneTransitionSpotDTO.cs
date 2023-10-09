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
    public AreaLocationDTO area;
    public Position position;
    public Position size;
    public AreaLocationDTO areaToLoad;
    public string facingDirection;
    #endregion

    #region Constructors
    public SceneTransitionSpotDTO() { }

    public SceneTransitionSpotDTO(AreaLocationDTO area, Position position, Position size, AreaLocationDTO areaToLoad, string facingDirection)
    {
        this.area = area;
        this.position = position;
        this.size = size;
        this.areaToLoad = areaToLoad;
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

    /// <summary>
    ///     This function converts a <c>SceneTransitionSpotData</c> object into a <c>SceneTransitionSpotDTO</c> instance
    /// </summary>
    /// <param name="sceneTransitionSpotData">The <c>SceneTransitionSpotData</c> object to convert</param>
    /// <returns></returns>
    public static SceneTransitionSpotDTO ConvertDataToDto(SceneTransitionSpotData sceneTransitionSpotData)
    {
        AreaLocationDTO areaLocation = new AreaLocationDTO(sceneTransitionSpotData.GetArea().GetWorldIndex(), 0);
        if (sceneTransitionSpotData.GetArea().IsDungeon())
        {
            areaLocation.dungeonIndex = sceneTransitionSpotData.GetArea().GetDungeonIndex();
        }
        Position position = new Position(sceneTransitionSpotData.GetPosition().x, sceneTransitionSpotData.GetPosition().y);
        Position size = new Position(sceneTransitionSpotData.GetSize().x, sceneTransitionSpotData.GetSize().y);
        AreaLocationDTO areaToLoad = new AreaLocationDTO(sceneTransitionSpotData.GetAreaToLoad().GetWorldIndex(), 0);
        if(sceneTransitionSpotData.GetAreaToLoad().IsDungeon())
        {
            areaToLoad.dungeonIndex = sceneTransitionSpotData.GetAreaToLoad().GetDungeonIndex();
        }
        string facingDirection = sceneTransitionSpotData.GetFacingDirection().ToString();
        SceneTransitionSpotDTO data = new SceneTransitionSpotDTO(areaLocation, position, size, areaToLoad, facingDirection);
        return data;
    }
}
