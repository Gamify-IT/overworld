using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a npc spot
/// </summary>
public class NpcSpotData
{
    #region Attributes
    private AreaInformation area;
    private int index;
    private Vector2 position;
    private string name;
    private string spriteName;
    private string iconName;
    #endregion

    public NpcSpotData(AreaInformation area, int index, Vector2 position, string name, string spriteName, string iconName)
    {
        this.area = area;
        this.index = index;
        this.position = position;
        this.name = name;
        this.spriteName = spriteName;
        this.iconName = iconName;
    }

    /// <summary>
    ///     This function converts a <c>NpcSpotDTO</c> object into a <c>NpcSpotData</c> instance
    /// </summary>
    /// <param name="npcSpotDTO">The <c>NpcSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static NpcSpotData ConvertDtoToData(NpcSpotDTO npcSpotDTO)
    {
        AreaInformation area = new AreaInformation(npcSpotDTO.area.worldIndex, new Optional<int>());
        if (npcSpotDTO.area.dungeonIndex != 0)
        {
            area.SetDungeonIndex(npcSpotDTO.area.dungeonIndex);
        }
        int index = npcSpotDTO.index;
        Vector2 position = new Vector2(npcSpotDTO.position.x, npcSpotDTO.position.y);
        string name = npcSpotDTO.name;
        string spriteName = npcSpotDTO.spriteName;
        string iconName = npcSpotDTO.iconName;
        NpcSpotData data = new NpcSpotData(area, index, position, name, spriteName, iconName);
        return data;
    }

    #region GetterAndSetter
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetArea(AreaInformation area)
    {
        this.area = area;
    }

    public AreaInformation GetArea()
    {
        return area;
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
    }

    public void SetSpriteName(string spriteName)
    {
        this.spriteName = spriteName;
    }

    public string GetSpriteName()
    {
        return spriteName;
    }

    public void SetIconName(string iconName)
    {
        this.iconName = iconName;
    }

    public string GetIconName()
    {
        return iconName;
    }
    #endregion
}
