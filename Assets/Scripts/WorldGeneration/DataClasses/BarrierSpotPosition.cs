using UnityEngine;

/// <summary>
///     This class contains all information for a position of a barrier spot and is used to transfer this data from the <c>ObjectPositionGenerator</c> to the <c>ObjectGenerator</c>
/// </summary>
public class BarrierSpotPosition
{
    private Vector2Int position;
    private BarrierStyle style;
    private int destinationWorld;

    public BarrierSpotPosition(Vector2Int position, BarrierStyle style, int destinationWorld)
    {
        this.position = position;
        this.style = style;
        this.destinationWorld = destinationWorld;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public BarrierStyle GetStyle()
    {
        return style;
    }

    public int GetDestinationWorld()
    {
        return destinationWorld;
    }
}
