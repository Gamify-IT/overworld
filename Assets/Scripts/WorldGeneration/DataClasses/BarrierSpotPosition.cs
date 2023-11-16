using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to transfer position and style of barrier spot position
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
