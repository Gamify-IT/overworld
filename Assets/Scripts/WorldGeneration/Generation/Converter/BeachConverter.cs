using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachConverter : LayoutConverter
{
    private static readonly float wavePercentage = 0.05f;
    private static readonly float waterDecorationPercentage = 0.02f;

    public BeachConverter(CellType[,] baseLayout) : base(baseLayout) { }

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
                if (baseLayout[x, y] == CellType.FLOOR)
                {
                    //position is floor
                    tileTypes[x, y] = TileType.BEACH_FLOOR;
                }
                else if(baseLayout[x ,y] == CellType.CORRIDOR)
                {
                    //position is connection
                    tileTypes[x, y] = TileType.BEACH_CONNECTION;
                }
                else
                {
                    tileTypes[x, y] = TileType.BEACH_WATER;
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
                if (tileTypes[x, y] == TileType.BEACH_FLOOR)
                {
                    tileSprites[x, y, 0] = GetFloorSprite(x, y);
                }
                else if(tileTypes[x, y] == TileType.BEACH_CONNECTION)
                {
                    tileSprites[x, y, 0] = GetConnectionSprite(x, y);
                }
                else if (tileTypes[x, y] == TileType.BEACH_WATER)
                {
                    //add random waves
                    if (Random.Range(0f, 1f) < wavePercentage)
                    {
                        tileSprites[x, y, 2] = TileSprite.BEACH_WATER_WAVE;
                    }
                    else
                    {
                        tileSprites[x, y, 2] = TileSprite.BEACH_WATER;

                        //add random water decoration
                        if (Random.Range(0f, 1f) < waterDecorationPercentage)
                        {
                            tileSprites[x, y, 3] = TileSprite.BEACH_WATER_DECORATION;
                        }
                    }
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
        if (IsInRange(x, y - 1) && tileTypes[x, y - 1] == TileType.BEACH_WATER)
        {
            //wall below -> bottom row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.BEACH_WATER)
            {
                //no floor left -> bottom left
                return TileSprite.BEACH_FLOOR_BOTTOM_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.BEACH_WATER)
            {
                //no floor right -> bottom right
                return TileSprite.BEACH_FLOOR_BOTTOM_RIGHT;
            }
            else
            {
                //bottom mid
                return TileSprite.BEACH_FLOOR_BOTTOM_MID;
            }
        }
        else if (IsInRange(x, y + 1) && tileTypes[x, y + 1] == TileType.BEACH_WATER)
        {
            //water above -> top row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.BEACH_WATER)
            {
                //no floor left -> top left
                return TileSprite.BEACH_FLOOR_TOP_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.BEACH_WATER)
            {
                //no floor right -> top right
                return TileSprite.BEACH_FLOOR_TOP_RIGHT;
            }
            else
            {
                //top mid
                return TileSprite.BEACH_FLOOR_TOP_MID;
            }
        }
        else
        {
            //mid row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.BEACH_WATER)
            {
                //no floor left -> mid left
                return TileSprite.BEACH_FLOOR_MID_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.BEACH_WATER)
            {
                //no floor right -> mid right
                return TileSprite.BEACH_FLOOR_MID_RIGHT;
            }
            else
            {
                //mid mid
                if (IsInRange(x - 1, y + 1) && tileTypes[x - 1, y + 1] == TileType.BEACH_WATER)
                {
                    //inner corner top left
                    return TileSprite.BEACH_FLOOR_INNER_CORNER_TOP_LEFT;
                }
                else if (IsInRange(x + 1, y + 1) && tileTypes[x + 1, y + 1] == TileType.BEACH_WATER)
                {
                    //inner corner top right
                    return TileSprite.BEACH_FLOOR_INNER_CORNER_TOP_RIGHT;
                }
                else if (IsInRange(x - 1, y - 1) && tileTypes[x - 1, y - 1] == TileType.BEACH_WATER)
                {
                    //inner corner bottom left
                    return TileSprite.BEACH_FLOOR_INNER_CORNER_BOTTOM_LEFT;
                }
                else if (IsInRange(x + 1, y - 1) && tileTypes[x + 1, y - 1] == TileType.BEACH_WATER)
                {
                    //inner corner bottom right
                    return TileSprite.BEACH_FLOOR_INNER_CORNER_BOTTOM_RIGHT;
                }
                else
                {
                    //regular mid sprite
                    return TileSprite.BEACH_FLOOR_MID_MID;
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
    private TileSprite GetConnectionSprite(int x, int y)
    {
        if (IsInRange(x, y - 1) && tileTypes[x, y - 1] == TileType.BEACH_WATER)
        {
            //wall below -> bottom row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.BEACH_WATER)
            {
                //no floor left -> bottom left
                return TileSprite.BEACH_CONNECTION_BOTTOM_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.BEACH_WATER)
            {
                //no floor right -> bottom right
                return TileSprite.BEACH_CONNECTION_BOTTOM_RIGHT;
            }
            else
            {
                //bottom mid
                return TileSprite.BEACH_CONNECTION_BOTTOM_MID;
            }
        }
        else if (IsInRange(x, y + 1) && tileTypes[x, y + 1] == TileType.BEACH_WATER)
        {
            //water above -> top row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.BEACH_WATER)
            {
                //no floor left -> top left
                return TileSprite.BEACH_CONNECTION_TOP_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.BEACH_WATER)
            {
                //no floor right -> top right
                return TileSprite.BEACH_CONNECTION_TOP_RIGHT;
            }
            else
            {
                //top mid
                return TileSprite.BEACH_CONNECTION_TOP_MID;
            }
        }
        else
        {
            //mid row
            if (IsInRange(x - 1, y) && tileTypes[x - 1, y] == TileType.BEACH_WATER)
            {
                //no floor left -> mid left
                return TileSprite.BEACH_CONNECTION_MID_LEFT;
            }
            else if (IsInRange(x + 1, y) && tileTypes[x + 1, y] == TileType.BEACH_WATER)
            {
                //no floor right -> mid right
                return TileSprite.BEACH_CONNECTION_MID_RIGHT;
            }
            else
            {
                //mid mid
                if (IsInRange(x - 1, y + 1) && tileTypes[x - 1, y + 1] == TileType.BEACH_WATER)
                {
                    //inner corner top left
                    return TileSprite.BEACH_CONNECTION_INNER_CORNER_TOP_LEFT;
                }
                else if (IsInRange(x + 1, y + 1) && tileTypes[x + 1, y + 1] == TileType.BEACH_WATER)
                {
                    //inner corner top right
                    return TileSprite.BEACH_CONNECTION_INNER_CORNER_TOP_RIGHT;
                }
                else if (IsInRange(x - 1, y - 1) && tileTypes[x - 1, y - 1] == TileType.BEACH_WATER)
                {
                    //inner corner bottom left
                    return TileSprite.BEACH_CONNECTION_INNER_CORNER_BOTTOM_LEFT;
                }
                else if (IsInRange(x + 1, y - 1) && tileTypes[x + 1, y - 1] == TileType.BEACH_WATER)
                {
                    //inner corner bottom right
                    return TileSprite.BEACH_CONNECTION_INNER_CORNER_BOTTOM_RIGHT;
                }
                else
                {
                    //regular mid sprite
                    return TileSprite.BEACH_CONNECTION_MID_MID;
                }
            }
        }
    }
}
