using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnvironmentObjectGenerator
{
    #region Settings
    //Settings
    private static readonly float maxObjectPercentage = 0.02f;
    private static readonly int maxIterationsPerObject = 10;
    private static readonly int minDistance = 10;

    private static readonly float bigStoneExpandChance = 0.7f;

    private static readonly float bushExpandChance = 0.8f;

    private static readonly float treeExpandChance = 0.8f;

    private static readonly float fenceExpandChance = 0.85f;

    private static readonly int maxLogSize = 4;
    private static readonly float logExpandChance = 0.66f;

    //Object lists
    private static List<EnvironmentObject> caveObjects = new List<EnvironmentObject> {
        EnvironmentObject.STONE_SMALL,
        EnvironmentObject.STONE_BIG,
        EnvironmentObject.BARREL,
        EnvironmentObject.GRAVE
    };

    private static List<EnvironmentObject> savannaObjects = new List<EnvironmentObject> {
        EnvironmentObject.STONE_SMALL,
        EnvironmentObject.STONE_BIG,
        EnvironmentObject.SAVANNA_BUSH,
        EnvironmentObject.SAVANNA_TREE,
        EnvironmentObject.TREE_STUMP,
        EnvironmentObject.FENCE,
        EnvironmentObject.HOUSE_SMALL,
        EnvironmentObject.HOUSE_BIG,
        EnvironmentObject.BARREL,
        EnvironmentObject.LOG
    };

    private static List<EnvironmentObject> beachObjects = new List<EnvironmentObject> {
        EnvironmentObject.STONE_SMALL,
        EnvironmentObject.STONE_BIG,
        EnvironmentObject.BUSH,
        EnvironmentObject.TREE,
        EnvironmentObject.TREE_STUMP,
        EnvironmentObject.HOUSE_SMALL,
        EnvironmentObject.HOUSE_BIG,
        EnvironmentObject.BARREL,
        EnvironmentObject.LOG
    };

    private static List<EnvironmentObject> forestObjects = new List<EnvironmentObject> {
        EnvironmentObject.STONE_SMALL,
        EnvironmentObject.STONE_BIG,
        EnvironmentObject.BUSH,
        EnvironmentObject.TREE,
        EnvironmentObject.TREE_STUMP,
        EnvironmentObject.FENCE,
        EnvironmentObject.HOUSE_SMALL,
        EnvironmentObject.HOUSE_BIG,
        EnvironmentObject.BARREL,
        EnvironmentObject.LOG
    };
    #endregion

    //Attributes
    private Vector2Int size;
    private CellType[,] cellTypes;
    private TileSprite[,,] tileSprites;
    private WorldStyle style;
    private System.Random pseudoRandomNumberGenerator;


    public EnvironmentObjectGenerator(CellType[,] cellTypes, TileSprite[,,] tileSprites, WorldStyle style, string seed)
    {
        size = new Vector2Int(cellTypes.GetLength(0), cellTypes.GetLength(1));
        this.cellTypes = cellTypes;
        this.tileSprites = tileSprites;
        this.style = style;
        pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());
    }

    //adds environment objects to the layout
    public void AddObjects()
    {
        //sample points
        List<Vector2Int> objectPositions = GetObjectPositions();

        //get possible objects
        List<EnvironmentObject> objects = GetPossibleObjects();

        //for each point:
        foreach (Vector2Int objectPosition in objectPositions)
        {
            //  decide type
            int objectIndex = pseudoRandomNumberGenerator.Next(0, objects.Count);
            EnvironmentObject objectType = objects[objectIndex];

            //  place if possible
            TryPlaceObject(objectPosition, objectType);
        }
    }

    #region Get Positinos

    //return object positions
    private List<Vector2Int> GetObjectPositions()
    {
        List<Vector2Int> objectPositions = new List<Vector2Int>();

        //get all possible positions
        List<Vector2Int> possiblePositions = GetPossiblePositions();

        //sample at max 1% of the positions
        int floorPositions = possiblePositions.Count;
        int maxObjects = Mathf.RoundToInt(maxObjectPercentage * floorPositions);

        for(int objectIndex=0; objectIndex < maxObjects; objectIndex++)
        {
            bool positionFound = false;

            for(int iteration = 0; iteration < maxIterationsPerObject; iteration++)
            {
                //get random position
                int index = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int newPosition = possiblePositions[index];

                if(ValidPosition(objectPositions, newPosition))
                {
                    //valid position found
                    objectPositions.Add(newPosition);
                    positionFound = true;
                    break;
                }
                else
                {
                    //position not possible
                    possiblePositions.Remove(newPosition);
                }
            }

            if(!positionFound)
            {
                //no more valid position found
                break;
            }
        }

        return objectPositions;
    }

    /// <summary>
    ///     This function creates a list containig all positions an object can be placed at
    /// </summary>
    /// <returns>A list containing all positions an object can be placed at</returns>
    private List<Vector2Int> GetPossiblePositions()
    {
        List<Vector2Int> objectPositions = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (cellTypes[x, y] == CellType.FLOOR)
                {
                    objectPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return objectPositions;
    }

    //checks if any position is to close
    private bool ValidPosition(List<Vector2Int> objectPositions, Vector2Int newPosition)
    {
        if(!SurroundedByFloor(newPosition))
        {
            return false;
        }

        foreach(Vector2Int objectPosition in objectPositions)
        {
            if(GetDistance(objectPosition, newPosition) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    //true, if all floor tiles around, false otherwise
    private bool SurroundedByFloor(Vector2Int position)
    {
        for(int x = position.x - 1; x <= position.x + 1; x++)
        {
            for(int y = position.y - 1; y <= position.y + 1; y++)
            {
                if(!IsInRange(x, y) || cellTypes[x, y] != CellType.FLOOR)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    ///     This function calculates the distance between two points
    /// </summary>
    /// <param name="pos1">The first position</param>
    /// <param name="pos2">The second position</param>
    /// <returns>The distance between the given positions</returns>
    private float GetDistance(Vector2Int pos1, Vector2Int pos2)
    {
        int deltaX = Mathf.Abs(pos1.x - pos2.x);
        int deltaY = Mathf.Abs(pos1.y - pos2.y);

        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    #endregion

    //get all allowed objects
    private List<EnvironmentObject> GetPossibleObjects()
    {
        List<EnvironmentObject> objects = new List<EnvironmentObject>();

        switch (style)
        {
            case WorldStyle.SAVANNA:
                objects = savannaObjects;
                break;

            case WorldStyle.CAVE:
                objects = caveObjects;
                break;

            case WorldStyle.BEACH:
                objects = beachObjects;
                break;

            case WorldStyle.FOREST:
                objects = forestObjects;
                break;
        }

        return objects;
    }

    #region Object Placement

    //tries to place an object of given type at given position
    private void TryPlaceObject(Vector2Int position, EnvironmentObject type)
    {
        switch(type)
        {
            case EnvironmentObject.STONE_SMALL:
                PlaceSmallStone(position);
                break;

            case EnvironmentObject.STONE_BIG:
                PlaceBigStone(position);
                break;

            case EnvironmentObject.BUSH:
                PlaceBush(position, TileSprite.BUSH);
                break;

            case EnvironmentObject.SAVANNA_BUSH:
                PlaceBush(position, TileSprite.SAVANNA_BUSH);
                break;

            case EnvironmentObject.TREE:
                PlaceTree(position, TileSprite.TREE);
                break;

            case EnvironmentObject.SAVANNA_TREE:
                PlaceTree(position, TileSprite.SAVANNA_TREE);
                break;

            case EnvironmentObject.TREE_STUMP:
                PlaceTreeStump(position);
                break;

            case EnvironmentObject.FENCE:
                PlaceFence(position);
                break;

            case EnvironmentObject.HOUSE_SMALL:
                PlaceSmallHouse(position);
                break;

            case EnvironmentObject.HOUSE_BIG:
                PlaceBigHouse(position);
                break;

            case EnvironmentObject.BARREL:
                PlaceBarrel(position);
                break;

            case EnvironmentObject.LOG:
                PlaceLog(position);
                break;

            case EnvironmentObject.GRAVE:
                PlaceGrave(position);
                break;
        }
    }

    //places small stone object
    private void PlaceSmallStone(Vector2Int position)
    {
        //if expandable to 2x2: 50 - 50 chance, else 1x1
        Optional<Vector2Int> canExpand = FloorArea(position, 2, 2);
        if(canExpand.IsPresent())
        {
            int objectSize = pseudoRandomNumberGenerator.Next(0, 2);
            if (objectSize == 1)
            {
                //place 2x2 stone
                PlaceSpriteAtArea(canExpand.Value(), 2, 2, TileSprite.STONE_SMALL);
            }
            else
            {
                //place 1x1 stone
                cellTypes[position.x, position.y] = CellType.OBJECT;
                tileSprites[position.x, position.y, 4] = TileSprite.STONE_SMALL;
            }
        }
        else
        {
            //place 1x1 stone
            cellTypes[position.x, position.y] = CellType.OBJECT;
            tileSprites[position.x, position.y, 4] = TileSprite.STONE_SMALL;
        }
    }

    //places big stone object
    private void PlaceBigStone(Vector2Int position)
    {
        Optional<Vector2Int> startPosition = FloorArea(position, 2, 2);

        if (!startPosition.IsPresent())
        {
            return;
        }

        List<Vector2Int> currentTiles = GetStartPosition(startPosition.Value(), 2, 2);

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Vector2Int> expandPoints = GetBigExpandOptions(currentTiles);

            if (expandPoints.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < bigStoneExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);

                    //pick two tiles to expand
                    Vector2Int expandPosition = expandPoints[expandIndex];

                    if(expandPoints.Contains(expandPosition + Vector2Int.left))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.left);
                        placedSomething = true;
                    }
                    else if (expandPoints.Contains(expandPosition + Vector2Int.up))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.up);
                        placedSomething = true;
                    }
                    else if (expandPoints.Contains(expandPosition + Vector2Int.right))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.right);
                        placedSomething = true;
                    }
                    else if (expandPoints.Contains(expandPosition + Vector2Int.down))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.down);
                        placedSomething = true;
                    }
                }
            }
        }

        //place sprites and mark as object
        foreach (Vector2Int tile in currentTiles)
        {
            tileSprites[tile.x, tile.y, 4] = TileSprite.STONE_BIG;
            cellTypes[tile.x, tile.y] = CellType.OBJECT;
        }
    }

    //places bush object
    private void PlaceBush(Vector2Int position, TileSprite sprite)
    {
        List<Vector2Int> currentTiles = new List<Vector2Int> { position };

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Vector2Int> expandPoints = GetExpandOptions(currentTiles);

            if(expandPoints.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < bushExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);
                    Vector2Int expandPosition = expandPoints[expandIndex];
                    currentTiles.Add(expandPosition);
                    placedSomething = true;
                }
            }            
        }

        //place sprites and mark as object
        foreach (Vector2Int tile in currentTiles)
        {
            tileSprites[tile.x, tile.y, 4] = sprite;
            cellTypes[tile.x, tile.y] = CellType.OBJECT;
        }
    }

    //place tree object
    private void PlaceTree(Vector2Int position, TileSprite sprite)
    {
        Optional<Vector2Int> startPosition = FloorArea(position, 2, 2);

        if (!startPosition.IsPresent())
        {
            return;
        }

        List<Vector2Int> currentTiles = GetStartPosition(startPosition.Value(), 2, 2);

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Vector2Int> expandPoints = GetBigExpandOptions(currentTiles);

            if (expandPoints.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < treeExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);

                    //pick two tiles to expand
                    Vector2Int expandPosition = expandPoints[expandIndex];

                    if (expandPoints.Contains(expandPosition + Vector2Int.left))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.left);
                        placedSomething = true;
                    }
                    else if (expandPoints.Contains(expandPosition + Vector2Int.up))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.up);
                        placedSomething = true;
                    }
                    else if (expandPoints.Contains(expandPosition + Vector2Int.right))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.right);
                        placedSomething = true;
                    }
                    else if (expandPoints.Contains(expandPosition + Vector2Int.down))
                    {
                        currentTiles.Add(expandPosition);
                        currentTiles.Add(expandPosition + Vector2Int.down);
                        placedSomething = true;
                    }
                }
            }
        }

        //place sprites and mark as object
        foreach (Vector2Int tile in currentTiles)
        {
            tileSprites[tile.x, tile.y, 4] = sprite;
            cellTypes[tile.x, tile.y] = CellType.OBJECT;
        }
    }

    //place tree stump object
    private void PlaceTreeStump(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 2, 2);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 2, 2, TileSprite.TREE_STUMP);
        }
    }

    //place fence object
    private void PlaceFence(Vector2Int position)
    {
        List<Vector2Int> currentTiles = new List<Vector2Int> { position };

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Vector2Int> expandPoints = GetExpandOptions(currentTiles);

            if (expandPoints.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < fenceExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);
                    Vector2Int expandPosition = expandPoints[expandIndex];
                    currentTiles.Add(expandPosition);
                    placedSomething = true;
                }
            }
        }

        //place sprites and mark as object
        if(currentTiles.Count > 1)
        {
            foreach (Vector2Int tile in currentTiles)
            {
                tileSprites[tile.x, tile.y, 4] = TileSprite.FENCE;
                cellTypes[tile.x, tile.y] = CellType.OBJECT;
            }
        }        
    }

    //place small house object
    private void PlaceSmallHouse(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 2, 3);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 2, 3, TileSprite.HOUSE_SMALL);
        }
    }

    //place big house object
    private void PlaceBigHouse(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 5, 5);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 5, 5, TileSprite.HOUSE_BIG);
        }
    }

    //place barrel object
    private void PlaceBarrel(Vector2Int position)
    {
        if(SurroundedByFloor(position + Vector2Int.up))
        {
            //expand upwards
            PlaceSpriteAtArea(position, 1, 2, TileSprite.BARREL);
        }
        else if(SurroundedByFloor(position + Vector2Int.down))
        {
            //expand downwards
            PlaceSpriteAtArea(position + Vector2Int.down, 1, 2, TileSprite.BARREL);
        }
    }

    //place log object
    private void PlaceLog(Vector2Int position)
    {
        List<Vector2Int> currentTiles = new List<Vector2Int> { position };

        bool placedSomething = true;

        while(placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Vector2Int> expandPoints = GetHorizontalExpandOptions(currentTiles);

            if(expandPoints.Count > 0)
            {
                //expand by log expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (currentTiles.Count < maxLogSize && expand < logExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);
                    Vector2Int expandPosition = expandPoints[expandIndex];
                    currentTiles.Add(expandPosition);
                    placedSomething = true;
                }
            }                 
        }

        //place sprites and mark as object
        if(currentTiles.Count > 1)
        {
            foreach(Vector2Int tile in currentTiles)
            {
                tileSprites[tile.x, tile.y, 4] = TileSprite.LOG;
                cellTypes[tile.x, tile.y] = CellType.OBJECT;
            }
        }
    }

    //place grave object
    private void PlaceGrave(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 2, 2);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 2, 2, TileSprite.GRAVE);
        }
    }

    #region Helper functions

    //check if square of given size is free including given position
    //return bottom left, if found, empty optinal otherwise
    private Optional<Vector2Int> FloorArea(Vector2Int position, int width, int height)
    {
        Optional<Vector2Int> bottomLeft = new Optional<Vector2Int>();

        for(int x = position.x - width + 1; x <= position.x; x++)
        {
            for (int y = position.y - height + 1; y <= position.y; y++)
            {
                Vector2Int bottomLeftVector = new Vector2Int(x, y);
                if (FreeFloorArea(bottomLeftVector, width, height))
                {
                    bottomLeft.SetValue(bottomLeftVector);
                    return bottomLeft;
                }
            }
        }

        return bottomLeft;
    }

    //check if given area and 1 tile around is floor
    private bool FreeFloorArea(Vector2Int bottomLeft, int width, int height)
    {
        for (int x = bottomLeft.x - 1; x <= bottomLeft.x + width + 1; x++)
        {
            for (int y = bottomLeft.y - 1; y <= bottomLeft.y + height + 1; y++)
            {
                if (!IsInRange(x, y) || cellTypes[x, y] != CellType.FLOOR)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //places given sprite at all tiles in given square
    private void PlaceSpriteAtArea(Vector2Int bottomLeft, int width, int height, TileSprite sprite)
    {
        for(int x = bottomLeft.x; x < bottomLeft.x + width; x++)
        {
            for(int y = bottomLeft.y; y < bottomLeft.y + height; y++)
            {
                cellTypes[x, y] = CellType.OBJECT;
                tileSprites[x, y, 4] = sprite;
            }
        }
    }

    //gets free tiles on left and right
    private List<Vector2Int> GetHorizontalExpandOptions(List<Vector2Int> positions)
    {
        List<Vector2Int> expandOptions = new List<Vector2Int>();

        foreach(Vector2Int position in positions)
        {
            if (!positions.Contains(position + Vector2Int.left) && SurroundedByFloor(position + Vector2Int.left))
            {
                expandOptions.Add(position + Vector2Int.left);
            }

            if (!positions.Contains(position + Vector2Int.right) && SurroundedByFloor(position + Vector2Int.right))
            {
                expandOptions.Add(position + Vector2Int.right);
            }
        }        

        return expandOptions;
    }

    //gets free tiles in all four directions
    private List<Vector2Int> GetExpandOptions(List<Vector2Int> positions)
    {
        HashSet<Vector2Int> expandOptions = new HashSet<Vector2Int>();

        foreach (Vector2Int position in positions)
        {
            //check left
            if (CanExpand(positions, position, position + Vector2Int.left))
            {
                expandOptions.Add(position + Vector2Int.left);
            }

            //check up
            if (CanExpand(positions, position, position + Vector2Int.up))
            {
                expandOptions.Add(position + Vector2Int.up);
            }

            //check right
            if (CanExpand(positions, position, position + Vector2Int.right))
            {
                expandOptions.Add(position + Vector2Int.right);
            }

            //check down
            if (CanExpand(positions, position, position + Vector2Int.down))
            {
                expandOptions.Add(position + Vector2Int.down);
            }
        }

        return expandOptions.ToList();
    }

    //checks if expansionPoint can be expanded by newPosition
    private bool CanExpand(List<Vector2Int> positions, Vector2Int expansionPoint, Vector2Int newPosition)
    {
        if (!SurroundedByFloor(newPosition))
        {
            return false;
        }

        if (GetAmountNeighbors(positions, newPosition) > 1)
        {
            //new position would have more than 1 neighbor
            return false;
        }

        if (GetAmountNeighbors(positions, expansionPoint) > 1)
        {
            //expanded position would have more than 2 neighbors
            return false;
        }

        return true;
    }

    //count neighbors in list
    private int GetAmountNeighbors(List<Vector2Int> positions, Vector2Int newPosition)
    {
        int neighbors = 0;

        if (positions.Contains(newPosition + Vector2Int.left))
        {
            neighbors++;
        }

        if (positions.Contains(newPosition + Vector2Int.up))
        {
            neighbors++;
        }

        if (positions.Contains(newPosition + Vector2Int.right))
        {
            neighbors++;
        }

        if (positions.Contains(newPosition + Vector2Int.down))
        {
            neighbors++;
        }

        return neighbors;
    }

    //return list of vectors of given position and size
    private List<Vector2Int> GetStartPosition(Vector2Int bottomLeft, int width, int height)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = bottomLeft.x; x < bottomLeft.x + width; x++)
        {
            for (int y = bottomLeft.y; y < bottomLeft.y + height; y++)
            {
                positions.Add(new Vector2Int(x, y));
            }
        }

        return positions;
    }

    //gets free tiles in all four directions, at least two tiles
    private List<Vector2Int> GetBigExpandOptions(List<Vector2Int> positions)
    {
        HashSet<Vector2Int> expandOptions = new HashSet<Vector2Int>();

        foreach (Vector2Int position in positions)
        {
            //check left
            if (CanBigExpand(positions, position, position + Vector2Int.left))
            {
                expandOptions.Add(position + Vector2Int.left);
            }

            //check up
            if (CanBigExpand(positions, position, position + Vector2Int.up))
            {
                expandOptions.Add(position + Vector2Int.up);
            }

            //check right
            if (CanBigExpand(positions, position, position + Vector2Int.right))
            {
                expandOptions.Add(position + Vector2Int.right);
            }

            //check down
            if (CanBigExpand(positions, position, position + Vector2Int.down))
            {
                expandOptions.Add(position + Vector2Int.down);
            }
        }

        return expandOptions.ToList();
    }

    //checks if expansionPoint can be expanded by newPosition
    private bool CanBigExpand(List<Vector2Int> positions, Vector2Int expansionPoint, Vector2Int newPosition)
    {
        if(positions.Contains(newPosition))
        {
            return false;
        }

        if (!SurroundedByFloor(newPosition))
        {
            return false;
        }

        if (expansionPoint.x == newPosition.x)
        {
            //try expand up / down
            if(!positions.Contains(newPosition + Vector2Int.left) 
                && IsNeighbor(positions, newPosition + Vector2Int.left)
                && SurroundedByFloor(newPosition + Vector2Int.left))
            {
                //tile left of newPosition is free and not in list, but neighbor -> can expand
                return true;
            }
            if (!positions.Contains(newPosition + Vector2Int.right)
                && IsNeighbor(positions, newPosition + Vector2Int.right)
                && SurroundedByFloor(newPosition + Vector2Int.right))
            {
                //tile right of newPosition is free and not in list, but neighbor -> can expand
                return true;
            }
        }
        else
        {
            //try expand left / right
            if (!positions.Contains(newPosition + Vector2Int.up)
                && IsNeighbor(positions, newPosition + Vector2Int.up)
                && SurroundedByFloor(newPosition + Vector2Int.up))
            {
                //tile above of newPosition is free and not in list, but neighbor -> can expand
                return true;
            }
            if (!positions.Contains(newPosition + Vector2Int.down)
                && IsNeighbor(positions, newPosition + Vector2Int.down)
                && SurroundedByFloor(newPosition + Vector2Int.down))
            {
                //tile below of newPosition is free and not in list, but neighbor -> can expand
                return true;
            }
        }

        return false;
    }

    //check, if any neighbor is in list
    private bool IsNeighbor(List<Vector2Int> positions, Vector2Int newPosition)
    {
        if(positions.Contains(newPosition + Vector2Int.left))
        {
            return true;
        }

        if (positions.Contains(newPosition + Vector2Int.up))
        {
            return true;
        }

        if (positions.Contains(newPosition + Vector2Int.right))
        {
            return true;
        }

        if (positions.Contains(newPosition + Vector2Int.down))
        {
            return true;
        }

        return false;
    }

    #endregion

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

    #region Getter

    public TileSprite[,,] GetTileSprites()
    {
        return tileSprites;
    }

    public CellType[,] GetCellTypes()
    {
        return cellTypes;
    }

    #endregion
}
