using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to generate object positions
/// </summary>
public class ObjectPositionGenerator
{
    #region Constants
    //Iterations per search spot
    private static readonly int maxIterations = 20;

    //Minimum distance from dungeon spot to world connections
    private static readonly int minDungeonDistance = 75;

    //Minimum distance to objects of other types
    private static readonly int minObjectDistance = 10;

    //Minimum distance to objects of same type
    private static readonly int minSameObjectDistance = 20;

    //Radius for npcs and books around minigames
    private static readonly int minigameRadius = 20;
    #endregion

    private Pathfinder pathfinder;

    private Vector2Int size;
    private List<Vector2Int> objectPositions;
    private List<Vector2Int> dungeonWallPositions;
    private CellType[,] tileType;
    private WorldStyle style;

    private List<WorldConnection> worldConnections;
    private List<Vector2Int> minigamePositions;
    private List<Vector2Int> npcPositions;
    private List<Vector2Int> bookPositions;
    private List<Vector2Int> teleporterPositions;

    private List<DungeonSpotPosition> dungeonPositions;
    private List<BarrierSpotPosition> barrierPositions;

    #region Constructor

    public ObjectPositionGenerator(CellType[,] tileType, List<WorldConnection> worldConnections, WorldStyle style)
    {
        size = new Vector2Int(tileType.GetLength(0), tileType.GetLength(1));
        objectPositions = new List<Vector2Int>();
        dungeonWallPositions = new List<Vector2Int>();            
        GetObjectPositions(tileType);
        this.tileType = tileType;
        this.style = style;

        this.worldConnections = worldConnections;
        minigamePositions = new List<Vector2Int>();
        npcPositions = new List<Vector2Int>();
        bookPositions = new List<Vector2Int>();
        teleporterPositions = new List<Vector2Int>();
        dungeonPositions = new List<DungeonSpotPosition>();

        bool[,] accessableTiles = GetAccessableTiles(tileType);
        pathfinder = new Pathfinder(accessableTiles);
    }

