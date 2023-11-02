using System;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataGenerator : LayoutGenerator
{
    private int borderSize;    
    private int corridorSize;
    private int iterations;
    private int floorNeighborsNeeded;
    private int floorRoomThreshold;
    private int wallRoomThreshold;

    #region Constructors

    public CellularAutomataGenerator(
        string seed,
        Vector2Int size,
        int accessability, 
        List<WorldConnection> worldConnections) 
        : base(seed, size, accessability, worldConnections) 
    {
        GetSettings();
    }

    public CellularAutomataGenerator(
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(size, accessability, worldConnections) 
    {
        GetSettings();
    }

    public CellularAutomataGenerator(
        string seed,
        Vector2Int size,
        int accessability)
        : base(seed, size, accessability) 
    {
        GetSettings();
    }

    public CellularAutomataGenerator(
        Vector2Int size,
        int accessability)
        : base(size, accessability) 
    {
        GetSettings();
    }

    /// <summary>
    ///     This function reads to generator settings from the local file and sets up the variables needed
    /// </summary>
    private void GetSettings()
    {
        string path = "GameSettings/GeneratorSettings";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        GenerationSettings generationSettings = GenerationSettings.CreateFromJSON(json);
        
        borderSize = generationSettings.borderSize;
        corridorSize = generationSettings.corridorSize;
        iterations = generationSettings.iterationsCA;
        floorNeighborsNeeded = generationSettings.floorThresholdCA;
        floorRoomThreshold = generationSettings.floorRoomThreshold;
        wallRoomThreshold = generationSettings.wallRoomThreshold;
    }

    #endregion

    public override void GenerateLayout()
    {
        Debug.Log("Cellular Automata layout generator" + "\n" + 
            "Accessability: " + accessability + "\n" + 
            "Seed: " + seed);

        InitializeGrid();

        for(int i=0; i<iterations; i++)
        {
            CAIteration();
        }

        //Setup room manager
        RoomManager roomManager = new RoomManager(layout);

        //Remove too small areas
        roomManager.RemoveSmallRooms(TileType.WALL, wallRoomThreshold);
        roomManager.RemoveSmallRooms(TileType.FLOOR, floorRoomThreshold);

        //Add world connections, if present
        if(worldConnections.Count > 0)
        {
            roomManager.AddWorldConnections(worldConnections);
        }

        //Connect rooms
        roomManager.ConnectRooms(corridorSize);

        //Remove small wall areas that might were created
        roomManager.RemoveSmallRooms(TileType.WALL, wallRoomThreshold);

        //Retrieve updated layout
        layout = roomManager.GetLayout();
    }

    #region Iterations

    /// <summary>
    ///     This function initializes the layout randomly with the stored probability and seed
    /// </summary>
    private void InitializeGrid()
    {
        CreateBorder();
        CreateRandomCenter();
    }

    /// <summary>
    ///     This function creates a border of wall tiles of the stored thickness
    /// </summary>
    private void CreateBorder()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < borderSize; y++)
            {
                layout[x, y] = false;
            }
            for (int y = size.y - borderSize; y < size.y; y++)
            {
                layout[x, y] = false;
            }
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < borderSize; x++)
            {
                layout[x, y] = false;
            }
            for (int x = size.x - borderSize; x < size.x; x++)
            {
                layout[x, y] = false;
            }
        }
    }

    /// <summary>
    ///     This function filles the center with the stored seed
    /// </summary>
    private void CreateRandomCenter()
    {
        System.Random pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());

        for (int x = borderSize; x < size.x-borderSize; x++)
        {
            for (int y = borderSize; y < size.y - borderSize; y++)
            {

                if (pseudoRandomNumberGenerator.Next(0, 100) <= accessability)
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

        for (int x = borderSize; x < size.x-borderSize; x++)
        {
            for(int y=borderSize; y < size.y-borderSize; y++)
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
                if(!IsInRange(x,y))
                {
                    //cell is outside border, do nothing
                    continue;
                }

                if (x == posX && y == posY)
                {
                    //cell itself, do nothing
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

    #endregion

    #region General

    /// <summary>
    ///     This function checks, whether a given position is inside of the layout size
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>True, if in range, false otherwise</returns>
    private bool IsInRange(int posX, int posY)
    {
        return (posX >= 0 && posX < size.x && posY >= 0 && posY < size.y);
    }

    #endregion
}
