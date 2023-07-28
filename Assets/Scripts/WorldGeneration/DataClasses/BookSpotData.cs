using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a book spot
/// </summary>
public class BookSpotData
{
    #region Attributes
    private int worldIndex;
    private Optional<int> dungeonIndex;
    private int index;
    private Vector2 position;
    private string name;
    #endregion

    public BookSpotData(int worldIndex, Optional<int> dungeonIndex, int index, Vector2 position, string name)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.index = index;
        this.position = position;
        this.name = name;
    }

    /// <summary>
    ///     This function converts a <c>BookSpotDTO</c> object into a <c>BookSpotData</c> instance
    /// </summary>
    /// <param name="bookSpotDTO">The <c>BookSpotDTO</c> object to convert</param>
    /// <returns></returns>
    public static BookSpotData ConvertDtoToData(BookSpotDTO bookSpotDTO)
    {
        int worldIndex = bookSpotDTO.location.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if (bookSpotDTO.location.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(bookSpotDTO.location.dungeonIndex);
        }
        int index = bookSpotDTO.index;
        Vector2 position = new Vector2(bookSpotDTO.position.x, bookSpotDTO.position.y);
        string name = bookSpotDTO.name;
        BookSpotData data = new BookSpotData(worldIndex, dungeonIndex, index, position, name);
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
