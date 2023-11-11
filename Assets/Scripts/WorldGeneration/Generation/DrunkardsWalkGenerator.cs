using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkardsWalkGenerator : LayoutGenerator
{
    private int borderSize;
    private int radius;

    private int iterations;
    private HashSet<Vector2Int> tiles;
    private bool canShift;

    #region Constructors

    public DrunkardsWalkGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(seed, size, accessability, worldConnections) 
    {
        GetSettings();
        iterations = size.x * size.y * accessability / 100;
        tiles = new HashSet<Vector2Int>();
        canShift = true;
    }

    public DrunkardsWalkGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        int borderSize)
        : base(seed, size, accessability) 
    {
        GetSettings();
        iterations = size.x * size.y * accessability / 100;
        tiles = new HashSet<Vector2Int>();
        canShift = true;
        this.borderSize = borderSize;
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
        radius = generationSettings.radiusDW;
        corridorSize = generationSettings.radiusDW;
    }

    #endregion

    public override void GenerateLayout()
    {
        Debug.Log("Drunkard's Walk layout generator" + "\n" +
            "Accessability: " + accessability + "\n" +
            "Seed: " + seed);

        //Initialize
        InitializeGrid();

        //Pseudo Random Number Generator
        System.Random pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());

        //Get start position
        Vector2Int previousPosition = new Vector2Int(size.x / 2, size.y / 2);
        tiles.Add(previousPosition);

        for (int i = 0; i < iterations; i++)
        {
            bool validPositionFound = false;
            Vector2Int newPosition = new Vector2Int();

            while(!validPositionFound)
            {
                Direction direction = (Direction) pseudoRandomNumberGenerator.Next(0, 5);
                newPosition = previousPosition + GetPositionChange(direction);

                if(IsInCenter(newPosition))
                {
                    validPositionFound = true;
                }
            }

            HashSet<Vector2Int> newTiles = DiggCircle(newPosition, radius);
            tiles.UnionWith(newTiles);

            //if new position is too close to border
            if(canShift && IsToCloseToBorder(newPosition))
            {
                //get distance to all borders
                LayoutDistance distances = GetDistances();

                //shift to center, if possible
                int xShift = (distances.right - distances.left) / 2;
                int yShift = (distances.up - distances.down) / 2;
                if (xShift == 0 && yShift == 0)
                {
                    canShift = false;
                }
                Shift(xShift, yShift);

                //shift previousPosition variable
                previousPosition = newPosition + new Vector2Int(xShift, yShift);
            }
            else
            {
                previousPosition = newPosition;
            }
        }

        ConvertHashSetToLayout();

        EnsureConnectivity();       
    }

    /// <summary>
    ///     This function initializes the layout as all walls
    /// </summary>
    private void InitializeGrid()
    {
        for(int x=0; x < size.x; x++)
        {
            for(int y=0; y < size.y; y++)
            {
                layout[x, y] = CellType.WALL;
            }
        }
    }

    /// <summary>
    ///     This function returns the position change for the given direction
    /// </summary>
    /// <param name="direction">The direction to move in</param>
    /// <returns>A vector containig the position change</returns>
    private Vector2Int GetPositionChange(Direction direction)
    {
        switch(direction)
        {
            case Direction.LEFT:
                return new Vector2Int(-1, 0);

            case Direction.UP:
                return new Vector2Int(0, 1);

            case Direction.RIGHT:
                return new Vector2Int(1, 0);

            case Direction.DOWN:
                return new Vector2Int(0, -1);

            default:
                return new Vector2Int(0, 0);
        }
    }

    /// <summary>
    ///     This function marks the given position and all tiles within a circle around it as floor tiles
    /// </summary>
    /// <param name="position">The center position</param>
    /// <param name="radius">The radius of the circle</param>
    private HashSet<Vector2Int> DiggCircle(Vector2Int position, int radius)
    {
        HashSet<Vector2Int> newTiles = new HashSet<Vector2Int>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {                    
                    int tilePosX = position.x + x;
                    int tilePosY = position.y + y;
                    Vector2Int tile = new Vector2Int(tilePosX, tilePosY);

                    if (IsInCenter(tile))
                    {
                        newTiles.Add(tile);
                    }
                }
            }
        }

        return newTiles;
    }

    /// <summary>
    ///     This function checks, wether the given position is to close to the border
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>True, if too close to a border, false otherwise</returns>
    private bool IsToCloseToBorder(Vector2Int position)
    {
        int minX = borderSize + 2*radius;
        int maxX = size.x - (borderSize + 2*radius);
        int minY = borderSize + 2*radius;
        int maxY = size.y - (borderSize + 2*radius);
        return (position.x < minX || position.x > maxX || position.y < minY || position.y > maxY);
    }

    /// <summary>
    ///     This function gets the minimum distances of a floor tile to each border
    /// </summary>
    /// <returns>A <c>LayoutDistance</c> object containing the minimum distances</returns>
    private LayoutDistance GetDistances()
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        
        foreach(Vector2Int tile in tiles)
        {
            if(tile.x < minX)
            {
                minX = tile.x;
            }

            if(tile.x > maxX)
            {
                maxX = tile.x;
            }

            if(tile.y < minY)
            {
                minY = tile.y;
            }

            if(tile.y > maxY)
            {
                maxY = tile.y;
            }
        }

        int distanceLeft = minX - borderSize;
        int distanceRight = size.x - 1 - borderSize - maxX;
        int distanceDown = minY - borderSize;
        int distanceUp = size.y - 1 - borderSize - maxY;

        LayoutDistance distances = new LayoutDistance(distanceLeft, distanceUp, distanceRight, distanceDown);

        return distances;
    }

    /// <summary>
    ///     This function shifts all tiles by the given amount
    /// </summary>
    /// <param name="xShift">The shifting in x direction</param>
    /// <param name="yShift">The shifting in y direction</param>
    private void Shift(int xShift, int yShift)
    {
        HashSet<Vector2Int> newTiles = new HashSet<Vector2Int>();

        foreach(Vector2Int tile in tiles)
        {
            Vector2Int newTile = new Vector2Int(tile.x + xShift, tile.y + yShift);
            newTiles.Add(newTile);
        }

        tiles = newTiles;
    }

    /// <summary>
    ///     This function sets the tiles stored in the HashSet into the layout array
    /// </summary>
    private void ConvertHashSetToLayout()
    {
        foreach(Vector2Int tile in tiles)
        {
            layout[tile.x, tile.y] = CellType.FLOOR;
        }
    }

    /// <summary>
    ///     This function checks, whether a given position is inside of the layout center, so inside the border
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>True, if in range, false otherwise</returns>
    private bool IsInCenter(Vector2Int position)
    {
        int posX = position.x;
        int posY = position.y;
        return (posX >= borderSize && posX < size.x - borderSize && posY >= borderSize && posY < size.y - borderSize);
    }
}
