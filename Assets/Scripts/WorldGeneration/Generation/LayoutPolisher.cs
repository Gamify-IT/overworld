using UnityEngine;

/// <summary>
///     This class is used to polish a layout structure to fit the requirements of the selected area style
/// </summary>
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

    /// <summary>
    ///     This function ensures that each wall tile has the given amount of vertically and horizontally connected wall tiles
    /// </summary>
    /// <param name="minWidth">The minimum amount of horizontally connected wall tiles</param>
    /// <param name="minHeight">The minimum amount of vertically connected wall tiles</param>
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

    /// <summary>
    ///     This function checks, if the given tile has enough vertically connected wall tiles
    /// </summary>
    /// <param name="tile">The tile to check</param>
    /// <param name="minHeight">The amount of vertically connected wall tiles needed</param>
    /// <returns>True, if the amount of wall tiles is greater or equal than required, false otherwise</returns>
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

    /// <summary>
    ///     This function checks, if the given tile has enough horizontally connected wall tiles
    /// </summary>
    /// <param name="tile">The tile to check</param>
    /// <param name="minWidth">The amount of horizontally connected wall tiles needed</param>
    /// <returns>True, if the amount of wall tiles is greater or equal than required, false otherwise</returns>
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

    /// <summary>
    ///     This function ensures that each ground tile (either floor or connector) has at least one horizontal and one vertical neighbor
    /// </summary>
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

    /// <summary>
    ///     This function checks, if the given position has no horizonal ground neighbors
    /// </summary>
    /// <param name="x">The x coordinate of the position to check</param>
    /// <param name="y">The y coordinate of the position to check</param>
    /// <returns>True, if there are no horizontal ground neighbors, false otherwise</returns>
    private bool SingleHorizontalTile(int x, int y)
    {
        return (layout[x - 1, y] == CellType.WALL && layout[x + 1, y] == CellType.WALL);
    }

    /// <summary>
    ///     This function checks, if the given position has no vertical ground neighbors
    /// </summary>
    /// <param name="x">The x coordinate of the position to check</param>
    /// <param name="y">The y coordinate of the position to check</param>
    /// <returns>True, if there are no vertical ground neighbors, false otherwise</returns>
    private bool SingleVerticalTile(int x, int y)
    {
        return (layout[x, y - 1] == CellType.WALL && layout[x, y + 1] == CellType.WALL);
    }

    /// <summary>
    ///     This function adds a horizontal ground neighboor, if possible, otherwise it turn the given tile to a wall tile
    /// </summary>
    /// <param name="x">The x coordinate of the position to check</param>
    /// <param name="y">The y coordinate of the position to check</param>
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

    /// <summary>
    ///     This function adds a vertical ground neighboor, if possible, otherwise it turn the given tile to a wall tile
    /// </summary>
    /// <param name="x">The x coordinate of the position to check</param>
    /// <param name="y">The y coordinate of the position to check</param>
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
