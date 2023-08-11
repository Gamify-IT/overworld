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

    /// <summary>
    ///     This function converts a <c>WorldConnectionDTO</c> object into a <c>WorldConnection</c> instance
    /// </summary>
    /// <param name="worldConnectionDto">The <c>WorldConnectionDTO</c> object to convert</param>
    /// <returns></returns>
    public static WorldConnection ConvertDtoToData(WorldConnectionDTO worldConnectionDto)
    {
        Vector2Int position = new Vector2Int((int)worldConnectionDto.position.x, (int)worldConnectionDto.position.y);
        WorldConnectionType connectionType = (WorldConnectionType)System.Enum.Parse(typeof(WorldConnectionType), worldConnectionDto.connectionType);

        WorldConnection worldConnection = new WorldConnection(position, connectionType);
        return worldConnection;
    }

    #region GetterAndSetter
    public Vector2Int GetPosition()
    {
        return position;
    }

    public WorldConnectionType GetWorldConnectionType()
    {
        return connectionType;
    }
    #endregion
}
