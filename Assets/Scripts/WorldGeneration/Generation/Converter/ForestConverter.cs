using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestConverter : LayoutConverter
{
    public ForestConverter(CellType[,] baseLayout) : base(baseLayout) { }

    public override void Convert()
    {
        GetTileTypes();
        ConvertToTileSprites();
    }

    /// <summary>
    ///     This function converts the given floor / wall layout to the correct tile types
    /// </summary>
    private void GetTileTypes()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (baseLayout[x, y] != CellType.WALL)
                {
                    //position is floor
                    tileTypes[x, y] = TileType.FOREST_FLOOR;
                }
                else
                {
                    //position in wall
                    tileTypes[x, y] = TileType.FOREST_TREE;
                }
            }
        }
    }

    /// <summary>
    ///     This function converts the given tile type layout to the correct cave tile sprites
    /// </summary>
    private void ConvertToTileSprites()
    {
        for (int x = 0; x < tileTypes.GetLength(0); x++)
        {
            for (int y = 0; y < tileTypes.GetLength(1); y++)
            {
                if(tileTypes[x, y] == TileType.FOREST_FLOOR)
                {
                    //floor tile
                    tileSprites[x, y, 0] = TileSprite.FOREST_GRASS;
                }
                else
                {
                    //wall tile
                    tileSprites[x, y, 0] = TileSprite.FOREST_GRASS;
                    tileSprites[x, y, 4] = TileSprite.FOREST_TREE;
                }
            }
        }
    }
}
