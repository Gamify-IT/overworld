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
    private Optional<List<WorldConnection>> worldConnections;

    public AreaGenerator(Vector2Int size, WorldStyle style, float accessability, Optional<List<WorldConnection>> worldConnections)
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

        switch (style)
        {
            case WorldStyle.SAVANNA:
                GenerateLayout("Overworld-Savanna_0", "Overworld-Savanna_453");
                break;

            case WorldStyle.BEACH:
                GenerateLayout("Overworld_156", "Overworld_276");
                break;

            case WorldStyle.CAVE:
                GenerateLayout("cave_0", "cave_12");
                break;

            case WorldStyle.FOREST:
                GenerateLayout("Overworld_0", "Overworld_574");
                break;
        }        
    }

    private void GenerateLayout(string groundTile, string wallTile)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (Random.Range(0f, 1f) < accessability)
                {
                    layout[x, y, 0] = groundTile;
                }
                else
                {
                    layout[x, y, 2] = wallTile;
                }
            }
        }
    }

    public string[,,] GetLayout()
    {
        return layout;
    }
}
