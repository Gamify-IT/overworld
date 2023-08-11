using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve world map data from Get Requests.
/// </summary>
[Serializable]
public class WorldConnectionDTO
{
    public Position position;
    public string connectionType;

    public WorldConnectionDTO() { }

    public WorldConnectionDTO(Position position, string connectionType)
    {
        this.position = position;
        this.connectionType = connectionType;
    }

    /// <summary>
    ///     This function converts a json string to a <c>WorldConnectionDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>WorldConnectionDTO</c> object containing the data</returns>
    public static WorldConnectionDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<WorldConnectionDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>WorldConnection</c> object into a <c>WorldConnectionDTO</c> instance
    /// </summary>
    /// <param name="worldConnection">The <c>WorldConnection</c> object to convert</param>
    /// <returns></returns>
    public static WorldConnectionDTO ConvertDataToDto(WorldConnection worldConnection)
    {
        Position position = new Position(worldConnection.GetPosition().x, worldConnection.GetPosition().y);
        string connectionType = worldConnection.GetWorldConnectionType().ToString();

        WorldConnectionDTO worldConnectionDTO = new WorldConnectionDTO(position, connectionType);
        return worldConnectionDTO;
    }
}
