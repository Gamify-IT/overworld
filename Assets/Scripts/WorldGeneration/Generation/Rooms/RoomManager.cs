using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager
{
    private int borderSize;
    private int worldConnectionWidth;
    private int corridorSize;

    private CellType[,] layout;
    private Vector2Int size;
    private string seed;
    System.Random pseudoRandomNumberGenerator;

    public RoomManager(CellType[,] layout, string seed)
    {
        this.layout = layout;
        size = new Vector2Int(layout.GetLength(0), layout.GetLength(1));
        this.seed = seed;
        pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());
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
        worldConnectionWidth = generationSettings.worldConnectionWidth;
        corridorSize = generationSettings.corridorSize;
    }

    /// <summary>
    ///     This function returns the changed layout
    /// </summary>
    /// <returns>The changed layout</returns>
    public CellType[,] GetLayout()
    {
        return layout;
    }

    #region Rooms

    /// <summary>
    ///     This function removes all rooms of the given type with size below the given threshold
    /// </summary>
    /// <param name="roomType">The type of rooms to check</param>
    /// <param name="threshold">The minimum room size</param>
    public void RemoveSmallRooms(CellType roomType, int threshold)
    {
        //get rooms
        List<Room> rooms = GetRooms(roomType);

        //remove too small rooms
        foreach (Room room in rooms)
        {
            if (room.GetTiles().Count < threshold)
            {
                RemoveRoom(room);
            }
        }
    }

    /// <summary>
    ///     This function returns all rooms of the given type
    /// </summary>
    /// <param name="roomType">The type of the rooms</param>
    /// <returns>A list containing all rooms of the given type</returns>
    private List<Room> GetRooms(CellType roomType)
    {
        List<Room> rooms = new List<Room>();
        bool[,] flaggedTiles = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (!flaggedTiles[x, y] && layout[x, y] == roomType)
                {
                    //tile has correct type and was not visited yet

                    //get and add room
                    Room newRoom = GetRoomOfPosition(x, y);
                    rooms.Add(newRoom);

                    //mark all tiles as visited
                    foreach (Vector2Int tile in newRoom.GetTiles())
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
        CellType roomType = layout[posX, posY];

        //setup tile lists and flag grid
        List<Vector2Int> tiles = new List<Vector2Int>();
        List<Vector2Int> borderTiles = new List<Vector2Int>();
        bool[,] flaggedTiles = new bool[size.x, size.y];

        //setup queue
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(posX, posY));
        flaggedTiles[posX, posY] = true;

        //flood fill
        while (queue.Count > 0)
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
            for (int i = 0; i < 4; i++)
            {
                //get neighbor
                Vector2Int neighbor = tile + nextNeighbor[i];

                if (IsInRange(neighbor.x, neighbor.y) && !flaggedTiles[neighbor.x, neighbor.y])
                {
                    //neighbor is inside the grid and was not visited
                    if(layout[neighbor.x, neighbor.y] == roomType)
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
    ///     This function removes the given rooms by changing the type of all tiles
    /// </summary>
    /// <param name="room">The room to remove</param>
    private void RemoveRoom(Room room)
    {
        CellType type = room.GetRoomType();
        foreach (Vector2Int tile in room.GetTiles())
        {
            if (type == CellType.FLOOR)
            {
                layout[tile.x, tile.y] = CellType.WALL;
            }
            else
            {
                layout[tile.x, tile.y] = CellType.FLOOR;
            }
        }
    }

    #endregion

    #region WorldConnections

    /// <summary>
    ///     This function creates the world connections in the border
    /// </summary>
    /// <param name="worldConnections">The world connections to add</param>
    public void AddWorldConnections(List<WorldConnection> worldConnections)
    {
        foreach(WorldConnection worldConnection in worldConnections)
        {
            AddWorldConnection(worldConnection);
        }
    }

    /// <summary>
    ///     This function creates a world connection in the border
    /// </summary>
    /// <param name="worldConnection">The world connection to add</param>
    private void AddWorldConnection(WorldConnection worldConnection)
    {
        //connection on left side
        if (worldConnection.GetPosition().x == 0 && worldConnection.GetPosition().y > (borderSize + worldConnectionWidth) && worldConnection.GetPosition().y < size.y - (borderSize + worldConnectionWidth))
        {
            for (int x = 0; x < borderSize + corridorSize; x++)
            {
                for (int offset = -worldConnectionWidth/2; offset <= worldConnectionWidth/2; offset++)
                {
                    layout[x, worldConnection.GetPosition().y + offset] = CellType.FLOOR;
                }
            }
        }

        //connection on bottom side
        if (worldConnection.GetPosition().y == 0 && worldConnection.GetPosition().x > (borderSize + worldConnectionWidth) && worldConnection.GetPosition().x < size.x - (borderSize + worldConnectionWidth))
        {
            for (int y = 0; y < borderSize + corridorSize; y++)
            {
                for (int offset = -worldConnectionWidth/2; offset <= worldConnectionWidth/2; offset++)
                {
                    layout[worldConnection.GetPosition().x + offset, y] = CellType.FLOOR;
                }
            }
        }

        //connection on right side
        if (worldConnection.GetPosition().x == size.x - 1 && worldConnection.GetPosition().y > (borderSize + worldConnectionWidth) && worldConnection.GetPosition().y < size.y - (borderSize + worldConnectionWidth))
        {
            for (int x = size.x - 1; x > size.x - 1 - (borderSize + corridorSize); x--)
            {
                for (int offset = -worldConnectionWidth/2; offset <= worldConnectionWidth/2; offset++)
                {
                    layout[x, worldConnection.GetPosition().y + offset] = CellType.FLOOR;
                }
            }
        }

        //connection on top side
        if (worldConnection.GetPosition().y == size.y - 1 && worldConnection.GetPosition().x > (borderSize + worldConnectionWidth) && worldConnection.GetPosition().x < size.x - (borderSize + worldConnectionWidth))
        {
            for (int y = size.y - 1; y > size.y - 1 - (borderSize + corridorSize); y--)
            {
                for (int offset = -worldConnectionWidth/2; offset <= worldConnectionWidth/2; offset++)
                {
                    layout[worldConnection.GetPosition().x + offset, y] = CellType.FLOOR;
                }
            }
        }
    }

    #endregion

    #region Connections

    /// <summary>
    ///     This function connects all the rooms
    /// </summary>
    /// <param name="corridorSize">The size of the connecting corridors</param>
    public void ConnectRooms(int corridorSize)
    {
        List<Room> unconnectedRooms = GetRooms(CellType.FLOOR);
        List<Room> connectedRooms = new List<Room>();

        //identify biggest room (master room)
        int maxRoomSize = 0;
        Room biggestRoom = new Room(CellType.FLOOR);

        foreach (Room room in unconnectedRooms)
        {
            if (room.GetTiles().Count > maxRoomSize)
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

            foreach (Room room in connectedRooms)
            {
                if (!closestRooms.ContainsKey(room) || !unconnectedRooms.Contains(closestRooms[room].GetDestinationRoom()))
                {
                    //no closest room yet or closest one is already connected

                    PossibleRoomConnection possibleRoomConnection = GetClosestRoom(room, unconnectedRooms);

                    if (closestRooms.ContainsKey(room))
                    {
                        closestRooms[room] = possibleRoomConnection;
                    }
                    else
                    {
                        closestRooms.Add(room, possibleRoomConnection);
                    }
                }

                if (closestRooms[room].GetDistance() < minDistance)
                {
                    //connection is best for now

                    minDistance = closestRooms[room].GetDistance();
                    roomToConnect = room;
                }
            }

            //connect closest two rooms
            CreateConnection(closestRooms[roomToConnect], corridorSize);

            //move newly connected room to connect rooms list
            connectedRooms.Add(closestRooms[roomToConnect].GetDestinationRoom());
            unconnectedRooms.Remove(closestRooms[roomToConnect].GetDestinationRoom());
        }
    }

    /// <summary>
    ///     This function finds the closest room in the given list for the given room
    /// </summary>
    /// <param name="room">The room to find the closest other room to</param>
    /// <param name="unconnectedRooms">The possible rooms</param>
    /// <returns>A <c>PosibleRoomConnection</c> object containing the information about the closest room</returns>
    private PossibleRoomConnection GetClosestRoom(Room room, List<Room> unconnectedRooms)
    {
        float minDistance = float.MaxValue;

        List<PossibleRoomConnection> shortestConnections = new List<PossibleRoomConnection>();

        foreach (Room otherRoom in unconnectedRooms)
        {
            foreach (Vector2Int borderDestinationTile in otherRoom.GetBorderTiles())
            {
                foreach (Vector2Int borderStartTile in room.GetBorderTiles())
                {
                    float distance = Vector2Int.Distance(borderStartTile, borderDestinationTile);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        PossibleRoomConnection shortestConnection = new PossibleRoomConnection(room, otherRoom, borderStartTile, borderDestinationTile, distance);
                        shortestConnections = new List<PossibleRoomConnection> { shortestConnection };

                    }
                    else if(distance == minDistance)
                    {
                        PossibleRoomConnection shortestConnection = new PossibleRoomConnection(room, otherRoom, borderStartTile, borderDestinationTile, distance);
                        shortestConnections.Add(shortestConnection);
                    }
                }
            }
        }

        int connectionIndex = pseudoRandomNumberGenerator.Next(0, shortestConnections.Count);
        return shortestConnections[connectionIndex];
    }

    /// <summary>
    ///     This function connects the given rooms at the provided tiles
    /// </summary>
    /// <param name="roomConnection">The rooms to connect</param>
    /// <param name="corridorSize">The size of the connecting corridors</param>
    private void CreateConnection(PossibleRoomConnection roomConnection, int corridorSize)
    {
        List<Vector2Int> connectionTiles = GetConnectionTiles(roomConnection.GetStartTile(), roomConnection.GetDestinationTile());
        foreach (Vector2Int tile in connectionTiles)
        {
            DrawCircle(tile, corridorSize);
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

        for (int i = 0; i < longest; i++)
        {
            //add tile
            tiles.Add(new Vector2Int(posX, posY));

            if (inverted)
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
            if (gradientAccumulation >= longest)
            {
                if (inverted)
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
                if (x * x + y * y <= radius * radius)
                {
                    int tilePosX = position.x + x;
                    int tilePosY = position.y + y;

                    if (IsInCenter(tilePosX, tilePosY))
                    {
                        if(layout[tilePosX, tilePosY] == CellType.WALL)
                        {
                            layout[tilePosX, tilePosY] = CellType.CORRIDOR;
                        }
                    }
                }
            }
        }
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
        return (posX >= borderSize && posX < size.x - borderSize && posY >= borderSize && posY < size.y - borderSize);
    }

    #endregion
}
