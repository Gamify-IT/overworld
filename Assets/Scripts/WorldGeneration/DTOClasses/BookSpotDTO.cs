using UnityEngine;
using System;

/// <summary>
///     This class is used to transfer an <c>BookSpotData</c> from and to the overworld backend
/// </summary>
[Serializable]
public class BookSpotDTO
{
    #region Attributes
    public AreaLocationDTO area;
    public int index;
    public Position position;
    public string name;
    #endregion

    #region Constructors
    public BookSpotDTO() { }

    public BookSpotDTO(AreaLocationDTO area, int index, Position position, string name)
    {
        this.area = area;
        this.index = index;
        this.position = position;
        this.name = name;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>BookSpotDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>BookSpotDTO</c> object containing the data</returns>
    public static BookSpotDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BookSpotDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>BookSpotData</c> object into a <c>BookSpotDTO</c> instance
    /// </summary>
    /// <param name="bookSpotData">The <c>BookSpotData</c> object to convert</param>
    /// <returns></returns>
    public static BookSpotDTO ConvertDataToDto(BookSpotData bookSpotData)
    {
        AreaLocationDTO areaLocation = new AreaLocationDTO(bookSpotData.GetArea().GetWorldIndex(), 0);
        if (bookSpotData.GetArea().IsDungeon())
        {
            areaLocation.dungeonIndex = bookSpotData.GetArea().GetDungeonIndex();
        }
        int index = bookSpotData.GetIndex();
        Position position = new Position(bookSpotData.GetPosition().x, bookSpotData.GetPosition().y);
        string name = bookSpotData.GetName();
        BookSpotDTO data = new BookSpotDTO(areaLocation, index, position, name);
        return data;
    }
}
