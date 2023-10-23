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
    public string[,,] Convert(bool[,] baseLayout)
    {
        string[,,] layout = new string[baseLayout.GetLength(0), baseLayout.GetLength(1), 5];
        InitLayout(layout);

        string groundTile = "";
        string wallTile = "";

        switch (areaStyle)
        {
            case WorldStyle.SAVANNA:
                groundTile = "Overworld-Savanna_0";
                wallTile = "Overworld-Savanna_453";
                break;

            case WorldStyle.BEACH:
                groundTile = "Overworld_156";
                wallTile = "Overworld_276";
                break;

            case WorldStyle.CAVE:
                groundTile = "cave_0";
                wallTile = "cave_12";
                break;

            case WorldStyle.FOREST:
                groundTile = "Overworld_0";
                wallTile = "Overworld_574";
                break;
        }

        for(int x=0; x< baseLayout.GetLength(0); x++)
        {
            for(int y=0; y< baseLayout.GetLength(1); y++)
            {
                if(baseLayout[x,y])
                {
                    //position is floor
                    layout[x, y, 0] = groundTile;
                }
                else
                {
                    //position is wall
                    layout[x, y, 2] = wallTile;
                }
            }
        }

        return layout;
    }

    /// <summary>
    ///     This function initializes the layout array
    /// </summary>
    private void InitLayout(string[,,] layout) 
    {
        for (int x = 0; x < layout.GetLength(0); x++)
        {
            for (int y = 0; y < layout.GetLength(1); y++)
            {
                for (int z = 0; z < 5; z++)
                {
                    layout[x, y, z] = "";
                }
            }
        }
    }
}
