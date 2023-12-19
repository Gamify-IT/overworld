using UnityEngine;

/// <summary>
///     This class contains all data to manage a book spot
/// </summary>
public class BookSpotData
{
    #region Attributes
    private AreaInformation area;
    private int index;
    private Vector2 position;
    private string name;
    #endregion

    public BookSpotData(AreaInformation area, int index, Vector2 position, string name)
    {
        this.area = area;
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
        AreaInformation area = new AreaInformation(bookSpotDTO.area.worldIndex, new Optional<int>());
        if (bookSpotDTO.area.dungeonIndex != 0)
        {
            area.SetDungeonIndex(bookSpotDTO.area.dungeonIndex);
        }
        int index = bookSpotDTO.index;
        Vector2 position = new Vector2(bookSpotDTO.position.x, bookSpotDTO.position.y);
        string name = bookSpotDTO.name;
        BookSpotData data = new BookSpotData(area, index, position, name);
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
    #endregion
}
