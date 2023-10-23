using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve npc spot data from Get Requests.
/// </summary>
[Serializable]
public class NpcSpotDTO
{
    #region Attributes
    public AreaLocationDTO area;
    public int index;
    public Position position;
    public string name;
    public string spriteName;
    public string iconName;
    #endregion

    #region Constructors
    public NpcSpotDTO() { }

    public NpcSpotDTO(AreaLocationDTO area, int index, Position position, string name, string spriteName, string iconName)
    {
        this.area = area;
        this.index = index;
        this.position = position;
        this.name = name;
        this.spriteName = spriteName;
        this.iconName = iconName;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>NpcSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>NpcSpotDTO</c> object containing the data</returns>
    public static NpcSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NpcSpotDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>NpcSpotData</c> object into a <c>NpcSpotDTO</c> instance
    /// </summary>
    /// <param name="npcSpotData">The <c>NpcSpotData</c> object to convert</param>
    /// <returns></returns>
    public static NpcSpotDTO ConvertDataToDto(NpcSpotData npcSpotData)
    {
        AreaLocationDTO areaLocation = new AreaLocationDTO(npcSpotData.GetArea().GetWorldIndex(), 0);
        if (npcSpotData.GetArea().IsDungeon())
        {
            areaLocation.dungeonIndex = npcSpotData.GetArea().GetDungeonIndex();
        }
        int index = npcSpotData.GetIndex();
        Position position = new Position(npcSpotData.GetPosition().x, npcSpotData.GetPosition().y);
        string name = npcSpotData.GetName();
        string spriteName = npcSpotData.GetSpriteName();
        string iconName = npcSpotData.GetIconName();
        NpcSpotDTO data = new NpcSpotDTO(areaLocation, index, position, name, spriteName, iconName);
        return data;
    }
}
