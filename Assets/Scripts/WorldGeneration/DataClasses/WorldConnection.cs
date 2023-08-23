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
    private int destinationWorld;
    private WorldConnectionType connectionType;

    public WorldConnection(Vector2Int position, int destinationWorld, WorldConnectionType connectionType)
    {
        this.position = position;
        this.destinationWorld = destinationWorld;
        this.connectionType = connectionType;
    }

    /// <summary>
    ///     This function converts a <c>WorldConnectionDTO</c> object into a <c>WorldConnection</c> instance
    /// </summary>
    /// <param name="worldConnectionDto">The <c>WorldConnectionDTO</c> object to convert</param>
    /// <returns></returns>
    public static WorldConnection ConvertDtoToData(WorldConnectionDTO worldConnectionDto)
    {
        Vector2Int position = new Vector2Int((int)worldConnectionDto.position.x, (int)worldConnectionDto.position.y);
        int destinationWorld = worldConnectionDto.destinationWorld;
        WorldConnectionType connectionType = (WorldConnectionType)System.Enum.Parse(typeof(WorldConnectionType), worldConnectionDto.connectionType);

        WorldConnection worldConnection = new WorldConnection(position, destinationWorld, connectionType);
        return worldConnection;
    }

    #region GetterAndSetter
    public Vector2Int GetPosition()
    {
        return position;
    }

    public int GetDestinationWorld()
    {
        return destinationWorld;
    }

    public WorldConnectionType GetWorldConnectionType()
    {
        return connectionType;
    }
    #endregion
}
