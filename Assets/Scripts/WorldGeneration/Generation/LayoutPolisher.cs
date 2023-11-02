using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutPolisher
{
    private WorldStyle areaStyle;
    private bool[,] layout;
    private Vector2Int size;
    private int borderSize;

    #region Constructor

    public LayoutPolisher(WorldStyle areaStyle, bool[,] layout) 
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
    public bool[,] Polish()
    {
        int minWidth = 2;
        int minHeight = 0;

        switch(areaStyle)
        {
            case WorldStyle.SAVANNA:
                minWidth = 2;
                minHeight = 4;
                break;

            case WorldStyle.CAVE:
                minWidth = 2;
                minHeight = 4;
                break;

            case WorldStyle.BEACH:
                minWidth = 2;
                minHeight = 2;
                break;

            case WorldStyle.FOREST:
                minWidth = 2; 
                minHeight = 2;
                break;
        }

        EnsureWallProperties(minWidth, minHeight);

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
                    if (GetTileType(x, y) == TileType.WALL)
                    {
                        Vector2Int tile = new Vector2Int(x, y);
                        if(!EnoughVerticalWalls(tile, minHeight) || !EnoughHorizontalWalls(tile, minWidth))
                        {
                            layout[x, y] = true;
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
            if(IsInRange(tile.x, tile.y + i) && GetTileType(tile.x, tile.y + i) == TileType.WALL)
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
            if (IsInRange(tile.x, tile.y - i) && GetTileType(tile.x, tile.y - i) == TileType.WALL)
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
            if (IsInRange(tile.x + i, tile.y) && GetTileType(tile.x + i, tile.y) == TileType.WALL)
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
            if (IsInRange(tile.x - i, tile.y) && GetTileType(tile.x - i, tile.y) == TileType.WALL)
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

    #endregion

    #region General

    /// <summary>
    ///     This function returns the tile type of the given position
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>The type of the tile</returns>
    private TileType GetTileType(int posX, int posY)
    {
        if (layout[posX, posY])
        {
            return TileType.FLOOR;
        }
        else
        {
            return TileType.WALL;
        }
    }

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
