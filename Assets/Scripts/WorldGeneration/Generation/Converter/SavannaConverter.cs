using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavannaConverter : LayoutConverter
{
    public SavannaConverter(bool[,] baseLayout) : base(baseLayout) { }

    public override void Convert()
    {
        tileTypes = GetTileTypes();
        tileSprites = ConvertToTileSprites();
    }

    /// <summary>
    ///     This function converts the given floor / wall layout to the correct tile types (floor, wall, void)
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
                    layout[x, y] = TileType.SAVANNA_FLOOR;
                }
                else
                {
                    //position in wall
                    layout[x, y] = TileType.SAVANNA_WALL;
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
                if (tileTypes[x, y] == TileType.SAVANNA_FLOOR)
                {
                    layout[x, y] = TileSprite.SAVANNA_FLOOR;
                }
                else if (tileTypes[x, y] == TileType.SAVANNA_WALL)
                {
                    layout[x, y] = TileSprite.SAVANNA_WALL;
                }
            }
        }

        return layout;
    }
}
