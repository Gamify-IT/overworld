using UnityEngine;

/// <summary>
///     This class defines a Layout Converter for the forest area style
/// </summary>
public class ForestConverter : LayoutConverter
{
    private static readonly float floorDecorationPercentage = 0.05f;

    public ForestConverter(CellType[,] baseLayout) : base(baseLayout) { }

    /// <summary>
    ///     This function converts the stored layout according to the rules for a forest layout
    /// </summary>
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
    ///     This function converts the given tile type layout to the correct forest sprites
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

                    //add random floor decoration
                    if (Random.Range(0f, 1f) < floorDecorationPercentage)
                    {
                        tileSprites[x, y, 1] = TileSprite.FOREST_FLOOR_DECORATION;
                    }
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
