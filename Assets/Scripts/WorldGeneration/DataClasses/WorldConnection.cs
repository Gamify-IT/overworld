using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldConnectionType
{
    ENTRY,
    EXIT
}

public class WorldConnection
{
    private Vector2Int position;
    private WorldConnectionType connectionType;

    public WorldConnection(Vector2Int position, WorldConnectionType connectionType)
    {
        this.position = position;
        this.connectionType = connectionType;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public WorldConnectionType GetWorldConnectionType()
    {
        return connectionType;
    }
}
