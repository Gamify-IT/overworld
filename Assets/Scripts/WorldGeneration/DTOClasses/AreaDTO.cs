using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaDTO
{
    #region Attributes
    public AreaLocationDTO area;
    public bool generatedArea;
    public CustomAreaMapDTO areaMapDTO;
    #endregion

    #region Constructors
    public AreaDTO() { }

    public AreaDTO(AreaLocationDTO area, bool generatedArea, CustomAreaMapDTO areaMapDTO)
    {
        this.area = area;
        this.generatedArea = generatedArea;
        this.areaMapDTO = areaMapDTO;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>AreaDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>AreaDTO</c> object containing the data</returns>
    public static AreaDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AreaDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>AreaData</c> object into a <c>AreaDTO</c> instance
    /// </summary>
    /// <param name="areaData">The <c>AreaData</c> object to convert</param>
    /// <returns></returns>
    public static AreaDTO ConvertDataToDto(AreaData areaData)
    {
        int worldIndex = areaData.GetArea().GetWorldIndex();
        int dungeonIndex = 0;
        if(areaData.GetArea().IsDungeon())
        {
            dungeonIndex = areaData.GetArea().GetDungeonIndex();
        }
        AreaLocationDTO area = new AreaLocationDTO(worldIndex, dungeonIndex);

        bool generatedArea = areaData.IsGeneratedArea();

        CustomAreaMapDTO areaMapDTO = new CustomAreaMapDTO();
        if(generatedArea)
        {
            areaMapDTO = CustomAreaMapDTO.ConvertDataToDto(areaData.GetAreaMapData());
        }

        AreaDTO areaDTO = new AreaDTO(area, generatedArea, areaMapDTO);
        return areaDTO;
    }
}
