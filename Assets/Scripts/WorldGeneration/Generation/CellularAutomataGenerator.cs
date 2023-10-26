using System;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataGenerator : LayoutGenerator
{
    private static readonly int borderThickness = 3;
    private static readonly int worldConnectionWidth = 1;
    private static readonly int corridorThickness = 4;

    private static readonly int iterations = 15;
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

        //Remove small floor and wall rooms
        List<Room> floorRooms = RemoveSmallRooms();

        //add world connections, if present
        if(worldConnections.Count > 0)
        {
            AddWorldConnections();

            List<Room> worldConnectionRooms = new List<Room>();

            //size of world connection space in border
            int isolatedWorldConnectionSize = borderThickness * (1 + 2 * worldConnectionWidth);

            foreach(WorldConnection worldConnection in worldConnections)
            {
                Room room = GetRoomOfPosition(worldConnection.GetPosition().x, worldConnection.GetPosition().y);

                if(room.GetTiles().Count == isolatedWorldConnectionSize)
                {
                    worldConnectionRooms.Add(room);
                }
            }

            floorRooms.AddRange(worldConnectionRooms);
        }

        //Connect remaining floor rooms
        ConnectRooms(floorRooms);

        //remove unwanted structures
        PostProcess();
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
            for (int y = 0; y < borderThickness; y++)
            {
                layout[x, y] = false;
            }
            for (int y = size.y - borderThickness; y < size.y; y++)
            {
                layout[x, y] = false;
            }
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < borderThickness; x++)
            {
                layout[x, y] = false;
            }
            for (int x = size.x - borderThickness; x < size.x; x++)
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

        for (int x = borderThickness; x < size.x-borderThickness; x++)
        {
            for (int y = borderThickness; y < size.y - borderThickness; y++)
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

        for (int x = borderThickness; x < size.x-borderThickness; x++)
        {
            for(int y=borderThickness; y < size.y-borderThickness; y++)
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

    /// <summary>
    ///     This function creates the world connections in the border
    /// </summary>
    private void AddWorldConnections()
    {
        foreach (WorldConnection worldConnection in worldConnections)
        {
            //connection on left side
            if (worldConnection.GetPosition().x == 0 && worldConnection.GetPosition().y > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().y < size.y - (borderThickness + worldConnectionWidth))
            {
                for (int x = 0; x < borderThickness; x++)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[x, worldConnection.GetPosition().y + offset] = true;
                    }
                }
            }

            //connection on bottom side
            if (worldConnection.GetPosition().y == 0 && worldConnection.GetPosition().x > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().x < size.x - (borderThickness + worldConnectionWidth))
            {
                for (int y = 0; y < borderThickness; y++)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[worldConnection.GetPosition().x + offset, y] = true;
                    }
                }
            }

            //connection on right side
            if (worldConnection.GetPosition().x == size.x-1 && worldConnection.GetPosition().y > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().y < size.y - (borderThickness + worldConnectionWidth))
            {
                for (int x = size.x - 1; x > size.x - 1 - borderThickness; x--)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[x, worldConnection.GetPosition().y + offset] = true;
                    }
                }
            }

            //connection on top side
            if (worldConnection.GetPosition().y == size.y-1 && worldConnection.GetPosition().x > (borderThickness + worldConnectionWidth) && worldConnection.GetPosition().x < size.x - (borderThickness + worldConnectionWidth))
            {
                for (int y = size.y - 1; y > size.y - 1 - borderThickness; y--)
                {
                    for (int offset = -worldConnectionWidth; offset <= worldConnectionWidth; offset++)
                    {
                        layout[worldConnection.GetPosition().x + offset, y] = true;
                    }
                }
            }
        }
    }

    #endregion

    #region Rooms

    /// <summary>
    ///     This function removes all rooms with size below the threshold
    /// </summary>
    /// <returns>A list containing all floor rooms</returns>
    private List<Room> RemoveSmallRooms()
    {
        //get wall rooms
        List<Room> wallRooms = GetRooms(TileType.WALL);

        //remove too small wall rooms
        foreach(Room room in wallRooms)
        {
            if(room.GetTiles().Count < wallRoomThreshold)
            {
                RemoveRoom(room);
            }
        }

        //get floor rooms
        List<Room> floorRooms = GetRooms(TileType.FLOOR);

        //remove too small floor rooms
        List<Room> remainingFloorRoomes = new List<Room>();
        foreach (Room room in floorRooms)
        {
            if (room.GetTiles().Count < floorRoomThreshold)
            {
                RemoveRoom(room);
            }
            else
            {
                remainingFloorRoomes.Add(room);
            }
        }

        return remainingFloorRoomes;
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

        for (int x = borderThickness; x < size.x - borderThickness; x++)
        {
            for (int y = borderThickness; y < size.y - borderThickness; y++)
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

                if (IsInRange(neighbor.x, neighbor.y) && !flaggedTiles[neighbor.x, neighbor.y])
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

    /// <summary>
    ///     This function connects all the rooms
    /// </summary>
    /// <param name="floorRooms">A list containing all rooms to connect</param>
    private void ConnectRooms(List<Room> floorRooms)
    {
        List<Room> unconnectedRooms = new List<Room>(floorRooms);
        List<Room> connectedRooms = new List<Room>();

        //identify biggest room (master room)
        int maxRoomSize = 0;
        Room biggestRoom = new Room(TileType.FLOOR);

        foreach(Room room in floorRooms)
        {
            if(room.GetTiles().Count > maxRoomSize)
            {
                maxRoomSize = room.GetTiles().Count;
                biggestRoom = room;
            }
        }
        
        unconnectedRooms.Remove(biggestRoom);
        connectedRooms.Add(biggestRoom);

        //init closest rooms dictionary
        Dictionary<Room, PossibleRoomConnection> closestRooms = new Dictionary<Room, PossibleRoomConnection>();

        while (unconnectedRooms.Count > 0)
        {
            float minDistance = float.MaxValue;
            Room roomToConnect = biggestRoom;

            foreach(Room room in connectedRooms)
            {
                if(!closestRooms.ContainsKey(room) || !unconnectedRooms.Contains(closestRooms[room].GetDestinationRoom()))
                {
                    //no closest room yet or closest one is already connected

                    PossibleRoomConnection possibleRoomConnection = GetClosestRoom(room, unconnectedRooms);
                    
                    if(closestRooms.ContainsKey(room))
                    {
                        closestRooms[room] = possibleRoomConnection;
                    }
                    else
                    {
                        closestRooms.Add(room, possibleRoomConnection);
                    }                    
                }

                if(closestRooms[room].GetDistance() < minDistance)
                {
                    //connection is best for now

                    minDistance = closestRooms[room].GetDistance();
                    roomToConnect = room;
                }
            }

            //connect closest two rooms
            CreateConnection(closestRooms[roomToConnect]);

            //move newly connected room to connect rooms list
            connectedRooms.Add(closestRooms[roomToConnect].GetDestinationRoom());
            unconnectedRooms.Remove(closestRooms[roomToConnect].GetDestinationRoom());
        }
    }

    /// <summary>
    ///     This function finds the closest room in the given list for the given room
    /// </summary>
    /// <param name="room">The room to find the closest other room to</param>
    /// <param name="connectedRooms">The possible rooms</param>
    /// <returns>A <c>PosibleRoomConnection</c> object containing the information about the closest room</returns>
    private PossibleRoomConnection GetClosestRoom(Room room, List<Room> unconnectedRooms)
    {
        Room destinationRoom = room;
        Vector2Int startTile = new Vector2Int();
        Vector2Int destinationTile = new Vector2Int();
        float minDistance = float.MaxValue;

        foreach(Room otherRoom in unconnectedRooms)
        {
            foreach(Vector2Int borderDestinationTile in otherRoom.GetBorderTiles())
            {
                foreach(Vector2Int borderStartTile in room.GetBorderTiles())
                {
                    float distance = Vector2Int.Distance(borderStartTile, borderDestinationTile);

                    if(distance < minDistance)
                    {
                        minDistance = distance;
                        startTile = borderStartTile;
                        destinationTile = borderDestinationTile;
                        destinationRoom = otherRoom;
                    }
                }
            }
        }

        PossibleRoomConnection roomConnection = new PossibleRoomConnection(room, destinationRoom, startTile, destinationTile, minDistance);
        return roomConnection;
    }

    /// <summary>
    ///     This function connects the given rooms at the provided tiles
    /// </summary>
    /// <param name="roomConnection">The rooms to connect</param>
    private void CreateConnection(PossibleRoomConnection roomConnection)
    {
        List<Vector2Int> connectionTiles = GetConnectionTiles(roomConnection.GetStartTile(), roomConnection.GetDestinationTile());
        foreach(Vector2Int tile in connectionTiles)
        {
            DrawCircle(tile, corridorThickness);
        }
    }

    /// <summary>
    ///     This function creates a list of tiles connecting the two given tiles
    /// </summary>
    /// <param name="startTile">The start position</param>
    /// <param name="destinationTile">The end position</param>
    /// <returns>A list containing all tiles to connect the start and end tile</returns>
    private List<Vector2Int> GetConnectionTiles(Vector2Int startTile, Vector2Int destinationTile)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        //variables for current tile position
        int posX = startTile.x;
        int posY = startTile.y;

        //triangle sides
        int dx = destinationTile.x - startTile.x;
        int dy = destinationTile.y - startTile.y;

        //variables for changes each step
        bool inverted;
        int step;
        int gradientStep;
        int longest;
        int shortest;

        if (Mathf.Abs(dx) < Mathf.Abs(dy))
        {
            //slope > 45 degrees
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }
        else
        {
            //slope < 45 degrees
            inverted = false;
            longest = Mathf.Abs(dx);
            shortest = Mathf.Abs(dy);
            step = Math.Sign(dx);
            gradientStep = Math.Sign(dy);
        }

        int gradientAccumulation = longest / 2;

        for(int i=0; i<longest; i++)
        {
            //add tile
            tiles.Add(new Vector2Int(posX, posY));

            if(inverted)
            {
                //next tile in y direction
                posY += step;
            }
            else
            {
                //next tile in x direction
                posX += step;
            }

            //change other direction, if full tile up
            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest)
            {
                if(inverted)
                {
                    posX += gradientStep;
                }
                else
                {
                    posY += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        //add destination tile
        tiles.Add(destinationTile);

        return tiles;
    }

    /// <summary>
    ///     This function marks the given position and all tiles within a circle around it as floor tiles
    /// </summary>
    /// <param name="position">The center position</param>
    /// <param name="radius">The radius of the circle</param>
    private void DrawCircle(Vector2Int position, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if(x*x + y*y <= radius*radius)
                {
                    int tilePosX = position.x + x;
                    int tilePosY = position.y + y;

                    if(IsInCenter(tilePosX, tilePosY))
                    {
                        layout[tilePosX, tilePosY] = true;
                    }
                }
            }
        }
    }

    #endregion

    #region Post Processing

    private void PostProcess()
    {
        bool changedSomething = true;
        
        while(changedSomething)
        {
            changedSomething = false;

            for (int x = borderThickness; x < size.x - borderThickness; x++)
            {
                for (int y = borderThickness; y < size.y - borderThickness; y++)
                {
                    if (GetTileType(x, y) == TileType.WALL)
                    {
                        if(!ValidWallTile(x,y))
                        {
                            layout[x, y] = true;
                            changedSomething = true;
                        }
                    }
                }
            }
        }
    }

    public bool ValidWallTile(int x, int y)
    {
        bool horizontalWall = (IsInRange(x - 1, y) && GetTileType(x - 1, y) == TileType.WALL) || (IsInRange(x + 1, y) && GetTileType(x + 1, y) == TileType.WALL);
        bool verticalWall = (IsInRange(x, y + 1) && GetTileType(x, y + 1) == TileType.WALL) || (IsInRange(x, y - 1) && GetTileType(x, y - 1) == TileType.WALL);

        return horizontalWall && verticalWall;
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

    /// <summary>
    ///     This function checks, whether a given position is inside of the layout center, so inside the border
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>True, if in range, false otherwise</returns>
    private bool IsInCenter(int posX, int posY)
    {
        return (posX >= borderThickness && posX < size.x-borderThickness && posY >= borderThickness && posY < size.y - borderThickness);
    }

    #endregion
}
