using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a barrier spot
/// </summary>
public class BarrierSpotData
{
    #region Attribute
    private AreaInformation area;
    private Vector2 position;
    private BarrierType type;
    private int destinationAreaIndex;
    private BarrierStyle barrierStyle;
    #endregion

    public BarrierSpotData(AreaInformation area, Vector2 position, BarrierType type, int destinationAreaIndex, BarrierStyle barrierStyle)
    {
        this.area = area;
        this.position = position;
        this.type = type;
        this.destinationAreaIndex = destinationAreaIndex;
        this.barrierStyle = barrierStyle;
    }

    /// <summary>
    ///     This function converts a <c>BarrierSpotDTO</c> object into a <c>BarrierSpotData</c> instance
    /// </summary>
    /// <param name="barrierSpotDTO">The <c>BarrierSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static BarrierSpotData ConvertDtoToData(BarrierSpotDTO barrierSpotDTO)
    {
        Optional<int> dungeonIndex = new Optional<int>();
        AreaInformation area = new AreaInformation(barrierSpotDTO.area.worldIndex, dungeonIndex);
        if (barrierSpotDTO.area.dungeonIndex != 0)
        {
            area.SetDungeonIndex(barrierSpotDTO.area.dungeonIndex);
        }
        Vector2 position = new Vector2(barrierSpotDTO.position.x, barrierSpotDTO.position.y);
        BarrierType type = (BarrierType)System.Enum.Parse(typeof(BarrierType), barrierSpotDTO.type);
        int destinationAreaIndex = barrierSpotDTO.destinationAreaIndex;
        BarrierStyle style = (BarrierStyle)System.Enum.Parse(typeof(BarrierStyle), barrierSpotDTO.barrierStyle);
        BarrierSpotData data = new BarrierSpotData(area, position, type, destinationAreaIndex, style);
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
        this.destinationAreaIndex = destinationAreaIndex;

    }

    public int GetDestinationAreaIndex()
    {
        return destinationAreaIndex;
    }

    public void SetBarrierStyle(BarrierStyle barrierStyle)
    {
        this.barrierStyle = barrierStyle;
    }

    public BarrierStyle GetBarrierStyle()
    {
        return barrierStyle;
    }
    #endregion
}
