using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutPolisher
{
    private WorldStyle areaStyle;
    private CellType[,] layout;
    private Vector2Int size;
    private int borderSize;

    #region Constructor

    public LayoutPolisher(WorldStyle areaStyle, CellType[,] layout) 
    {
        this.areaStyle = areaStyle;
        this.layout = layout;
        size = new Vector2Int(layout.GetLength(0), layout.GetLength(1));

        GetSettings();
    }

    /// <summary>
    ///     This function reads to generator settings from the local file and sets up the variables needed
    /// </summary>
    private void GetSettings()
    {
        string path = "GameSettings/GeneratorSettings";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        GenerationSettings generationSettings = GenerationSettings.CreateFromJSON(json);

        borderSize = generationSettings.borderSize;
    }

    #endregion

    /// <summary>
    ///     This function polished the given floor / wall layout based on the selected style
    /// </summary>
    /// <returns>The polished layout</returns>
    public CellType[,] Polish()
    {
        switch(areaStyle)
        {
            case WorldStyle.SAVANNA:
                EnsureGroundProperties();
                EnsureWallProperties(2, 4);
                break;

            case WorldStyle.CAVE:
                EnsureWallProperties(2, 4);
                break;

            case WorldStyle.BEACH:
                EnsureGroundProperties();
                break;

            case WorldStyle.FOREST:
                EnsureWallProperties(2, 2);
                break;
        }
        return layout;
    }

    #region Polishing

    private void EnsureWallProperties(int minWidth, int minHeight)
    {
        bool changedSomething = true;

        while (changedSomething)
        {
            changedSomething = false;

            for (int x = borderSize; x < size.x - borderSize; x++)
            {
                for (int y = borderSize; y < size.y - borderSize; y++)
                {
                    if (layout[x, y] == CellType.WALL)
                    {
                        Vector2Int tile = new Vector2Int(x, y);
                        if(!EnoughVerticalWalls(tile, minHeight) || !EnoughHorizontalWalls(tile, minWidth))
                        {
                            layout[x, y] = CellType.FLOOR;
                            changedSomething = true;
                        }
                    }
                }
            }
        }
    }

    private bool EnoughVerticalWalls(Vector2Int tile, int minHeight)
    {
        int verticalWallTiles = 1;

        for(int i=1; i<minHeight; i++)
        {
            if(IsInRange(tile.x, tile.y + i) && layout[tile.x, tile.y + i] == CellType.WALL)
            {
                verticalWallTiles++;
            }
            else
            {
                i = minHeight;
            }
        }

        for (int i = 1; i < minHeight; i++)
        {
            if (IsInRange(tile.x, tile.y - i) && layout[tile.x, tile.y - i]  == CellType.WALL)
            {
                verticalWallTiles++;
            }
            else
            {
                i = minHeight;
            }
        }

        return verticalWallTiles >= minHeight;
    }

    private bool EnoughHorizontalWalls(Vector2Int tile, int minWidth)
    {
        int horizontalWallTiles = 1;

        for (int i = 1; i < minWidth; i++)
        {
            if (IsInRange(tile.x + i, tile.y) && layout[tile.x + i, tile.y] == CellType.WALL)
            {
                horizontalWallTiles++;
            }
            else
            {
                i = minWidth;
            }
        }

        for (int i = 1; i < minWidth; i++)
        {
            if (IsInRange(tile.x - i, tile.y) && layout[tile.x - i, tile.y] == CellType.WALL)
            {
                horizontalWallTiles++;
            }
            else
            {
                i = minWidth;
            }
        }

        return horizontalWallTiles >= minWidth;
    }

    private void EnsureGroundProperties()
    {
        bool changedSomething = true;

        while (changedSomething)
        {
            changedSomething = false;

            for (int x = borderSize; x < size.x - borderSize; x++)
            {
                for (int y = borderSize; y < size.y - borderSize; y++)
                {
                    if (layout[x, y] != CellType.WALL)
                    {
                        if (SingleHorizontalTile(x, y))
                        {
                            //try to extent left or right
                            TryToExtentHorizontally(x, y);
                            changedSomething = true;
                        }
                        else if (SingleVerticalTile(x, y))
                        {
                            //try to extent above or below
                            TryToExtentVertically(x, y);
                            changedSomething = true;
                        }
                    }
                }
            }
        }  
    }

    private bool SingleHorizontalTile(int x, int y)
    {
        return (layout[x - 1, y] == CellType.WALL && layout[x + 1, y] == CellType.WALL);
    }

    private bool SingleVerticalTile(int x, int y)
    {
        return (layout[x, y - 1] == CellType.WALL && layout[x, y + 1] == CellType.WALL);
    }

    private void TryToExtentHorizontally(int x, int y)
    {
        if(layout[x-1, y-1] != CellType.WALL || layout[x - 1, y + 1] != CellType.WALL)
        {
            //extend left
            layout[x - 1, y] = layout[x, y];
        }
        else if (layout[x + 1, y - 1] != CellType.WALL || layout[x + 1, y + 1] != CellType.WALL)
        {
            //extend right
            layout[x + 1, y] = layout[x, y];
        }
        else
        {
            //cannot extend -> turn into wall
            layout[x, y] = CellType.WALL;
        }
    }

    private void TryToExtentVertically(int x, int y)
    {
        if (layout[x - 1, y - 1] != CellType.WALL || layout[x + 1, y - 1] != CellType.WALL)
        {
            //extend down
            layout[x, y - 1] = layout[x, y];
        }
        else if (layout[x - 1, y + 1] != CellType.WALL || layout[x + 1, y + 1] != CellType.WALL)
        {
            //extend up
            layout[x, y + 1] = layout[x, y];
        }
        else
        {
            //cannot extend -> turn into wall
            layout[x, y] = CellType.WALL;
        }
    }


    #endregion

    #region General
    /// <summary>
    ///     This function checks, whether a given position is inside of the layout size
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>True, if in range, false otherwise</returns>
    private bool IsInRange(int posX, int posY)
    {
        return (posX >= 0 && posX < size.x && posY >= 0 && posY < size.y);
    }

    #endregion
}
