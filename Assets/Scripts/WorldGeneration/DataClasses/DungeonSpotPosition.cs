using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to transfer position and style of dungeon spot position
public class DungeonSpotPosition
{
    private Vector2Int position;
    private DungeonStyle style;

    public DungeonSpotPosition(Vector2Int position, DungeonStyle style)
    {
        this.position = position;
        this.style = style;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public DungeonStyle GetStyle()
    {
        return style;
    }
}
