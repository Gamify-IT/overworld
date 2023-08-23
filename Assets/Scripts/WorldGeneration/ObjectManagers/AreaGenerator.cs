using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaGenerator
{
    private static int borderThickness = 3;
    private static int worldConnectionWidth = 1;

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
        GenerateBorder(groundTile, wallTile);

        for (int x = borderThickness; x < size.x-borderThickness; x++)
        {
            for (int y = borderThickness; y < size.y-borderThickness; y++)
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

    /// <summary>
    ///     This function creates a border around the area
    /// </summary>
    /// <param name="groundTile">The ground tile</param>
    /// <param name="wallTile">The wall tile</param>
    private void GenerateBorder(string groundTile, string wallTile)
    {
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < borderThickness; y++)
            {
                layout[x, y, 2] = wallTile;
            }
            for(int y = size.y-borderThickness; y < size.y; y++)
            {
                layout[x, y, 2] = wallTile;
            }
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < borderThickness; x++)
            {
                layout[x, y, 2] = wallTile;
            }
            for (int x = size.x - borderThickness; x < size.x; x++)
            {
                layout[x, y, 2] = wallTile;
            }
        }

        foreach (WorldConnection worldConnection in worldConnections)
        {
            //connection on left side
            if(worldConnection.GetPosition().x == 0 && worldConnection.GetPosition().y > (borderThickness+worldConnectionWidth) && worldConnection.GetPosition().y < size.y - (borderThickness+worldConnectionWidth))
            {
                for(int x=0; x<borderThickness; x++)
                {
                    for(int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[x, worldConnection.GetPosition().y+offset, 2] = "none";
                        layout[x, worldConnection.GetPosition().y+offset, 0] = groundTile;
                    }
                }
            }

            //connection on bottom side
            if (worldConnection.GetPosition().y == 0 && worldConnection.GetPosition().x > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().x < size.x - (borderThickness + worldConnectionWidth))
            {
                for (int y = 0; y < borderThickness; y++)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[worldConnection.GetPosition().x + offset, y, 2] = "none";
                        layout[worldConnection.GetPosition().x + offset, y, 0] = groundTile;
                    }
                }
            }

            //connection on right side
            if (worldConnection.GetPosition().x == size.x && worldConnection.GetPosition().y > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().y < size.y - (borderThickness + worldConnectionWidth))
            {
                for (int x = size.x-1; x > size.x-1-borderThickness; x--)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[x, worldConnection.GetPosition().y+offset, 2] = "none";
                        layout[x, worldConnection.GetPosition().y+offset, 0] = groundTile;
                    }
                }
            }

            //connection on top side
            if (worldConnection.GetPosition().y == size.y && worldConnection.GetPosition().x > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().x < size.x - (borderThickness + worldConnectionWidth))
            {
                for (int y = size.y - 1; y > size.y - 1 - borderThickness; y--)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[worldConnection.GetPosition().x + offset, y, 2] = "none";
                        layout[worldConnection.GetPosition().x + offset, y, 0] = groundTile;
                    }
                }
            }
        }        
    }

    public string[,,] GetLayout()
    {
        return layout;
    }
}
