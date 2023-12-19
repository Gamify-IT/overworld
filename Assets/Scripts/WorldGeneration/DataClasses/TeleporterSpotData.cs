using UnityEngine;

/// <summary>
///     This class contains all data to manage a teleporter spot
/// </summary>
public class TeleporterSpotData
{
    #region Attributes
    AreaInformation area;
    private int index;
    private Vector2 position;
    private string name;
    #endregion

    public TeleporterSpotData(AreaInformation area, int index, Vector2 position, string name)
    {
        this.area = area;
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
        AreaInformation area = new AreaInformation(teleporterSpotDTO.area.worldIndex, new Optional<int>());
        if (teleporterSpotDTO.area.dungeonIndex != 0)
        {
            area.SetDungeonIndex(teleporterSpotDTO.area.dungeonIndex);
        }
        int index = teleporterSpotDTO.index;
        Vector2 position = new Vector2(teleporterSpotDTO.position.x, teleporterSpotDTO.position.y);
        string name = teleporterSpotDTO.name;
        TeleporterSpotData data = new TeleporterSpotData(area, index, position, name);
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
