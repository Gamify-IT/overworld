using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData
{
    #region Attributes
    private AreaInformation area;
    private bool generatedArea;
    private Optional<CustomAreaMapData> areaMapData;
    #endregion

    public AreaData(AreaInformation area, Optional<CustomAreaMapData> areaMapData)
    {
        this.area = area;
        if(areaMapData.IsPresent())
        {
            generatedArea = true;
            this.areaMapData = areaMapData;
        }
        else
        {
            generatedArea = false;
            this.areaMapData = new Optional<CustomAreaMapData>();
        }
    }

    /// <summary>
    ///     This function converts a <c>AreaDTO</c> object into a <c>AreaData</c> instance
    /// </summary>
    /// <param name="areaDTO">The <c>AreaDTO</c> object to convert</param>
    /// <returns></returns>
    public static AreaData ConvertDtoToData(AreaDTO areaDTO)
    {
        int worldIndex = areaDTO.area.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if(areaDTO.area.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(areaDTO.area.dungeonIndex);
        }
        AreaInformation area = new AreaInformation(worldIndex, dungeonIndex);

        bool generatedArea = areaDTO.generatedArea;

        Optional<CustomAreaMapData> areaMapData = new Optional<CustomAreaMapData>();
        if(generatedArea)
        {
            areaMapData.SetValue(CustomAreaMapData.ConvertDtoToData(areaDTO.customAreaMap));
        }

        AreaData areaData = new AreaData(area, areaMapData);
        return areaData;
    }

    /// <summary>
    ///     This function resets the area map data
    /// </summary>
    public void Reset()
    {
        generatedArea = false;
        areaMapData = new Optional<CustomAreaMapData>();
    }

    #region GetterAndSetter
    public AreaInformation GetArea()
    {
        return area;
    }

    public void SetArea(AreaInformation area)
    {
        this.area = area;
    }

    public bool IsGeneratedArea()
    {
        return generatedArea;
    }

    public CustomAreaMapData GetAreaMapData()
    {
        if(areaMapData.IsPresent())
        {
            return areaMapData.Value();
        }
        else
        {
            return null;
        }
    }

    public void SetAreaMapData(CustomAreaMapData areaMapData)
    {
        generatedArea = true;
        this.areaMapData.SetValue(areaMapData);        
    }    
    #endregion
}
