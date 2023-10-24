using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CellularAutomataGenerator : LayoutGenerator
{
    private static readonly int iterations = 10;
    private static readonly int floorNeighborsNeeded = 4;
    private static readonly int floorRoomThreshold = 100;
    private static readonly int wallRoomThreshold = 50;

    #region Constructors

    public CellularAutomataGenerator(
        string seed,
        Vector2Int size,
        int accessability, 
        List<WorldConnection> worldConnections) 
        : base(seed, size, accessability, worldConnections) {}

    public CellularAutomataGenerator(
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(size, accessability, worldConnections) {}

    public CellularAutomataGenerator(
        string seed,
        Vector2Int size,
        int accessability)
        : base(seed, size, accessability) {}

    public CellularAutomataGenerator(
        Vector2Int size,
        int accessability)
        : base(size, accessability) {}

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

        Debug.Log("Floor threshold: " + floorRoomThreshold);
        Debug.Log("Wall threshold: " + wallRoomThreshold);

        RemoveSmallRooms();
    }

    #region Iteratins

    /// <summary>
    ///     This function initializes the layout randomly with the stored probability
    /// </summary>
    private void InitializeGrid()
    {
        System.Random pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());

        for (int x=0; x < size.x; x++)
        {
            for(int y=0; y < size.y; y++)
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
                if(!PositionIsInRange(x,y))
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

    #region Rooms

    /// <summary>
    ///     This function removes all rooms with size below the threshold
    /// </summary>
    private void RemoveSmallRooms()
    {
        //get wall rooms
        List<Room> wallRooms = GetRooms(TileType.WALL);

        //remove too small wall rooms
        foreach(Room room in wallRooms)
        {
            if(room.GetTiles().Count < wallRoomThreshold)
            {
                Debug.Log("Remove wall room");
                RemoveRoom(room);
            }
        }

        //get floor rooms
        List<Room> floorRooms = GetRooms(TileType.FLOOR);

        //remove too small floor rooms
        foreach (Room room in floorRooms)
        {
            if (room.GetTiles().Count < floorRoomThreshold)
            {
                Debug.Log("Remove floor room");
                RemoveRoom(room);
            }
        }
    }

    /// <summary>
    ///     This function returns all rooms of the given type
    /// </summary>
    /// <param name="roomType">The type of the rooms</param>
    /// <returns>A list containing all rooms of the given type</returns>
    private List<Room> GetRooms(TileType roomType)
    {
        List<Room> rooms = new List<Room>();
        bool[,] flaggedTiles = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if(!flaggedTiles[x,y] && GetTileType(x,y) == roomType)
                {
                    //tile has correct type and was not visited yet

                    //get and add room
                    Room newRoom = GetRoomOfPosition(x, y);
                    rooms.Add(newRoom);

                    //mark all tiles as visited
                    foreach(Vector2Int tile in newRoom.GetTiles())
                    {
                        flaggedTiles[tile.x, tile.y] = true;
                    }
                }
            }
        }

        return rooms;
    }

    /// <summary>
    ///     This function return the room for the given position
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>The room the position is in</returns>
    private Room GetRoomOfPosition(int posX, int posY)
    {
        //get room type
        TileType roomType;
        if(layout[posX, posY])
        {
            roomType = TileType.FLOOR;
        }
        else
        {
            roomType = TileType.WALL;
        }
        
        //setup tile lists and flag grid
        List<Vector2Int> tiles = new List<Vector2Int>();
        List<Vector2Int> borderTiles = new List<Vector2Int>();
        bool[,] flaggedTiles = new bool[size.x, size.y];

        //setup queue
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(posX, posY));
        flaggedTiles[posX, posY] = true;

        //flood fill
        while(queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            //list of transition to neighbors
            List<Vector2Int> nextNeighbor = new List<Vector2Int>();
            nextNeighbor.Add(new Vector2Int(-1, 0));
            nextNeighbor.Add(new Vector2Int(0, 1));
            nextNeighbor.Add(new Vector2Int(1, 0));
            nextNeighbor.Add(new Vector2Int(0, -1));

            //check neighbors
            for(int i=0; i<4; i++)
            {
                //get neighbor
                Vector2Int neighbor = tile + nextNeighbor[i];

                if (PositionIsInRange(neighbor.x, neighbor.y) && !flaggedTiles[neighbor.x, neighbor.y])
                {
                    //neighbor is inside the grid and was not visited
                    if(GetTileType(neighbor.x, neighbor.y) == roomType)
                    {
                        //neighbor is same type
                        flaggedTiles[neighbor.x, neighbor.y] = true;
                        queue.Enqueue(neighbor);
                    }
                    else
                    {
                        //neighbor is different type -> tile is border
                        borderTiles.Add(tile);
                    }
                }
            }
        }

        //create room
        Room room = new Room(roomType);
        room.SetTiles(tiles);
        room.SetBorderTiles(borderTiles);

        return room;
    }

    /// <summary>
    ///     This function returns the tile type of the given position
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>The type of the tile</returns>
    private TileType GetTileType(int posX, int posY)
    {
        if(layout[posX, posY])
        {
            return TileType.FLOOR;
        }
        else
        {
            return TileType.WALL;
        }
    }

    /// <summary>
    ///     This function removes the given rooms by changing the type of all tiles
    /// </summary>
    /// <param name="room">The room to remove</param>
    private void RemoveRoom(Room room)
    {
        TileType type = room.GetRoomType();
        foreach(Vector2Int tile in room.GetTiles())
        {
            if(type == TileType.FLOOR)
            {
                layout[tile.x, tile.y] = false;
            }
            else
            {
                layout[tile.x, tile.y] = true;
            }
        }
    }

    #endregion

    #region Connections

    //connect different rooms

    // 1. define master room (biggest one)

    // 2. loop, until all rooms connected (#rooms - 1 times)

    //   2.1 identify for each room clostest other room

    //   2.2 connect clostest two rooms with one not connected to master room (adds one new room)

    #endregion

    #region General

    /// <summary>
    ///     This function checks, whether a given position is inside of the layout size
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>True, if in range, false otherwise</returns>
    private bool PositionIsInRange(int posX, int posY)
    {
        return (posX >= 0 && posX < size.x && posY >= 0 && posY < size.y);
    }

    #endregion
}
