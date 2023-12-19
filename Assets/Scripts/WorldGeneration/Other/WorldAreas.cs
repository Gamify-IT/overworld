using System.Collections.Generic;

/// <summary>
///     This class manages the <c>AreaData</c> for a world and its dungeons in the play gamemode
/// </summary>
public class WorldAreas
{
    private Dictionary<int, AreaData> areas;

    public WorldAreas()
    {
        areas = new Dictionary<int, AreaData>();
    }

    /// <summary>
    ///     This function adds an area data
    /// </summary>
    /// <param name="key">The index of the area in the worlds</param>
    /// <param name="data">The data to store</param>
    public void AddArea(int key, AreaData data)
    {
        areas.Add(key, data);
    }

    /// <summary>
    ///     This function returns the stored data for a given index
    /// </summary>
    /// <param name="key">The index of the area in its world</param>
    /// <returns>An optional containing the <c>AreaData</c>, if present, an empty optional otherwise</returns>
    public Optional<AreaData> GetArea(int key)
    {
        Optional<AreaData> data = new Optional<AreaData>();

        if(areas.ContainsKey(key))
        {
            data.SetValue(areas[key]);
        }

        return data;
    }
}
