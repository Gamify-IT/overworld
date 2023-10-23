using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAreas
{
    private Dictionary<int, AreaData> areas;

    public WorldAreas()
    {
        areas = new Dictionary<int, AreaData>();
    }

    public void AddArea(int key, AreaData data)
    {
        areas.Add(key, data);
    }

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
