using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestConverter : LayoutConverter
{
    public ForestConverter(bool[,] baseLayout) : base(baseLayout) { }

    public override void Convert()
    {
        tileTypes = GetTileTypes();
        tileSprites = ConvertToTileSprites();
    }

    /// <summary>
    ///     This function converts the given floor / wall layout to the correct tile types
    /// </summary>
    /// <returns>The converted type layout</returns>
    private TileType[,] GetTileTypes()
    {
        TileType[,] layout = new TileType[baseLayout.GetLength(0), baseLayout.GetLength(1)];

        for (int x = 0; x < baseLayout.GetLength(0); x++)
        {
            for (int y = 0; y < baseLayout.GetLength(1); y++)
            {
                if (baseLayout[x, y])
                {
                    //position is floor
                    layout[x, y] = TileType.FOREST_FLOOR;
                }
                else
                {
                    //position in wall
                    layout[x, y] = TileType.FOREST_FLOOR;
                }
            }
        }

        return layout;
    }

    /// <summary>
    ///     This function converts the given tile type layout to the correct cave tile sprites
    /// </summary>
    /// <returns>The converted cave sprite layout</returns>
    private TileSprite[,] ConvertToTileSprites()
    {
        TileSprite[,] layout = new TileSprite[tileTypes.GetLength(0), tileTypes.GetLength(1)];

        for (int x = 0; x < tileTypes.GetLength(0); x++)
        {
            for (int y = 0; y < tileTypes.GetLength(1); y++)
            {
                if (tileTypes[x, y] == TileType.FOREST_FLOOR)
                {
                    layout[x, y] = TileSprite.FOREST_FLOOR;
                }
                else if (tileTypes[x, y] == TileType.FOREST_TREE)
                {
                    layout[x, y] = TileSprite.FOREST_TREE;
                }
            }
        }

        return layout;
    }
}
