using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkardsWalkGenerator : LayoutGenerator
{
    private static readonly int borderSize = 3;
    private static readonly int maxStepsWithoutChanges = 100;

    private int iterations;

    #region Constructors

    public DrunkardsWalkGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(seed, size, accessability, worldConnections) 
    {
        iterations = size.x * size.y * accessability / 100;
    }

    public DrunkardsWalkGenerator(
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(size, accessability, worldConnections) 
    {
        iterations = size.x * size.y * accessability / 100;
    }

    public DrunkardsWalkGenerator(
        string seed,
        Vector2Int size,
        int accessability)
        : base(seed, size, accessability) 
    {
        iterations = size.x * size.y * accessability / 100;
    }

    public DrunkardsWalkGenerator(
        Vector2Int size,
        int accessability)
        : base(size, accessability) 
    {
        iterations = size.x * size.y * accessability / 100;
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
        layout[previousPosition.x, previousPosition.y] = true;

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

            layout[newPosition.x, newPosition.y] = true;

            previousPosition = newPosition;
        }
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
                layout[x, y] = false;
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
