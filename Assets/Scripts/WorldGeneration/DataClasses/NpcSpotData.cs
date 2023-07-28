using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a npc spot
/// </summary>
public class NpcSpotData
{
    #region Attributes
    private int worldIndex;
    private Optional<int> dungeonIndex;
    private int index;
    private Vector2 position;
    private string name;
    private string spriteName;
    private string iconName;
    #endregion

    public NpcSpotData(int worldIndex, Optional<int> dungeonIndex, int index, Vector2 position, string name, string spriteName, string iconName)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
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
        int worldIndex = npcSpotDTO.location.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if (npcSpotDTO.location.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(npcSpotDTO.location.dungeonIndex);
        }
        int index = npcSpotDTO.index;
        Vector2 position = new Vector2(npcSpotDTO.position.x, npcSpotDTO.position.y);
        string name = npcSpotDTO.name;
        string spriteName = npcSpotDTO.spriteName;
        string iconName = npcSpotDTO.iconName;
        NpcSpotData data = new NpcSpotData(worldIndex, dungeonIndex, index, position, name, spriteName, iconName);
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

    public void SetWorldIndex(int worldIndex)
    {
        this.worldIndex = worldIndex;
    }

    public int GetWorldIndex()
    {
        return worldIndex;
    }

    public void SetDungeonIndex(int dungeonIndex)
    {
        this.dungeonIndex.SetValue(dungeonIndex);
    }

    public bool IsDungeon()
    {
        return dungeonIndex.IsPresent();
    }

    public int GetDungeonIndex()
    {
        return dungeonIndex.Value();
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
