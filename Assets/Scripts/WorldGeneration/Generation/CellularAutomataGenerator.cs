using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataGenerator : LayoutGenerator
{
    private static readonly int iterations = 10;
    private static readonly int floorNeighborsNeeded = 4;

    public CellularAutomataGenerator(
        Vector2Int size, 
        float accessability, 
        List<WorldConnection> worldConnections) 
        : base(size, accessability, worldConnections) {}

    public CellularAutomataGenerator(
        Vector2Int size,
        float accessability)
        : base(size, accessability) { }

    public override void GenerateLayout()
    {
        Debug.Log("Cellular Automata layout generator");
        Debug.Log("Accessability: " + accessability);

        //CA implementation
        InitializeGrid();

        for(int i=0; i<iterations; i++)
        {
            CAIteration();
        }
    }

    /// <summary>
    ///     This function initializes the layout randomly with the stored probability
    /// </summary>
    private void InitializeGrid()
    {
        for(int x=0; x < size.x; x++)
        {
            for(int y=0; y < size.y; y++)
            {
                if (Random.Range(0f, 1f) < accessability)
                {
                    layout[x, y] = true;
                }
                else
                {
                    layout[x, y] = false;
                }
            }
        }
    }

    /// <summary>
    ///     This function performs one iteration of the Cellular Automata algorithm
    /// </summary>
    private void CAIteration()
    {
        bool[,] newLayout = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for(int y=0; y < size.y; y++)
            {
                bool isFloor = GetNewType(x,y);
                newLayout[x, y] = isFloor;
            }
        }

        layout = newLayout;
    }

    /// <summary>
    ///     This function determines the new type of the cell, based on the neighboring cells
    /// </summary>
    /// <param name="posX">X coordinate of the cell</param>
    /// <param name="posY">Y coordinate of the cell</param>
    /// <returns>True, if cell is now floor, false if it is a wall</returns>
    private bool GetNewType(int posX, int posY)
    {
        int floorNeighbors = GetFloorNeighbors(posX, posY);
;
        if(floorNeighbors >= floorNeighborsNeeded)
        {
            //cell now floor
            return true;
        }
        else
        {
            //cell now wall
            return false;
        }
    }

    /// <summary>
    ///     This function counts the floor neighbors of the given cell
    /// </summary>
    /// <param name="posX">X coordinate of the cell</param>
    /// <param name="posY">Y coordinate of the cell</param>
    /// <returns>The number of neighboring floor cells</returns>
    private int GetFloorNeighbors(int posX, int posY)
    {
        int floorNeighbors = 0;

        for(int x=posX-1; x <= posX+1; x++)
        {
            for(int y=posY-1; y <= posY+1; y++)
            {
                if(x == posX && y == posY)
                {
                    //cell itself, do nothing
                    continue;
                }

                if(x<0 || x >= size.x || y<0 || y >= size.y)
                {
                    //cell is at border, do nothing
                    continue;
                }

                if(layout[x, y])
                {
                    //neighboring cell is floor
                    floorNeighbors++;
                }
            }
        }

        return floorNeighbors;
    }
}
