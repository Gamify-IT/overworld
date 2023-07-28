using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a barrier spot
/// </summary>
public class BarrierSpotData
{
    #region Attribute
    private int worldIndex;
    private Optional<int> dungeonIndex;
    private Vector2 position;
    private BarrierType type;
    private int destinationWorldIndex;
    #endregion

    public BarrierSpotData(int worldIndex, Optional<int> dungeonIndex, Vector2 position, BarrierType type, int destinationWorldIndex)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.position = position;
        this.type = type;
        this.destinationWorldIndex = destinationWorldIndex;
    }

    /// <summary>
    ///     This function converts a <c>BarrierSpotDTO</c> object into a <c>BarrierSpotData</c> instance
    /// </summary>
    /// <param name="barrierSpotDTO">The <c>BarrierSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static BarrierSpotData ConvertDtoToData(BarrierSpotDTO barrierSpotDTO)
    {
        int worldIndex = barrierSpotDTO.location.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if (barrierSpotDTO.location.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(barrierSpotDTO.location.dungeonIndex);
        }
        Vector2 position = new Vector2(barrierSpotDTO.position.x, barrierSpotDTO.position.y);
        BarrierType type = (BarrierType)System.Enum.Parse(typeof(BarrierType), barrierSpotDTO.type);
        int destinationWorldIndex = barrierSpotDTO.destinationWorldIndex;
        BarrierSpotData data = new BarrierSpotData(worldIndex, dungeonIndex, position, type, destinationWorldIndex);
        return data;
    }

    #region GetterAndSetter
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

    public void SetBarrierType(BarrierType type)
    {
        this.type = type;
    }

    public BarrierType GetBarrierType()
    {
        return type;
    }

    public void SetDestinationAreaIndex(int destinationAreaIndex)
    {
        this.destinationWorldIndex = destinationAreaIndex;

    }

    public int GetDestinationAreaIndex()
    {
        return destinationWorldIndex;
    }
    #endregion
}
