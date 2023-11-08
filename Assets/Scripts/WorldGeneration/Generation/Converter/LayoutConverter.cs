using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines an Layout Converter. It is used to inherit for specific converter implementation.
/// </summary>
public abstract class LayoutConverter
{
    protected bool[,] baseLayout;
    protected Vector2Int size;
    protected TileType[,] tileTypes;
    protected TileSprite[,] tileSprites;    

    public LayoutConverter(bool[,] baseLayout)
    {
        this.baseLayout = baseLayout;
        size = new Vector2Int(baseLayout.GetLength(0), baseLayout.GetLength(1));
        tileTypes = new TileType[size.x, size.y];
        tileSprites = new TileSprite[size.x, size.y];
    }

    public abstract void Convert();

    public TileSprite[,] GetTileSprites()
    {
        return tileSprites;
    }

    protected bool IsInRange(int x, int y)
    {
        return (x >= 0 && x < size.x && y >= 0 && y < size.y);
    }
}
