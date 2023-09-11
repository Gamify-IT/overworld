using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInformationData
{
    #region Attributes
    private AreaInformation area;
    private Vector2Int size;
    private Vector2Int offset;
    private List<WorldConnection> worldConnections;
    #endregion

    public AreaInformationData(AreaInformation area, Vector2Int size, Vector2Int offset, List<WorldConnection> worldConnections)
    {
        this.area = area;
        this.size = size;
        this.offset = offset;
        this.worldConnections = worldConnections;
    }

    /// <summary>
    ///     This function converts a <c>AreaInformationDTO</c> object into a <c>AreaInformationData</c> instance
    /// </summary>
    /// <param name="areaInformationDTO">The <c>AreaInformationDTO</c> object to convert</param>
    /// <returns></returns>
    public static AreaInformationData ConvertDtoToData(AreaInformationDTO areaInformationDTO)
    {
        int worldIndex = areaInformationDTO.area.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if(areaInformationDTO.area.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(areaInformationDTO.area.dungeonIndex);
        }
        AreaInformation area = new AreaInformation(worldIndex, dungeonIndex);

        Vector2Int size = new Vector2Int(areaInformationDTO.size.x, areaInformationDTO.size.y);
        Vector2Int offset = new Vector2Int(areaInformationDTO.offset.x, areaInformationDTO.offset.y);
        List<WorldConnection> worldConnections = new List<WorldConnection>();
        for(int i=0; i<areaInformationDTO.worldConnections.Length; i++)
        {
            WorldConnection worldConnection = WorldConnection.ConvertDtoToData(areaInformationDTO.worldConnections[i]);
            worldConnections.Add(worldConnection);
        }

        AreaInformationData data = new AreaInformationData(area, size, offset, worldConnections);
        return data;
    }

    #region GetterAndSetter
    public Vector2Int GetSize()
    {
        return size;
    }

    public Vector2Int GetOffset()
    {
        return offset;
    }

    public List<WorldConnection> GetWorldConnections()
    {
        return worldConnections;
    }
    #endregion
}