    /// <summary>
    ///     This function gets the accessable tiles of a given cell type layout
    /// </summary>
    /// <param name="tiles">The layout</param>
    /// <returns>All accessable tiles</returns>
    private bool[,] GetAccessableTiles(CellType[,] tiles)
    {
        bool[,] accessableTiles = new bool[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if(tiles[x, y] != CellType.WALL)
                {
                    accessableTiles[x, y] = true;
                }
            }
        }
        return accessableTiles;
    }

    /// <summary>
    ///     This function fills the lists containig all positions an object can be placed at
    /// </summary>
    /// <param name="tiles">The tile types of the layout</param>
    private void GetObjectPositions(CellType[,] tiles)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if(tiles[x, y] == CellType.FLOOR)
                {
                    objectPositions.Add(new Vector2Int(x, y));
                }
                else if(tiles[x, y] == CellType.WALL)
                {
                    if(IsInRange(x, y-1) && tiles[x, y-1] == CellType.FLOOR)
                    {
                        //bottom tile of wall
                        dungeonWallPositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }

    #endregion

    #region Generate Functions
    public void ResetObjects()
    {
        minigamePositions = new List<Vector2Int>();
        npcPositions = new List<Vector2Int>();
        bookPositions = new List<Vector2Int>();
        teleporterPositions = new List<Vector2Int>();
        dungeonPositions = new List<DungeonSpotPosition>();
        barrierPositions = new List<BarrierSpotPosition>();
    }

    /// <summary>
    ///     This function generates positions for minigame spots
    /// </summary>
    /// <param name="amount">The amount of minigame spots to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateMinigamePositions(int amount)
    {
        //reset current spots
        minigamePositions = new List<Vector2Int>();

        bool success = true;

        //generate new spots
        for (int i=0; i<amount; i++)
        {
            bool spotCreated = false;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                Vector2Int position = GetPosition();

                bool objectToClose = ObjectToClose(position, minSameObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance);

                if (!objectToClose)
                {
                    //position found                    
                    minigamePositions.Add(position);
                    spotCreated = true;
                    break;
                }
            }   
            
            if(!spotCreated)
            {
                //no position found
                success = false;
                break;
            }
        }

        if(!success)
        {
            minigamePositions = new List<Vector2Int>();
        }

        return success;
    }

    /// <summary>
    ///     This function generates positions for npc spots
    /// </summary>
    /// <param name="amount">The amount of npc spots to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateNpcPositions(int amount)
    {
        //reset current spots
        npcPositions = new List<Vector2Int>();

        bool success = true;

        //generate new spots
        for (int i = 0; i < amount; i++)
        {
            bool spotCreated = false;

            if(minigamePositions.Count > i)
            {
                Vector2Int minigamePosition = minigamePositions[i];

                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    Vector2Int position = GetPositionInRange(minigamePosition, minigameRadius);

                    bool objectToClose = ObjectToClose(position, minObjectDistance, minSameObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance);

                    //check if actual distance is valid
                    bool distanceOk = false;

                    Optional<List<Vector2Int>> path = pathfinder.FindPath(minigamePosition, position);
                    if(path.IsPresent())
                    {
                        int distance = pathfinder.GetDistance(path.Value());

                        distanceOk = (distance <= minigameRadius);
                    }                    

                    if (!objectToClose && distanceOk)
                    {
                        //position found 
                        npcPositions.Add(position);
                        spotCreated = true;
                        break;
                    }
                }

                if (!spotCreated)
                {
                    //no position found
                    success = false;
                    break;
                }
            }
            else
            {
                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    Vector2Int position = GetPosition();

                    bool objectToClose = ObjectToClose(position, minObjectDistance, minSameObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance);

                    if (!objectToClose)
                    {
                        //position found 
                        npcPositions.Add(position);
                        spotCreated = true;
                        break;
                    }
                }

                if (!spotCreated)
                {
                    //no position found
                    success = false;
                    break;
                }
            }
        }

        if (!success)
        {
            npcPositions = new List<Vector2Int>();
        }

        return success;
    }

    /// <summary>
    ///     This function generates positions for book spots
    /// </summary>
    /// <param name="amount">The amount of book spots to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateBookPositions(int amount)
    {
        //reset current spots
        bookPositions = new List<Vector2Int>();

        bool success = true;

        //generate new spots
        for (int i = 0; i < amount; i++)
        {
            bool spotCreated = false;

            if (minigamePositions.Count > i)
            {
                Vector2Int minigamePosition = minigamePositions[i];

                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    Vector2Int position = GetPositionInRange(minigamePosition, minigameRadius);

                    bool objectToClose = ObjectToClose(position, minObjectDistance, minObjectDistance, minSameObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance);

                    //check if actual distance is valid
                    bool distanceOk = false;

                    Optional<List<Vector2Int>> path = pathfinder.FindPath(minigamePosition, position);
                    if (path.IsPresent())
                    {
                        int distance = pathfinder.GetDistance(path.Value());

                        distanceOk = (distance <= minigameRadius);
                    }

                    if (!objectToClose && distanceOk)
                    {
                        //position found 
                        bookPositions.Add(position);
                        spotCreated = true;
                        break;
                    }
                }

                if (!spotCreated)
                {
                    //no position found
                    success = false;
                    break;
                }
            }
            else
            {
                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    Vector2Int position = GetPosition();

                    bool objectToClose = ObjectToClose(position, minObjectDistance, minObjectDistance, minSameObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance);

                    if (!objectToClose)
                    {
                        //position found 
                        bookPositions.Add(position);
                        spotCreated = true;
                        break;
                    }
                }

                if (!spotCreated)
                {
                    //no position found
                    success = false;
                    break;
                }
            }
        }

        if (!success)
        {
            bookPositions = new List<Vector2Int>();
        }

        return success;
    }

    /// <summary>
    ///     This function generates positions for teleporter spots
    /// </summary>
    /// <param name="amount">The amount of teleporter spots to generate</param>
    public bool GenerateTeleporterPositions(int amount)
    {
        //reset current spots
        teleporterPositions = new List<Vector2Int>();

        bool success = true;

        //generate new spots
        for (int i = 0; i < amount; i++)
        {
            bool spotCreated = false;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                Vector2Int position = GetPosition();

                bool objectToClose = ObjectToClose(position, minObjectDistance, minObjectDistance, minObjectDistance, minSameObjectDistance, minObjectDistance, minObjectDistance);

                if (!objectToClose)
                {
                    //position found                    
                    teleporterPositions.Add(position);
                    spotCreated = true;
                    break;
                }
            }

            if (!spotCreated)
            {
                //no position found
                success = false;
                break;
            }
        }

        if (!success)
        {
            teleporterPositions = new List<Vector2Int>();
        }

        return success;
    }

    /// <summary>
    ///     This function generates positions for dungeon spots
    /// </summary>
    /// <param name="amount">The amount of dungeon spots to generate</param>
    public bool GenerateDungeonPositions(int amount)
    {
        //reset current spots
        dungeonPositions = new List<DungeonSpotPosition>();

        bool success = true;

        //generate new spots
        for (int i = 0; i < amount; i++)
        {
            bool spotCreated = false;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                DungeonStyle dungeonStyle = GetDungeonStyle();
                Vector2Int position = GetDungeonPosition(dungeonStyle);

                bool objectToClose = ObjectToClose(position, minObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance, minSameObjectDistance, minDungeonDistance);
                bool validDungeonPosition = IsValidDungeonPosition(position, dungeonStyle);

                if(!objectToClose && validDungeonPosition)
                {
                    Debug.Log("Dungeon spot found, type: " + dungeonStyle);

                    //position found
                    DungeonSpotPosition dungeonSpot = new DungeonSpotPosition(position, dungeonStyle);
                    dungeonPositions.Add(dungeonSpot);
                    spotCreated = true;
                    break;
                }
            }

            if (!spotCreated)
            {
                //no position found
                success = false;
                break;
            }
        }

        if (!success)
        {
            dungeonPositions = new List<DungeonSpotPosition>();
        }

        return success;
    }

    //create world connection barriers
    public List<BarrierSpotPosition> GetWorldBarrierSpots()
    {
        List<BarrierSpotPosition> barriers = new List<BarrierSpotPosition>();

        foreach(WorldConnection worldConnection in worldConnections)
        {
            Debug.Log("Create world connection barrier for world: " + worldConnection.GetDestinationWorld());

            BarrierSpotPosition barrier = new BarrierSpotPosition(worldConnection.GetPosition(), BarrierStyle.TREE, worldConnection.GetDestinationWorld());
            barriers.Add(barrier);
        }

        return barriers;
    }

    #region Dungeon Helper Functions

    //get random dungeon entrance style
    private DungeonStyle GetDungeonStyle()
    {
        if(style == WorldStyle.CAVE)
        {
            //gate or cave entrance
            if(Random.Range(0f, 1f) < 0.5f)
            {
                return DungeonStyle.GATE;
            }
            else
            {
                return DungeonStyle.CAVE_ENTRANCE;
            }
        }
        else
        {
            //house or trapdoor
            if (Random.Range(0f, 1f) < 0.5f)
            {
                return DungeonStyle.HOUSE;
            }
            else
            {
                return DungeonStyle.TRAPDOOR;
            }
        }
    }

    //get position for dungeon spot
    private Vector2Int GetDungeonPosition(DungeonStyle style)
    {
        switch(style)
        {
            case DungeonStyle.HOUSE:
            case DungeonStyle.TRAPDOOR:
                return GetPosition();

            case DungeonStyle.GATE:
            case DungeonStyle.CAVE_ENTRANCE:
                return GetWallPosition();
        }

        return new Vector2Int();
    }

    //check if valid dungeon position
    private bool IsValidDungeonPosition(Vector2Int position, DungeonStyle style)
    {
        switch (style)
        {
            case DungeonStyle.HOUSE:
                return ValidHousePosition(position);

            case DungeonStyle.TRAPDOOR:
                return ValidTrapdoorPosition(position);

            case DungeonStyle.GATE:
                return ValidGatePosition(position);

            case DungeonStyle.CAVE_ENTRANCE:
                return ValidCavePosition(position);
        }

        return false;
    }

    //check if position valid for trapdoor dungeon
    private bool ValidTrapdoorPosition(Vector2Int position)
    { 
        for(int x = position.x - 1; x <= position.x + 2; x++)
        {
            for(int y = position.y - 1; y < position.y + 2; y++)
            {
                if(!IsInRange(x, y) || tileType[x, y] != CellType.FLOOR)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //check if position valid for house dungeon
    private bool ValidHousePosition(Vector2Int position)
    {
        for (int x = position.x - 1; x <= position.x + 5; x++)
        {
            for (int y = position.y - 1; y < position.y + 5; y++)
            {
                if (!IsInRange(x, y) || tileType[x, y] != CellType.FLOOR)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //check if position valid for gate dungeon
    private bool ValidGatePosition(Vector2Int position)
    {
        for (int x = position.x; x <= position.x + 3; x++)
        {
            if(!IsInRange(x, position.y - 1) || tileType[x, position.y - 1] != CellType.FLOOR)
            {
                //no floor below
                return false;
            }

            if (!IsInRange(x, position.y) || tileType[x, position.y] != CellType.WALL)
            {
                //no wall same height
                return false;
            }

            if (!IsInRange(x, position.y + 1) || tileType[x, position.y + 1] != CellType.WALL)
            {
                //no wall above
                return false;
            }
        }

        return true;
    }

    //check if position valid for cave dungeon
    private bool ValidCavePosition(Vector2Int position)
    {
        if (!IsInRange(position.x -1 , position.y) || tileType[position.x - 1, position.y] != CellType.WALL)
        {
            //no wall left
            return false;
        }

        if (!IsInRange(position.x + 1, position.y) || tileType[position.x + 1, position.y] != CellType.WALL)
        {
            //no wall right
            return false;
        }

        return true;
    }

    #endregion

    #endregion

    #region Position finding
    /// <summary>
    ///     This function returns random position an object could be placed on
    /// </summary>
    /// <returns>A random position</returns>
    private Vector2Int GetPosition()
    {
        int index = Random.Range(0, objectPositions.Count);
        return objectPositions[index];
    }

    /// <summary>
    ///     This function returns random wall position an dungeon spot could be placed on
    /// </summary>
    /// <returns>A random wall position</returns>
    private Vector2Int GetWallPosition()
    {
        int index = Random.Range(0, dungeonWallPositions.Count);
        return dungeonWallPositions[index];
    }

    /// <summary>
    ///     This function returns a random position in a square around the given center position
    /// </summary>
    /// <param name="center">The center of the square</param>
    /// <param name="size">The size in all directions</param>
    /// <returns>A random position in the square arount the given center</returns>
    private Vector2Int GetPositionInRange(Vector2Int center, int size)
    {
        int minX = center.x - size;
        int maxX = center.x + size;
        int minY = center.y - size;
        int maxY = center.y + size;

        List<Vector2Int> positionsInRange = new List<Vector2Int>();

        foreach(Vector2Int position in objectPositions)
        {
            if(position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY)
            {
                positionsInRange.Add(position);
            }
        }

        int index = Random.Range(0, positionsInRange.Count);
        return positionsInRange[index];
    }

    #endregion

    private bool IsInRange(int posX, int posY)
    {
        return (posX >= 0 && posX < size.x && posY >= 0 && posY < size.y);
    }

    #region Distance checking
    /// <summary>
    ///     This function checks, whether a given position is to close to any other object or not
    /// </summary>
    /// <param name="positionToCheck">The position to check</param>
    /// <returns>True, if any object is too close, false otherwise</returns>
    private bool ObjectToClose(Vector2Int positionToCheck, int minMinigameDistance, int minNpcDistance, int minBookDistane, int minTeleporterDistance, int minDungeonDistance, int minWorldConnectionDistance)
    {
        //check world connections
        List<Vector2Int> worldConnectionPositions = new List<Vector2Int>();
        foreach (WorldConnection worldConnection in worldConnections)
        {
            worldConnectionPositions.Add(worldConnection.GetPosition());
        }
        if (ObjectToClose(positionToCheck, worldConnectionPositions, minWorldConnectionDistance))
        {
            return true;
        }

        //check minigames
        if(ObjectToClose(positionToCheck, minigamePositions, minMinigameDistance))
        {
            return true;
        }

        //check npcs
        if (ObjectToClose(positionToCheck, npcPositions, minNpcDistance))
        {
            return true;
        }

        //check books
        if (ObjectToClose(positionToCheck, bookPositions, minBookDistane))
        {
            return true;
        }

        //check teleporters
        if (ObjectToClose(positionToCheck, teleporterPositions, minTeleporterDistance))
        {
            return true;
        }

        //check dungeons
        List<Vector2Int> dungeons = new List<Vector2Int>();
        foreach(DungeonSpotPosition dungeonSpot in dungeonPositions)
        {
            dungeons.Add(dungeonSpot.GetPosition());
        }
        if (ObjectToClose(positionToCheck, dungeons, minDungeonDistance))
        {
            return true;
        }

        //no object too close
        return false;
    }

    /// <summary>
    ///     This function checks, whether a given position is to close to any other object in a given list or not
    /// </summary>
    /// <param name="positionToCheck">The position to check</param>
    /// <param name="spots">The other spots to compare it to</param>
    /// <param name="minDistance">The minimum allowed distance</param>
    /// <returns>True, if any object is too close, false otherwise</returns>
    private bool ObjectToClose(Vector2Int positionToCheck, List<Vector2Int> spots, int minDistance)
    {
        foreach (Vector2Int spot in spots)
        {
            if(StraightLineDistance(spot, positionToCheck) < minDistance)
            {
                int distance = 0;
                Optional<List<Vector2Int>> path = pathfinder.FindPath(positionToCheck, spot);
                if (path.IsPresent())
                {
                    distance = pathfinder.GetDistance(path.Value());
                }

                if (distance < minDistance)
                {
                    return true;
                }
            }            
        }

        return false;
    }

    /// <summary>
    ///     This function calculates the straight line distance between two points
    /// </summary>
    /// <param name="pos1">The first position</param>
    /// <param name="pos2">The second position</param>
    /// <returns>The straigth line distance between the given positions</returns>
    private float StraightLineDistance(Vector2Int pos1, Vector2Int pos2)
    {
        int deltaX = Mathf.Abs(pos1.x - pos2.x);
        int deltaY = Mathf.Abs(pos1.y - pos2.y);

        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
    #endregion

    #region Getter
    public List<Vector2Int> GetMinigameSpotPositions()
    {
        return minigamePositions;
    }

    public List<Vector2Int> GetNpcSpotPositions()
    {
        return npcPositions;
    }

    public List<Vector2Int> GetBookSpotPositions()
    {
        return bookPositions;
    }

    public List<Vector2Int> GetTeleporterSpotPositions()
    {
        return teleporterPositions;
    }

    public List<DungeonSpotPosition> GetDungeonSpotPositions()
    {
        return dungeonPositions;
    }
    #endregion
}
