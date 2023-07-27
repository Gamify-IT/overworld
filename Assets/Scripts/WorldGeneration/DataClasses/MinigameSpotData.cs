using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a minigame spot
/// </summary>
public class MinigameSpotData
{
    #region Attribute
    private int worldIndex;
    private Optional<int> dungeonIndex;
    private int index;
    private Vector2 position;   
    #endregion

    public MinigameSpotData(int worldIndex, Optional<int> dungeonIndex, int index, Vector2 position)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
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
        int worldIndex = minigameSpotDTO.location.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if(minigameSpotDTO.location.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(minigameSpotDTO.location.dungeonIndex);
        }
        int index = minigameSpotDTO.index;
        Vector2 position = new Vector2(minigameSpotDTO.position.x, minigameSpotDTO.position.y);
        MinigameSpotData data = new MinigameSpotData(worldIndex, dungeonIndex, index, position);
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

    public void SetWorldIndex(int worldIndex)
    {
        this.worldIndex = worldIndex;
    }

    public int GetWorldIndex()
    {
        return worldIndex;
    }

    public void SetDungeonIndex(int dungeonIndex)
    {
        this.dungeonIndex.SetValue(dungeonIndex);
    }

    public bool IsDungeon()
    {
        return dungeonIndex.IsPresent();
    }

    public int GetDungeonIndex()
    {
        return dungeonIndex.Value();
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
