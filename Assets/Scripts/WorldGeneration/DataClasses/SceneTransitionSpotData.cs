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
    private AreaInformation areaToLoad;
    private FacingDirection facingDirection;
    private DungeonStyle style;
    #endregion

    public SceneTransitionSpotData(AreaInformation area, Vector2 position, Vector2 size, AreaInformation areaToLoad, FacingDirection facingDirection, DungeonStyle style)
    {
        this.area = area;
        this.position = position;
        this.size = size;
        this.areaToLoad = areaToLoad;
        this.facingDirection = facingDirection;
        this.style = style;
    }

    /// <summary>
    ///     This function converts a <c>SceneTransitionSpotDTO</c> object into a <c>SceneTransitionSpotData</c> instance
    /// </summary>
    /// <param name="sceneTransitionSpotDTO">The <c>SceneTransitionSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static SceneTransitionSpotData ConvertDtoToData(SceneTransitionSpotDTO sceneTransitionSpotDTO)
    {
        AreaInformation area = new AreaInformation(sceneTransitionSpotDTO.area.worldIndex, new Optional<int>());
        if (sceneTransitionSpotDTO.area.dungeonIndex != 0)
        {
            area.SetDungeonIndex(sceneTransitionSpotDTO.area.dungeonIndex);
        }        
        Vector2 position = new Vector2(sceneTransitionSpotDTO.position.x, sceneTransitionSpotDTO.position.y);
        Vector2 size = new Vector2(sceneTransitionSpotDTO.size.x, sceneTransitionSpotDTO.size.y);
        AreaInformation areaToLoad = new AreaInformation(sceneTransitionSpotDTO.area.worldIndex, new Optional<int>());
        if (sceneTransitionSpotDTO.areaToLoad.dungeonIndex != 0)
        {
            areaToLoad.SetDungeonIndex(sceneTransitionSpotDTO.areaToLoad.dungeonIndex);
        }
        FacingDirection facingDirection = (FacingDirection) System.Enum.Parse(typeof(FacingDirection), sceneTransitionSpotDTO.facingDirection);
        DungeonStyle style = (DungeonStyle)System.Enum.Parse(typeof(DungeonStyle), sceneTransitionSpotDTO.style);
        SceneTransitionSpotData data = new SceneTransitionSpotData(area, position, size, areaToLoad, facingDirection, style);
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

    public void SetAreaToLoad(AreaInformation areaToLoad)
    {
        this.areaToLoad = areaToLoad;
    }

    public AreaInformation GetAreaToLoad()
    {
        return areaToLoad;
    }

    public void SetFacingDirection(FacingDirection facingDirection)
    {
        this.facingDirection = facingDirection;
    }

    public FacingDirection GetFacingDirection()
    {
        return facingDirection;
    }

    public void SetStyle(DungeonStyle style)
    {
        this.style = style;
    }

    public DungeonStyle GetStyle()
    {
        return style;
    }
    #endregion
}
