using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to generate object positions
/// </summary>
public class ObjectPositionGenerator
{
    private static readonly int maxIterations = 10;
    private static readonly int minDungeonDistance = 75;
    private static readonly int minObjectDistance = 20;

    private Pathfinder pathfinder;

    private Vector2Int size;
    private bool[,] accessableTiles;
    private List<WorldConnection> worldConnections;
    private List<Vector2Int> minigamePositions;
    private List<Vector2Int> npcPositions;
    private List<Vector2Int> bookPositions;
    private List<Vector2Int> teleporterPositions;
    private List<Vector2Int> dungeonPositions;
    private List<Vector2Int> barrierPositions;

    public ObjectPositionGenerator(CellType[,] tiles, List<WorldConnection> worldConnections)
    {
        size = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
        accessableTiles = GetAccessableTiles(tiles);
        this.worldConnections = worldConnections;
        minigamePositions = new List<Vector2Int>();
        npcPositions = new List<Vector2Int>();
        bookPositions = new List<Vector2Int>();
        teleporterPositions = new List<Vector2Int>();
        dungeonPositions = new List<Vector2Int>();
        barrierPositions = new List<Vector2Int>();

        pathfinder = new Pathfinder(accessableTiles);
    }

    public ObjectPositionGenerator(TileSprite[,,] tiles, List<WorldConnection> worldConnections)
    {
        size = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
        accessableTiles = GetAccessableTiles(tiles);
        this.worldConnections = worldConnections;
        minigamePositions = new List<Vector2Int>();
        npcPositions = new List<Vector2Int>();
        bookPositions = new List<Vector2Int>();
        teleporterPositions = new List<Vector2Int>();
        dungeonPositions = new List<Vector2Int>();
        barrierPositions = new List<Vector2Int>();

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
    ///     This function gets the accessable tiles of a given tile sprite layout
    /// </summary>
    /// <param name="tiles">The layout</param>
    /// <returns>All accessable tiles</returns>
    private bool[,] GetAccessableTiles(TileSprite[,,] tiles)
    {
        bool[,] accessableTiles = new bool[size.x, size.y];
        for(int x=0; x<size.x; x++)
        {
            for(int y=0; y<size.y; y++)
            {
                if(TileIsFree(tiles, x, y))
                {
                    accessableTiles[x, y] = true;
                }
            }
        }
        return accessableTiles;
    }

    /// <summary>
    ///     This function checks, whether the given position is blocked or not
    /// </summary>
    /// <param name="tiles">The sprites in each tile</param>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <returns>True, if the tile is free, false if it blocked</returns>
    private bool TileIsFree(TileSprite[,,] tiles, int x, int y)
    {
        return (tiles[x, y, 2] == TileSprite.UNDEFINED && tiles[x, y, 4] == TileSprite.UNDEFINED);
    }

    #region Generate Functions
    /// <summary>
    ///     This function generates positions for minigame spots
    /// </summary>
    /// <param name="amount">The amount of minigame spots to generate</param>
    public void GenerateMinigamePositions(int amount)
    {
        //reset current spots
        this.minigamePositions = new List<Vector2Int>();

        List<Vector2Int> minigamePositions = new List<Vector2Int>();

        for(int i=0; i<amount; i++)
        {
            Vector2Int position = GetNewPosition();
            minigamePositions.Add(position);
        }

        this.minigamePositions = minigamePositions;
    }

    /// <summary>
    ///     This function generates positions for npc spots
    /// </summary>
    /// <param name="amount">The amount of npc spots to generate</param>
    public void GenerateNpcPositions(int amount)
    {
        //reset current spots
        this.npcPositions = new List<Vector2Int>();

        List<Vector2Int> npcPositions = new List<Vector2Int>();

        for (int i = 0; i < amount; i++)
        {
            Vector2Int position = GetNewPosition();
            npcPositions.Add(position);
        }

        this.npcPositions = npcPositions;
    }

    /// <summary>
    ///     This function generates positions for book spots
    /// </summary>
    /// <param name="amount">The amount of book spots to generate</param>
    public void GenerateBookPositions(int amount)
    {
        //reset current spots
        this.bookPositions = new List<Vector2Int>();

        List<Vector2Int> bookPositions = new List<Vector2Int>();

        for (int i = 0; i < amount; i++)
        {
            Vector2Int position = GetNewPosition();
            bookPositions.Add(position);
        }

        this.bookPositions = bookPositions;
    }

    /// <summary>
    ///     This function generates positions for teleporter spots
    /// </summary>
    /// <param name="amount">The amount of teleporter spots to generate</param>
    public void GenerateTeleporterPositions(int amount)
    {
        //reset current spots
        this.teleporterPositions = new List<Vector2Int>();

        List<Vector2Int> teleporterPositions = new List<Vector2Int>();

        for (int i = 0; i < amount; i++)
        {
            Vector2Int position = GetNewPosition();
            teleporterPositions.Add(position);
        }

        this.teleporterPositions = teleporterPositions;
    }

    /// <summary>
    ///     This function generates positions for dungeon spots
    /// </summary>
    /// <param name="amount">The amount of dungeon spots to generate</param>
    public void GenerateDungeonPositions(int amount)
    {
        //reset current spots
        this.dungeonPositions = new List<Vector2Int>();

        List<Vector2Int> dungeonPositions = new List<Vector2Int>();

        for (int i = 0; i < amount; i++)
        {
            Vector2Int position = new Vector2Int();

            for(int iteration = 0; iteration < maxIterations; iteration++)
            {
                position = GetNewPosition();

                bool objectToClose = ObjectToClose(position);

                if(!objectToClose)
                {
                    Debug.Log("Dungeon Spot found in " + (iteration + 1) + " tries");
                    break;
                }
            }

            dungeonPositions.Add(position);
        }

        this.dungeonPositions = dungeonPositions;
    }
    #endregion

    /// <summary>
    ///     This function returns a new position, where no object is yet
    /// </summary>
    /// <returns>A new position</returns>
    private Vector2Int GetNewPosition()
    {
        bool positionFound = false;
        Vector2Int position = new Vector2Int();

        while(!positionFound)
        {
            //get position
            int x = Random.Range(0, size.x - 1);
            int y = Random.Range(0, size.y - 1);

            position = new Vector2Int(x, y);

            //check if accessable and free
            if(accessableTiles[x, y] && FreePosition(position))
            {
                positionFound = true;
            }
        }

        return position;
    }

    /// <summary>
    ///     This function checks, whether a given position  is free or not (no other object present there)
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>True, if there is no object in the position, false otherwise</returns>
    private bool FreePosition(Vector2Int position)
    {
        if(minigamePositions.Contains(position) ||
            npcPositions.Contains(position) ||
            bookPositions.Contains(position) ||
            teleporterPositions.Contains(position) ||
            dungeonPositions.Contains(position) ||
            barrierPositions.Contains(position))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    ///     This function checks, whether a given position is to close to any other object or not
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>True, if any object is too close, false otherwise</returns>
    private bool ObjectToClose(Vector2Int position)
    {
        //check world connections
        foreach (WorldConnection worldConnection in worldConnections)
        {
            int distance = 0;
            Optional<List<Vector2Int>> path = pathfinder.FindPath(position, worldConnection.GetPosition());
            if (path.IsPresent())
            {
                distance = pathfinder.GetDistance(path.Value());
            }

            if (distance < minDungeonDistance)
            {
                Debug.Log("Potential Spot " + position.ToString() + " too close to world connection (" + worldConnection.GetDestinationWorld() + ") with distance " + distance);
                return true;
            }
        }

        //check minigames
        foreach (Vector2Int spot in minigamePositions)
        {
            int distance = 0;
            Optional<List<Vector2Int>> path = pathfinder.FindPath(position, spot);
            if (path.IsPresent())
            {
                distance = pathfinder.GetDistance(path.Value());
            }

            if (distance < minObjectDistance)
            {
                Debug.Log("Potential Spot " + position.ToString() + " too close to minigame spot with distance " + distance);
                return true;
            }
        }

        //check npcs
        foreach (Vector2Int spot in npcPositions)
        {
            int distance = 0;
            Optional<List<Vector2Int>> path = pathfinder.FindPath(position, spot);
            if (path.IsPresent())
            {
                distance = pathfinder.GetDistance(path.Value());
            }

            if (distance < minObjectDistance)
            {
                Debug.Log("Potential Spot " + position.ToString() + " too close to npc spot with distance " + distance);
                return true;
            }
        }

        //check books
        foreach (Vector2Int spot in bookPositions)
        {
            int distance = 0;
            Optional<List<Vector2Int>> path = pathfinder.FindPath(position, spot);
            if (path.IsPresent())
            {
                distance = pathfinder.GetDistance(path.Value());
            }

            if (distance < minObjectDistance)
            {
                Debug.Log("Potential Spot " + position.ToString() + " too close to book spot with distance " + distance);
                return true;
            }
        }

        //check teleporters
        foreach (Vector2Int spot in teleporterPositions)
        {
            int distance = 0;
            Optional<List<Vector2Int>> path = pathfinder.FindPath(position, spot);
            if (path.IsPresent())
            {
                distance = pathfinder.GetDistance(path.Value());
            }

            if (distance < minObjectDistance)
            {
                Debug.Log("Potential Spot " + position.ToString() + " too close to teleporter spot with distance " + distance);
                return true;
            }
        }

        //check dungeons
        foreach (Vector2Int spot in dungeonPositions)
        {
            int distance = 0;
            Optional<List<Vector2Int>> path = pathfinder.FindPath(position, spot);
            if (path.IsPresent())
            {
                distance = pathfinder.GetDistance(path.Value());
            }

            if (distance < minObjectDistance)
            {
                Debug.Log("Potential Spot " + position.ToString() + " too close to dungeon spot with distance " + distance);
                return true;
            }
        }

        //no object too close
        return false;
    }

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
