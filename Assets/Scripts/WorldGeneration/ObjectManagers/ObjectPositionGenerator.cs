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
    private static readonly int maxIterations = 10;

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
    private CellType[,] tiles;

    private List<WorldConnection> worldConnections;
    private List<Vector2Int> minigamePositions;
    private List<Vector2Int> npcPositions;
    private List<Vector2Int> bookPositions;
    private List<Vector2Int> teleporterPositions;
    private List<Vector2Int> dungeonPositions;
    private List<Vector2Int> barrierPositions;

    #region Constructor

    public ObjectPositionGenerator(CellType[,] tiles, List<WorldConnection> worldConnections)
    {
        size = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
        objectPositions = GetObjectPositions(tiles);
        this.tiles = tiles;

        this.worldConnections = worldConnections;
        minigamePositions = new List<Vector2Int>();
        npcPositions = new List<Vector2Int>();
        bookPositions = new List<Vector2Int>();
        teleporterPositions = new List<Vector2Int>();
        dungeonPositions = new List<Vector2Int>();
        barrierPositions = new List<Vector2Int>();

        bool[,] accessableTiles = GetAccessableTiles(tiles);
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
    ///     This function creates a hash set containig all positions an object can be placed at
    /// </summary>
    /// <param name="tiles">The tile types of the layout</param>
    /// <returns>A hash set containing all positions an object can be placed at</returns>
    private List<Vector2Int> GetObjectPositions(CellType[,] tiles)
    {
        List<Vector2Int> objectPositions = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if(tiles[x, y] == CellType.FLOOR)
                {
                    objectPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return objectPositions;
    }

    #endregion

    #region Generate Functions
    public void ResetObjects()
    {
        minigamePositions = new List<Vector2Int>();
        npcPositions = new List<Vector2Int>();
        bookPositions = new List<Vector2Int>();
        teleporterPositions = new List<Vector2Int>();
        dungeonPositions = new List<Vector2Int>();
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
        dungeonPositions = new List<Vector2Int>();

        bool success = true;

        //generate new spots
        for (int i = 0; i < amount; i++)
        {
            bool spotCreated = false;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                Vector2Int position = GetPosition();

                bool objectToClose = ObjectToClose(position, minObjectDistance, minObjectDistance, minObjectDistance, minObjectDistance, minSameObjectDistance, minDungeonDistance);

                if(!objectToClose)
                {
                    //position found                    
                    dungeonPositions.Add(position);
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
            dungeonPositions = new List<Vector2Int>();
        }

        return success;
    }
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
        if (ObjectToClose(positionToCheck, dungeonPositions, minDungeonDistance))
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

    public List<Vector2Int> GetDungeonSpotPositions()
    {
        return dungeonPositions;
    }
    #endregion
}
