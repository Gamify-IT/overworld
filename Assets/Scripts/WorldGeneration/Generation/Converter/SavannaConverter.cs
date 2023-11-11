using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavannaConverter : LayoutConverter
{
    public SavannaConverter(CellType[,] baseLayout) : base(baseLayout) { }

    public override void Convert()
    {
        GetTileTypes();
        ConvertToTileSprites();
    }

    /// <summary>
    ///     This function converts the given floor / wall layout to the correct tile types (floor, wall, void)
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
                    tileTypes[x, y] = TileType.SAVANNA_FLOOR;
                }
                else
                {
                    //position is wall
                    if ((IsInRange(x, y + 1) && baseLayout[x, y + 1] != CellType.WALL) 
                        || (IsInRange(x, y + 2) && baseLayout[x, y + 2] != CellType.WALL)
                        || (IsInRange(x, y + 3) && baseLayout[x, y + 3] != CellType.WALL))
                    {
                        //if one, two or three above is floor -> wall
                        tileTypes[x, y] = TileType.SAVANNA_WALL;
                    }
                    else
                    {
                        //else -> water
                        tileTypes[x, y] = TileType.SAVANNA_WATER;
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
                if (tileTypes[x, y] == TileType.SAVANNA_FLOOR)
                {
                    tileSprites[x, y, 0] = GetFloorSprite(x,y);
                }
                else if (tileTypes[x, y] == TileType.SAVANNA_WALL)
                {
                    tileSprites[x, y, 2] = GetWallSprite(x, y);
                }
                else
                {
                    tileSprites[x, y, 2] = TileSprite.SAVANNA_WATER;
                }
            }
        }
    }

    /// <summary>
    ///     This function finds the correct floor tile for the given position
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <returns>The correct <c>TileSprite</c> to be displayed at the given position</returns>
    private TileSprite GetFloorSprite(int x, int y)
    {
        if (IsInRange(x, y - 1) && tileTypes[x, y - 1] == TileType.SAVANNA_WALL)
        {
            //wall below -> bottom row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] != TileType.SAVANNA_FLOOR)
            {
                //no floor left -> bottom left
                if(tileTypes[x-1,y] == TileType.SAVANNA_WALL)
                {
                    //bottom left wall
                    return TileSprite.SAVANNA_FLOOR_BOTTOM_LEFT_WALL;
                }
                else
                {
                    //bottom left water
                    return TileSprite.SAVANNA_FLOOR_BOTTOM_LEFT_WATER;
                }
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] != TileType.SAVANNA_FLOOR)
            {
                //no floor right -> bottom right
                if (tileTypes[x + 1, y] == TileType.SAVANNA_WALL)
                {
                    //bottom right wall
                    return TileSprite.SAVANNA_FLOOR_BOTTOM_RIGHT_WALL;
                }
                else
                {
                    //bottom right water
                    return TileSprite.SAVANNA_FLOOR_BOTTOM_RIGHT_WATER;
                }
            }
            else
            {
                //bottom mid
                return TileSprite.SAVANNA_FLOOR_BOTTOM_MID;
            }
        }
        else if (IsInRange(x, y + 1) && tileTypes[x, y + 1] == TileType.SAVANNA_WATER)
        {
            //water above -> top row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] != TileType.SAVANNA_FLOOR)
            {
                //no floor left -> top left
                return TileSprite.SAVANNA_FLOOR_TOP_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] != TileType.SAVANNA_FLOOR)
            {
                //no floor right -> top right
                return TileSprite.SAVANNA_FLOOR_TOP_RIGHT;
            }
            else
            {
                //top mid
                return TileSprite.SAVANNA_FLOOR_TOP_MID;
            }
        }
        else
        {
            //mid row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] != TileType.SAVANNA_FLOOR)
            {
                //no floor left -> mid left
                if (tileTypes[x - 1, y] == TileType.SAVANNA_WALL)
                {
                    //mid left wall
                    return TileSprite.SAVANNA_FLOOR_MID_LEFT_WALL;
                }
                else
                {
                    //mid left water
                    return TileSprite.SAVANNA_FLOOR_MID_LEFT_WATER;
                }
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] != TileType.SAVANNA_FLOOR)
            {
                //no floor right -> mid right
                if (tileTypes[x + 1, y] == TileType.SAVANNA_WALL)
                {
                    //mid right wall
                    return TileSprite.SAVANNA_FLOOR_MID_RIGHT_WALL;
                }
                else
                {
                    //mid right water
                    return TileSprite.SAVANNA_FLOOR_MID_RIGHT_WATER;
                }
            }
            else
            {
                //mid mid
                if(IsInRange(x - 1, y + 1) && tileTypes[x - 1, y + 1] == TileType.SAVANNA_WATER)
                {
                    //left inner corner
                    return TileSprite.SAVANNA_FLOOR_MID_INNER_CORNER_LEFT;
                    
                }
                else if(IsInRange(x + 1, y + 1) && tileTypes[x + 1, y + 1] == TileType.SAVANNA_WATER)
                {
                    //right inner corner
                    return TileSprite.SAVANNA_FLOOR_MID_INNER_CORNER_RIGHT;
                }
                else
                {
                    //regular mid sprite
                    return TileSprite.SAVANNA_FLOOR_MID_MID;
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
        if (IsInRange(x, y - 1) && tileTypes[x, y - 1] == TileType.SAVANNA_WALL)
        {
            //Top Wall tile
            if (tileSprites[x, y - 1, 2] == TileSprite.SAVANNA_WALL_BOTTOM_LEFT ||
                tileSprites[x, y - 1, 2] == TileSprite.SAVANNA_WALL_TOP_LEFT_WALL ||
                tileSprites[x, y - 1, 2] == TileSprite.SAVANNA_WALL_TOP_LEFT_WATER)
            {
                //left end of wall
                if(tileTypes[x-1,y] == TileType.SAVANNA_WATER)
                {
                    //connected to water
                    return TileSprite.SAVANNA_WALL_TOP_LEFT_WATER;
                }
                else
                {
                    //connected to wall
                    return TileSprite.SAVANNA_WALL_TOP_LEFT_WALL;
                }
            }
            else if (tileSprites[x, y - 1, 2] == TileSprite.SAVANNA_WALL_BOTTOM_RIGHT ||
                tileSprites[x, y - 1, 2] == TileSprite.SAVANNA_WALL_TOP_RIGHT_WALL ||
                tileSprites[x, y - 1, 2] == TileSprite.SAVANNA_WALL_TOP_RIGHT_WATER)
            {
                //right end of wall
                if (tileTypes[x + 1, y] == TileType.SAVANNA_WATER)
                {
                    //connected to water
                    return TileSprite.SAVANNA_WALL_TOP_RIGHT_WATER;
                }
                else
                {
                    //connected to wall
                    return TileSprite.SAVANNA_WALL_TOP_RIGHT_WALL;
                }
            }
            else
            {
                //middle of wall
                return TileSprite.SAVANNA_WALL_TOP_MID;
            }
        }
        else
        {
            //Bottom Wall tile
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.SAVANNA_WATER)
            {
                //left end of wall
                return TileSprite.SAVANNA_WALL_BOTTOM_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.SAVANNA_WATER)
            {
                //right end of wall
                return TileSprite.SAVANNA_WALL_BOTTOM_RIGHT;
            }
            else
            {
                //middle of wall
                return TileSprite.SAVANNA_WALL_BOTTOM_MID;
            }
        }
    }
}
