using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutConverter
{
    private WorldStyle areaStyle;

    public LayoutConverter(WorldStyle areaStyle)
    {
        this.areaStyle = areaStyle;
    }

    /// <summary>
    ///     This function converts the given floor / wall layout to the correct tiles, based on the selected style
    /// </summary>
    /// <param name="baseLayout">The layout to convert</param>
    /// <returns>The converted layout</returns>
    public TileType[,] Convert(bool[,] baseLayout)
    {
        TileType[,] layout = new TileType[baseLayout.GetLength(0), baseLayout.GetLength(1)];
        InitLayout(layout);

        TileType groundTile = TileType.UNDEFINED;
        TileType wallTile = TileType.UNDEFINED;

        switch (areaStyle)
        {
            case WorldStyle.SAVANNA:
                groundTile = TileType.SAVANNA_FLOOR;
                wallTile = TileType.SAVANNA_WALL;
                break;

            case WorldStyle.BEACH:
                groundTile = TileType.BEACH_FLOOR;
                wallTile = TileType.BEACH_WATER;
                break;

            case WorldStyle.CAVE:
                groundTile = TileType.CAVE_FLOOR;
                wallTile = TileType.CAVE_WALL;
                break;

            case WorldStyle.FOREST:
                groundTile = TileType.FOREST_FLOOR;
                wallTile = TileType.FOREST_TREE;
                break;
        }

        for(int x=0; x< baseLayout.GetLength(0); x++)
        {
            for(int y=0; y< baseLayout.GetLength(1); y++)
            {
                if(baseLayout[x,y])
                {
                    //position is floor
                    layout[x, y] = groundTile;
                }
                else
                {
                    //position is wall
                    layout[x, y] = wallTile;
                }
            }
        }

        return layout;
    }

    /// <summary>
    ///     This function initializes the layout array
    /// </summary>
    private void InitLayout(TileType[,] layout) 
    {
        for (int x = 0; x < layout.GetLength(0); x++)
        {
            for (int y = 0; y < layout.GetLength(1); y++)
            {
                layout[x, y] = TileType.UNDEFINED;
            }
        }
    }
}
