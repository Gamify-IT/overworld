using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

/// <summary>
///     This class is used to allow retrieving a World Map Layout from Get Requests.
/// </summary>
[Serializable]
public class LayoutDTO
{
    public AreaLocationDTO area;
    public int sizeX;
    public int sizeY;
    public string generatorType;
    public string seed;
    public int accessability;
    public string style;

    #region Constructors
    public LayoutDTO() { }

    public LayoutDTO(AreaLocationDTO area, int sizeX, int sizeY, string generatorType, string seed, int accessability, string style)
    {
        this.area = area;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.generatorType = generatorType;
        this.seed = seed;
        this.accessability = accessability;
        this.style = style;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>Layout</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>Layout</c> object containing the data</returns>
    public static LayoutDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LayoutDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>Layout</c> object into a <c>LayoutDTO</c> instance
    /// </summary>
    /// <param name="layout">The <c>Layout</c> object to convert</param>
    /// <returns>The converted <c>LayoutDTO</c> object</returns>
    public static LayoutDTO ConvertDataToDto(Layout layout)
    {
        int worldIndex = layout.GetArea().GetWorldIndex();
        int dungeonIndex = 0;
        if(layout.GetArea().IsDungeon())
        {
            dungeonIndex = layout.GetArea().GetDungeonIndex();
        }
        AreaLocationDTO area = new AreaLocationDTO(worldIndex, dungeonIndex);

        int sizeX = layout.GetTiles().GetLength(0);
        int sizeY = layout.GetTiles().GetLength(1);
        string generatorType = layout.GetGeneratorType().ToString();
        string seed = layout.GetSeed();
        int accessability = layout.GetAccessability();
        string style = layout.GetStyle().ToString();

        LayoutDTO data = new LayoutDTO(area, sizeX, sizeY, generatorType, seed, accessability, style);
        return data;
    }
}
