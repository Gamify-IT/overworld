using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaInformationDTO
{
    public AreaLocationDTO area;
    public SerializableVector2Int size;
    public SerializableVector2Int offset;
    public WorldConnectionDTO[] worldConnections;

    public AreaInformationDTO() { }

    public AreaInformationDTO(AreaLocationDTO area, SerializableVector2Int size, SerializableVector2Int offset, WorldConnectionDTO[] worldConnections)
    {
        this.area = area;
        this.size = size;
        this.offset = offset;
        this.worldConnections = worldConnections;
    }

    /// <summary>
    ///     This function converts a json string to a <c>AreaInformationDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>AreaInformationDTO</c> object containing the data</returns>
    public static AreaInformationDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AreaInformationDTO>(jsonString);
    }
}
