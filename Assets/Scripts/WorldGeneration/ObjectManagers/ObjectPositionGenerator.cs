using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to generate object positions
/// </summary>
public class ObjectPositionGenerator
{
    private Vector2Int size;
    private Vector2Int offset;
    private bool[,] accessableTiles;
    private List<Vector2> minigamePositions;
    private List<Vector2> npcPositions;
    private List<Vector2> bookPositions;
    private List<Vector2> teleporterPositions;
    private List<Vector2> sceneTransitionPositions;
    private List<Vector2> barrierPositions;

    public ObjectPositionGenerator(bool[,] accessableTiles, Vector2Int offset)
    {
        size = new Vector2Int(accessableTiles.GetLength(0), accessableTiles.GetLength(1));
        this.offset = offset;
        this.accessableTiles = accessableTiles;
        minigamePositions = new List<Vector2>();
        npcPositions = new List<Vector2>();
        bookPositions = new List<Vector2>();
        teleporterPositions = new List<Vector2>();
        sceneTransitionPositions = new List<Vector2>();
        barrierPositions = new List<Vector2>();
    }

    public ObjectPositionGenerator(TileSprite[,] tiles, Vector2Int offset)
    {
        size = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
        this.offset = offset;
        accessableTiles = GetAccessableTiles(tiles);
        minigamePositions = new List<Vector2>();
        npcPositions = new List<Vector2>();
        bookPositions = new List<Vector2>();
        teleporterPositions = new List<Vector2>();
        sceneTransitionPositions = new List<Vector2>();
        barrierPositions = new List<Vector2>();
    }

    /// <summary>
    ///     This function gets the accessable tiles of a given layout
    /// </summary>
    /// <param name="tiles">The layout</param>
    /// <returns>All accessable tiles</returns>
    private bool[,] GetAccessableTiles(TileSprite[,] tiles)
    {
        bool[,] accessableTiles = new bool[size.x, size.y];
        for(int x=0; x<size.x; x++)
        {
            for(int y=0; y<size.y; y++)
            {
                switch(tiles[x,y])
                {
                    case TileSprite.CAVE_FLOOR:
                    case TileSprite.BEACH_FLOOR:
                    case TileSprite.FOREST_FLOOR:
                    case TileSprite.SAVANNA_FLOOR:
                        accessableTiles[x, y] = true;
                        break;
                }
            }
        }
        return accessableTiles;
    }

    #region Generate Functions
    /// <summary>
    ///     This function generates positions for minigame spots
    /// </summary>
    /// <param name="amount">The amount of minigame spots to generate</param>
    public void GenerateMinigamePositions(int amount)
    {
        List<Vector2> minigamePositions = new List<Vector2>();

        for(int i=0; i<amount; i++)
        {
            Vector2 position = GetNewPosition();
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
        List<Vector2> npcPositions = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {
            Vector2 position = GetNewPosition();
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
        List<Vector2> bookPositions = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {
            Vector2 position = GetNewPosition();
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
        List<Vector2> teleporterPositions = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {
            Vector2 position = GetNewPosition();
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
        List<Vector2> dungeonPositions = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {
            Vector2 position = GetNewPosition();
            dungeonPositions.Add(position);
        }

        this.sceneTransitionPositions = dungeonPositions;
    }
    #endregion

    /// <summary>
    ///     This function returns a new position, where no object is yet
    /// </summary>
    /// <returns>A new position</returns>
    private Vector2 GetNewPosition()
    {
        bool positionFound = false;
        Vector2 position = new Vector2();

        while(!positionFound)
        {
            //get position
            int x = Random.Range(0, size.x - 1);
            int y = Random.Range(0, size.y - 1);

            position = new Vector2(x + 0.5f, y + 0.5f);

            //check if accessable and free
            if(accessableTiles[x, y] && FreePosition(position))
            {
                positionFound = true;
            }
        }

        //shift to global position
        position = new Vector2(position.x + offset.x, position.y + offset.y);

        return position;
    }

    /// <summary>
    ///     This function checks, whether a given position  is free or not (no other object present there)
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>True, if there is no object in the position, false otherwise</returns>
    private bool FreePosition(Vector2 position)
    {
        if(minigamePositions.Contains(position) ||
            npcPositions.Contains(position) ||
            bookPositions.Contains(position) ||
            teleporterPositions.Contains(position) ||
            sceneTransitionPositions.Contains(position) ||
            barrierPositions.Contains(position))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #region Getter
    public List<Vector2> GetMinigameSpotPositions()
    {
        return minigamePositions;
    }

    public List<Vector2> GetNpcSpotPositions()
    {
        return npcPositions;
    }

    public List<Vector2> GetBookSpotPositions()
    {
        return bookPositions;
    }

    public List<Vector2> GetTeleporterSpotPositions()
    {
        return teleporterPositions;
    }

    public List<Vector2> GetDungeonSpotPositions()
    {
        return sceneTransitionPositions;
    }
    #endregion
}
