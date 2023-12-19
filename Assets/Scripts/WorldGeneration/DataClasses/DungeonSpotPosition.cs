using UnityEngine;

/// <summary>
///     This class contains all information for a position of a dungeon spot and is used to transfer this data from the <c>ObjectPositionGenerator</c> to the <c>ObjectGenerator</c>
/// </summary>
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
