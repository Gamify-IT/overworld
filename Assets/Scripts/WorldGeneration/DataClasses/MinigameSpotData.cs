using UnityEngine;

/// <summary>
///     This class contains all data to manage a minigame spot
/// </summary>
public class MinigameSpotData
{
    #region Attribute
    private AreaInformation area;
    private int index;
    private Vector2 position;   
    #endregion

    public MinigameSpotData(AreaInformation area, int index, Vector2 position)
    {
        this.area = area;
        this.index = index;
        this.position = position;
    }

    /// <summary>
    ///     This function converts a <c>MinigameSpotDTO</c> object into a <c>MinigameSpotData</c> instance
    /// </summary>
    /// <param name="minigameSpotDTO">The <c>MinigameSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static MinigameSpotData ConvertDtoToData(MinigameSpotDTO minigameSpotDTO)
    {
        AreaInformation area = new AreaInformation(minigameSpotDTO.area.worldIndex, new Optional<int>());
        if(minigameSpotDTO.area.dungeonIndex != 0)
        {
            area.SetDungeonIndex(minigameSpotDTO.area.dungeonIndex);
        }
        int index = minigameSpotDTO.index;
        Vector2 position = new Vector2(minigameSpotDTO.position.x, minigameSpotDTO.position.y);
        MinigameSpotData data = new MinigameSpotData(area, index, position);
        return data;
    }

    #region GetterAndSetter
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }

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
    #endregion
}
