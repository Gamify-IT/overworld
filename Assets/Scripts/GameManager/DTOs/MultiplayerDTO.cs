using System;
using UnityEngine;

public class MultiplayerDTO
{
    public Vector3 position;
    public AreaLocationDTO areaLocation;
    public int character;

    public MultiplayerDTO (Vector3 position, AreaLocationDTO areaLocation, int character)
    {
        this.position = position;
        this.areaLocation = areaLocation;   
        this.character = character;
    }

    /// <summary>
    ///     This function converts a json string to a <c>MultiplayerDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>MultiplayerDTO</c> object containing the data</returns>
    public static MultiplayerDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MultiplayerDTO>(jsonString);
    }
}