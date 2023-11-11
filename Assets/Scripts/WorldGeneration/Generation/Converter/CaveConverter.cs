using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveConverter : LayoutConverter
{

    public CaveConverter(CellType[,] baseLayout) : base(baseLayout) { }

    public override void Convert()
    {
        GetTileTypes();
        ConvertToTileSprites();
    }

    /// <summary>
    ///     This function converts the given floor / wall layout to the correct cave tile types (floor, wall, void)
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
                    tileTypes[x, y] = TileType.CAVE_FLOOR;
                }
                else
                {
                    //position is wall
                    if ( (IsInRange(x,y-1) && baseLayout[x, y - 1] != CellType.WALL) || (IsInRange(x, y - 2) && baseLayout[x, y - 2] != CellType.WALL) )
                    {
                        //if one or two below is floor -> wall
                        tileTypes[x, y] = TileType.CAVE_WALL;
                    }
                    else
                    {
                        //else -> void
                        tileTypes[x, y] = TileType.CAVE_VOID;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     This function converts the given tile type layout to the correct cave tile sprites
    /// </summary>
    private void ConvertToTileSprites()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (tileTypes[x, y] == TileType.CAVE_FLOOR)
                {
                    tileSprites[x, y, 0] = TileSprite.CAVE_FLOOR;
                }
                else if (tileTypes[x, y] == TileType.CAVE_WALL)
                {
                    tileSprites[x, y, 2] = GetWallSprite(x, y);
                }
                else if (tileTypes[x, y] == TileType.CAVE_VOID)
                {
                    tileSprites[x, y, 2] = TileSprite.CAVE_VOID;
                }
            }
        }
    }

    /// <summary>
    ///     This function finds the correct wall tile for the given position
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <returns>The correct <c>TileSprite</c> to be displayed at the given position</returns>
    private TileSprite GetWallSprite(int x, int y)
    {
        if(IsInRange(x, y-1) && tileTypes[x,y-1] == TileType.CAVE_WALL)
        {
            //Top Wall tile
            if(tileSprites[x,y-1,2] == TileSprite.CAVE_WALL_BOTTOM_LEFT)
            {
                //left end of wall
                return TileSprite.CAVE_WALL_TOP_LEFT;
            }
            else if(tileSprites[x, y-1,2] == TileSprite.CAVE_WALL_BOTTOM_RIGHT)
            {
                //right end of wall
                return TileSprite.CAVE_WALL_TOP_RIGHT;
            }
            else
            {
                //middle of wall
                return TileSprite.CAVE_WALL_TOP_MID;
            }
        }
        else
        {
            //Bottom Wall tile
            if(IsInRange(x-1, y) && tileTypes[x-1, y] == TileType.CAVE_FLOOR)
            {
                //left end of wall
                return TileSprite.CAVE_WALL_BOTTOM_LEFT;
            }
            else if(IsInRange(x+1, y) && tileTypes[x+1, y] == TileType.CAVE_FLOOR)
            {
                //right end of wall
                return TileSprite.CAVE_WALL_BOTTOM_RIGHT;
            }
            else
            {
                //middle of wall
                return TileSprite.CAVE_WALL_BOTTOM_MID;
            }
        }
    }
}
