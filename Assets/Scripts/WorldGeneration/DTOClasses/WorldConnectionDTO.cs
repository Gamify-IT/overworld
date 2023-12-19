using UnityEngine;
using System;

/// <summary>
///     This class is used to transfer a <c>WorldConnection</c> from and to the overworld backend
/// </summary>
[Serializable]
public class WorldConnectionDTO
{
    public SerializableVector2Int position;
    public int destinationWorld;
    public string connectionType;

    public WorldConnectionDTO() { }

    public WorldConnectionDTO(SerializableVector2Int position, int destinationWorld, string connectionType)
    {
        this.position = position;
        this.destinationWorld = destinationWorld;
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
        SerializableVector2Int position = new SerializableVector2Int(worldConnection.GetPosition().x, worldConnection.GetPosition().y);
        int destinationWorld = worldConnection.GetDestinationWorld();
        string connectionType = worldConnection.GetWorldConnectionType().ToString();

        WorldConnectionDTO worldConnectionDTO = new WorldConnectionDTO(position, destinationWorld, connectionType);
        return worldConnectionDTO;
    }
}
