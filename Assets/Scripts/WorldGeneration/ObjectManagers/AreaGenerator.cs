using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaGenerator
{
    private string[,,] layout;
    private Vector2Int size;
    private WorldStyle style;
    private float accessability;
    private List<WorldConnection> worldConnections;

    public AreaGenerator(Vector2Int size, WorldStyle style, float accessability, List<WorldConnection> worldConnections)
    {
        layout = new string[size.x, size.y, 5];
        this.size = size;
        this.style = style;
        this.accessability = accessability;
        this.worldConnections = worldConnections;
    }

    private void InitLayout()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < 5; z++)
                {
                    layout[x, y, z] = "none";
                }
            }
        }
    }

    public void GenerateLayout()
    {
        InitLayout();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (Random.Range(0f, 1f) < accessability)
                {
                    layout[x,y,0] = "Overworld-Savanna_0";
                }
                else
                {
                    layout[x, y, 2] = "Overworld-Savanna_453";
                }
            }
        }
    }

    public string[,,] GetLayout()
    {
        return layout;
    }
}
