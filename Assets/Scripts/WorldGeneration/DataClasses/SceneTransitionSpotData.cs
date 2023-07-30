using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a scene transition spot
/// </summary>
public class SceneTransitionSpotData
{
    #region Attribute
    private AreaInformation area;
    private Vector2 position;
    private Vector2 size;
    private string sceneToLoad;
    private AreaInformation areaToLoad;
    private Vector2 playerPosition;
    private FacingDirection facingDirection;
    #endregion

    public SceneTransitionSpotData(AreaInformation area, Vector2 position, Vector2 size, string sceneToLoad, AreaInformation areaToLoad, Vector2 playerPosition, FacingDirection facingDirection)
    {
        this.area = area;
        this.position = position;
        this.size = size;
        this.sceneToLoad = sceneToLoad;
        this.areaToLoad = areaToLoad;
        this.playerPosition = playerPosition;
        this.facingDirection = facingDirection;
    }

    /// <summary>
    ///     This function converts a <c>SceneTransitionSpotDTO</c> object into a <c>SceneTransitionSpotData</c> instance
    /// </summary>
    /// <param name="sceneTransitionSpotDTO">The <c>SceneTransitionSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static SceneTransitionSpotData ConvertDtoToData(SceneTransitionSpotDTO sceneTransitionSpotDTO)
    {
        AreaInformation area = new AreaInformation(sceneTransitionSpotDTO.location.worldIndex, new Optional<int>());
        if (sceneTransitionSpotDTO.location.dungeonIndex != 0)
        {
            area.SetDungeonIndex(sceneTransitionSpotDTO.location.dungeonIndex);
        }        
        Vector2 position = new Vector2(sceneTransitionSpotDTO.position.x, sceneTransitionSpotDTO.position.y);
        Vector2 size = new Vector2(sceneTransitionSpotDTO.size.x, sceneTransitionSpotDTO.size.y);
        string sceneToLoad = sceneTransitionSpotDTO.sceneToLoad;
        AreaInformation areaToLoad = new AreaInformation(sceneTransitionSpotDTO.location.worldIndex, new Optional<int>());
        if (sceneTransitionSpotDTO.areaToLoad.dungeonIndex != 0)
        {
            areaToLoad.SetDungeonIndex(sceneTransitionSpotDTO.location.dungeonIndex);
        }
        Vector2 playerPosition = new Vector2(sceneTransitionSpotDTO.playerPosition.x, sceneTransitionSpotDTO.playerPosition.y);
        FacingDirection facingDirection = (FacingDirection) System.Enum.Parse(typeof(FacingDirection), sceneTransitionSpotDTO.facingDirection);
        SceneTransitionSpotData data = new SceneTransitionSpotData(area, position, size, sceneToLoad, areaToLoad, playerPosition, facingDirection);
        return data;
    }

    #region GetterAndSetter
    public void SetArea(AreaInformation area)
    {
        this.area = area;
    }

    public AreaInformation GetArea()
    {
        return area;
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public void SetSize(Vector2 size)
    {
        this.size = size;
    }

    public Vector2 GetSize()
    {
        return size;
    }

    public void SetSceneToLoad(string sceneToLoad)
    {
        this.sceneToLoad = sceneToLoad;
    }

    public string GetSceneToLoad()
    {
        return sceneToLoad;
    }

    public void SetAreaToLoad(AreaInformation areaToLoad)
    {
        this.areaToLoad = areaToLoad;
    }

    public AreaInformation GetAreaToLoad()
    {
        return areaToLoad;
    }

    public void SetPlayerPosition(Vector2 playerPosition)
    {
        this.playerPosition = playerPosition;
    }

    public Vector2 GetPlayerPosition()
    {
        return playerPosition;
    }

    public void SetFacingDirection(FacingDirection facingDirection)
    {
        this.facingDirection = facingDirection;
    }

    public FacingDirection GetFacingDirection()
    {
        return facingDirection;
    }
    #endregion
}
