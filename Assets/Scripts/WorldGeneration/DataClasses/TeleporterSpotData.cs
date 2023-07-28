using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSpotData
{
    #region Attributes
    private int worldIndex;
    private Optional<int> dungeonIndex;
    private int index;
    private Vector2 position;
    private string name;
    #endregion

    public TeleporterSpotData(int worldIndex, Optional<int> dungeonIndex, int index, Vector2 position, string name)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.index = index;
        this.position = position;
        this.name = name;
    }

    /// <summary>
    ///     This function converts a <c>TeleporterSpotDTO</c> object into a <c>TeleporterSpotData</c> instance
    /// </summary>
    /// <param name="teleporterSpotDTO">The <c>TeleporterSpotDTO</c> object to convert</param>
    /// <returns>A <c>TeleporterSpotData</c> object with the data</returns>
    public static TeleporterSpotData ConvertDtoToData(TeleporterSpotDTO teleporterSpotDTO)
    {
        int worldIndex = teleporterSpotDTO.location.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if (teleporterSpotDTO.location.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(teleporterSpotDTO.location.dungeonIndex);
        }
        int index = teleporterSpotDTO.index;
        Vector2 position = new Vector2(teleporterSpotDTO.position.x, teleporterSpotDTO.position.y);
        string name = teleporterSpotDTO.name;
        TeleporterSpotData data = new TeleporterSpotData(worldIndex, dungeonIndex, index, position, name);
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
    #endregion
}
