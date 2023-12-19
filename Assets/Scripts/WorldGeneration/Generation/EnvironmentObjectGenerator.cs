using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
///     This class adds environmental objects to a given layout
/// </summary>
public class EnvironmentObjectGenerator
{
    #region Settings
    //Object lists for the area styles
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

    #region Attributes
    //Attributes
    private int borderSize;
    private Vector2Int size;
    private CellType[,] cellTypes;
    private TileSprite[,,] tileSprites;
    private WorldStyle style;
    private System.Random pseudoRandomNumberGenerator;

    //Settings
    private float maxObjectPercentage;
    private int maxIterationsPerObject;
    private int objectTriesPerPosition = 3;
    private int minDistance;
    private float spawnChance;
    private int spawnDistance;
    private float bigStoneExpandChance;
    private float bushExpandChance;
    private float treeExpandChance;
    private float fenceExpandChance;
    private int maxLogSize;
    private float logExpandChance;
    #endregion

    public EnvironmentObjectGenerator(CellType[,] cellTypes, TileSprite[,,] tileSprites, WorldStyle style, string seed)
    {
        size = new Vector2Int(cellTypes.GetLength(0), cellTypes.GetLength(1));
        this.cellTypes = cellTypes;
        this.tileSprites = tileSprites;
        this.style = style;
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

        path = "GameSettings/EnvironmentObjectsSettings";
        targetFile = Resources.Load<TextAsset>(path);
        json = targetFile.text;
        EnvironmentObjectsSettings environmentObjectsSettings = EnvironmentObjectsSettings.CreateFromJSON(json);

        maxObjectPercentage = environmentObjectsSettings.maxObjectPercentage;
        maxIterationsPerObject = environmentObjectsSettings.maxIterationsPerObject;
        minDistance = environmentObjectsSettings.minDistance;
        spawnChance = environmentObjectsSettings.spawnChance;
        spawnDistance = environmentObjectsSettings.spawnDistance;
        bigStoneExpandChance = environmentObjectsSettings.bigStoneExpandChance;
        bushExpandChance = environmentObjectsSettings.bushExpandChance;
        treeExpandChance = environmentObjectsSettings.treeExpandChance;
        fenceExpandChance = environmentObjectsSettings.fenceExpandChance;
        maxLogSize = environmentObjectsSettings.maxLogSize;
        logExpandChance = environmentObjectsSettings.logExpandChance;
    }

    /// <summary>
    ///     This function adds environmental objects to the layout
    /// </summary>
    public void AddObjects()
    {
        //sample points
        List<Vector2Int> objectPositions = GetObjectPositions();

        //get possible objects
        List<EnvironmentObject> objects = GetPossibleObjects();

        int objectsPlaced = 0;

        //for each point:
        foreach (Vector2Int objectPosition in objectPositions)
        {
            for(int i = 0; i < objectTriesPerPosition; i++)
            {
                //decide type
                int objectIndex = pseudoRandomNumberGenerator.Next(0, objects.Count);
                EnvironmentObject objectType = objects[objectIndex];

                //place if possible
                if(TryPlaceObject(objectPosition, objectType))
                {
                    //object placed, no more tries needed
                    objectsPlaced++;
                    break;
                }
            }            
        }

        Debug.Log("Objects placed " + objectsPlaced + " / " + objectPositions.Count);
    }

    #region Get Positinos

    /// <summary>
    ///     This function gets a list of positions to place an object at
    /// </summary>
    /// <returns>A list containing positions to place environmental objects at</returns>
    private List<Vector2Int> GetObjectPositions()
    {
        List<Vector2Int> objectPositions = new List<Vector2Int>();

        //get all possible positions
        List<Vector2Int> possiblePositions = GetPossiblePositions();
        Debug.Log("Possible Positions Found: " + possiblePositions.Count);

        //sample maxObjectPercentage of the positions
        int floorPositions = possiblePositions.Count;
        int maxObjects = Mathf.RoundToInt(maxObjectPercentage * floorPositions);
        Debug.Log("Try to return " + maxObjects + " object spots");

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

        for (int x = borderSize; x < size.x - borderSize; x++)
        {
            for (int y = borderSize; y < size.y - borderSize; y++)
            {
                if (cellTypes[x, y] == CellType.FLOOR && SurroundedByFloor(new Vector2Int(x, y)))
                {
                    objectPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return objectPositions;
    }

    /// <summary>
    ///     This funciton checks, if a given position is valid and far enough away from a list of positions
    /// </summary>
    /// <param name="objectPositions">The position to check the distance to</param>
    /// <param name="newPosition">The position ot check</param>
    /// <returns>True, if the position is not at a wall and is far enough away from all positions in the list, false otherwise</returns>
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

    /// <summary>
    ///     This function checks, if a given position is touching a wall or not
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>True, if no wall is touched, false otherwise</returns>
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

    /// <summary>
    ///     This funciton returns a list of valid positions near a given list of positions
    /// </summary>
    /// <param name="positions">The positions to find a new vaid position nearby</param>
    /// <returns>A list containing all valid positions nearby at least one of the given positions</returns>
    private List<Vector2Int> GetPossiblePositionsNearby(List<Vector2Int> positions)
    {
        HashSet<Vector2Int> possiblePositions = new HashSet<Vector2Int>();

        foreach(Vector2Int position in positions)
        {
            possiblePositions.UnionWith(CheckForNearbyPositions(position));
        }

        return possiblePositions.ToList();
    }

    /// <summary>
    ///     This function gets all valid positions near the given position
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>A hash set containing all valid positions nearby the given one</returns>
    private HashSet<Vector2Int> CheckForNearbyPositions(Vector2Int position)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();

        for(int x = position.x - spawnDistance; x <= position.x + spawnDistance; x++)
        {
            for (int y = position.y - spawnDistance; y <= position.y + spawnDistance; y++)
            {
                Vector2Int potentialPosition = new Vector2Int(x, y);
                if (SurroundedByFloor(potentialPosition))
                {
                    positions.Add(potentialPosition);
                }
            }
        }

        return positions;
    }

    #endregion

    /// <summary>
    ///     This function returns all suitable object types
    /// </summary>
    /// <returns>A list containing all suitable object types for the area style</returns>
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

    /// <summary>
    ///     This function tries to place a given object type at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <param name="type">The type of object to place</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool TryPlaceObject(Vector2Int position, EnvironmentObject type)
    {
        switch(type)
        {
            case EnvironmentObject.STONE_SMALL:
                return PlaceSmallStone(position);

            case EnvironmentObject.STONE_BIG:
                return PlaceBigStone(position);

            case EnvironmentObject.BUSH:
                return PlaceBush(position, TileSprite.BUSH);

            case EnvironmentObject.SAVANNA_BUSH:
                return PlaceBush(position, TileSprite.SAVANNA_BUSH);

            case EnvironmentObject.TREE:
                return PlaceTree(position, TileSprite.TREE);

            case EnvironmentObject.SAVANNA_TREE:
                return PlaceTree(position, TileSprite.SAVANNA_TREE);

            case EnvironmentObject.TREE_STUMP:
                return PlaceTreeStump(position);

            case EnvironmentObject.FENCE:
                return PlaceFence(position);

            case EnvironmentObject.HOUSE_SMALL:
                return PlaceSmallHouse(position);

            case EnvironmentObject.HOUSE_BIG:
                return PlaceBigHouse(position);

            case EnvironmentObject.BARREL:
                return PlaceBarrel(position);

            case EnvironmentObject.LOG:
                return PlaceLog(position);

            case EnvironmentObject.GRAVE:
                return PlaceGrave(position);

            default:
                return false;
        }
    }

    /// <summary>
    ///     This function tries to place a small stone object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceSmallStone(Vector2Int position)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        //if expandable to 2x2: 50 - 50 chance, else 1x1
        Optional<Vector2Int> canExpand = FloorArea(position, 2, 2);
        if(canExpand.IsPresent())
        {
            int objectSize = pseudoRandomNumberGenerator.Next(0, 2);
            if (objectSize == 1)
            {
                //place 2x2 stone
                tiles.Add(canExpand.Value());
                tiles.Add(canExpand.Value() + Vector2Int.right);
                tiles.Add(canExpand.Value() + Vector2Int.up);
                tiles.Add(canExpand.Value() + new Vector2Int(1, 1));

                PlaceSpriteAtArea(canExpand.Value(), 2, 2, TileSprite.STONE_SMALL);
            }
            else
            {
                //place 1x1 stone
                tiles.Add(position);
                cellTypes[position.x, position.y] = CellType.OBJECT;
                tileSprites[position.x, position.y, 4] = TileSprite.STONE_SMALL;
            }
        }
        else
        {
            //place 1x1 stone
            tiles.Add(position);
            cellTypes[position.x, position.y] = CellType.OBJECT;
            tileSprites[position.x, position.y, 4] = TileSprite.STONE_SMALL;
        }

        //try to create other small stone nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if(spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(tiles);
            if(possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceSmallStone(spawnPosition);
            }            
        }

        return true;
    }

    /// <summary>
    ///     This function tries to place a big stone object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceBigStone(Vector2Int position)
    {
        Optional<Vector2Int> startPosition = FloorArea(position, 2, 2);

        if (!startPosition.IsPresent())
        {
            return false;
        }

        List<Vector2Int> startTiles = GetStartPosition(startPosition.Value(), 2, 2);
        HashSet<Vector2Int> currentTiles = new HashSet<Vector2Int>(startTiles);

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention options
            List<Tuple<Vector2Int, Vector2Int>> expandOptions = GetBigExpandOptions(currentTiles);

            if (expandOptions.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < bigStoneExpandChance * 100)
                {
                    placedSomething = true;

                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandOptions.Count);

                    //pick the tuple to expand
                    Tuple<Vector2Int, Vector2Int> expandPositions = expandOptions[expandIndex];

                    //add positions to hash set
                    currentTiles.Add(expandPositions.Item1);
                    currentTiles.Add(expandPositions.Item2);
                }
            }
        }

        //place sprites and mark as object
        foreach (Vector2Int tile in currentTiles)
        {
            tileSprites[tile.x, tile.y, 4] = TileSprite.STONE_BIG;
            cellTypes[tile.x, tile.y] = CellType.OBJECT;
        }

        //try to create small stone nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if (spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(currentTiles.ToList());
            if (possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceSmallStone(spawnPosition);
            }
        }

        return true;
    }

    /// <summary>
    ///     This function tries to place a bush object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceBush(Vector2Int position, TileSprite sprite)
    {
        List<Vector2Int> tiles = new List<Vector2Int> { position };
        List<Vector2Int> edgeTiles = new List<Vector2Int> { position };

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Tuple<Vector2Int, Vector2Int>> expandPoints = GetExpandOptions(tiles, edgeTiles);

            if(expandPoints.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < bushExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);
                    Tuple<Vector2Int, Vector2Int> expandPosition = expandPoints[expandIndex];
                    tiles.Add(expandPosition.Item2);

                    //update edge tiles
                    edgeTiles.Remove(expandPosition.Item1);
                    edgeTiles.Add(expandPosition.Item2);

                    placedSomething = true;
                }
            }            
        }

        //place sprites and mark as object
        foreach (Vector2Int tile in tiles)
        {
            tileSprites[tile.x, tile.y, 4] = sprite;
            cellTypes[tile.x, tile.y] = CellType.OBJECT;
        }

        //try to create other bush nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if (spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(tiles);
            if (possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceBush(spawnPosition, sprite);
            }
        }

        return true;
    }

    /// <summary>
    ///     This function tries to place a tree object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceTree(Vector2Int position, TileSprite sprite)
    {
        Optional<Vector2Int> startPosition = FloorArea(position, 2, 2);

        if (!startPosition.IsPresent())
        {
            return false;
        }

        List<Vector2Int> startTiles = GetStartPosition(startPosition.Value(), 2, 2);
        HashSet<Vector2Int> currentTiles = new HashSet<Vector2Int>(startTiles);

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Tuple<Vector2Int, Vector2Int>> expandOptions = GetBigExpandOptions(currentTiles);

            if (expandOptions.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < treeExpandChance * 100)
                {
                    placedSomething = true;

                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandOptions.Count);

                    //pick the tuple to expand
                    Tuple<Vector2Int, Vector2Int> expandPositions = expandOptions[expandIndex];

                    //add positions to hash set
                    currentTiles.Add(expandPositions.Item1);
                    currentTiles.Add(expandPositions.Item2);
                }
            }
        }

        //place sprites and mark as object
        foreach (Vector2Int tile in currentTiles)
        {
            tileSprites[tile.x, tile.y, 4] = sprite;
            cellTypes[tile.x, tile.y] = CellType.OBJECT;
        }

        return true;
    }

    /// <summary>
    ///     This function tries to place a tree stump object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceTreeStump(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 2, 2);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 2, 2, TileSprite.TREE_STUMP);
            return true;
        }
        return false;
    }

    /// <summary>
    ///     This function tries to place a fence object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceFence(Vector2Int position)
    {
        bool success = false;
        List<Vector2Int> tiles = new List<Vector2Int> { position };
        List<Vector2Int> edgeTiles = new List<Vector2Int> { position };

        bool placedSomething = true;

        while (placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Tuple<Vector2Int, Vector2Int>> expandPoints = GetExpandOptions(tiles, edgeTiles);

            if (expandPoints.Count > 0)
            {
                //expand by bush expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (expand < fenceExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);
                    Tuple<Vector2Int, Vector2Int> expandPosition = expandPoints[expandIndex];
                    tiles.Add(expandPosition.Item2);

                    //update edge tiles
                    edgeTiles.Remove(expandPosition.Item1);
                    edgeTiles.Add(expandPosition.Item2);

                    placedSomething = true;
                }
            }
        }

        //place sprites and mark as object
        if(tiles.Count > 1)
        {
            success = true;
            foreach (Vector2Int tile in tiles)
            {
                tileSprites[tile.x, tile.y, 4] = TileSprite.FENCE;
                cellTypes[tile.x, tile.y] = CellType.OBJECT;
            }
        }

        //try to create other fence nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if (spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(tiles);
            if (possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceFence(spawnPosition);
            }
        }

        return success;
    }

    /// <summary>
    ///     This function tries to place a small house object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceSmallHouse(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 2, 3);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 2, 3, TileSprite.HOUSE_SMALL);
            return true;
        }
        return false;
    }

    /// <summary>
    ///     This function tries to place a big house object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceBigHouse(Vector2Int position)
    {
        Optional<Vector2Int> spot = FloorArea(position, 5, 5);
        if (spot.IsPresent())
        {
            PlaceSpriteAtArea(spot.Value(), 5, 5, TileSprite.HOUSE_BIG);
            return true;
        }
        return false;
    }

    /// <summary>
    ///     This function tries to place a barrel object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceBarrel(Vector2Int position)
    {
        bool success = false;
        List<Vector2Int> tiles = new List<Vector2Int>();

        if(SurroundedByFloor(position + Vector2Int.up))
        {
            //expand upwards
            success = true;
            PlaceSpriteAtArea(position, 1, 2, TileSprite.BARREL);
            tiles.Add(position);
            tiles.Add(position + Vector2Int.up);
        }
        else if(SurroundedByFloor(position + Vector2Int.down))
        {
            //expand downwards
            success = true;
            PlaceSpriteAtArea(position + Vector2Int.down, 1, 2, TileSprite.BARREL);
            tiles.Add(position);
            tiles.Add(position + Vector2Int.down);
        }

        //try to create other barrel nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if (spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(tiles);
            if (possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceBarrel(spawnPosition);
            }
        }

        return success;
    }

    /// <summary>
    ///     This function tries to place a log object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceLog(Vector2Int position)
    {
        bool success = false;
        List<Vector2Int> tiles = new List<Vector2Int> { position };

        bool placedSomething = true;

        while(placedSomething)
        {
            placedSomething = false;

            //get all extention points
            List<Vector2Int> expandPoints = GetHorizontalExpandOptions(tiles);

            if(expandPoints.Count > 0)
            {
                //expand by log expand chance
                int expand = pseudoRandomNumberGenerator.Next(0, 100);
                if (tiles.Count < maxLogSize && expand < logExpandChance * 100)
                {
                    //choose where to expand
                    int expandIndex = pseudoRandomNumberGenerator.Next(0, expandPoints.Count);
                    Vector2Int expandPosition = expandPoints[expandIndex];
                    tiles.Add(expandPosition);
                    placedSomething = true;
                }
            }                 
        }

        //place sprites and mark as object
        if(tiles.Count > 1)
        {
            success = true;
            foreach(Vector2Int tile in tiles)
            {
                tileSprites[tile.x, tile.y, 4] = TileSprite.LOG;
                cellTypes[tile.x, tile.y] = CellType.OBJECT;
            }
        }

        //try to create other log nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if (spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(tiles);
            if (possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceLog(spawnPosition);
            }
        }

        return success;
    }

    /// <summary>
    ///     This function tries to place a gravestone object at the given position
    /// </summary>
    /// <param name="position">The position to place the object on</param>
    /// <returns>True, if the placement was successful, false otherwise</returns>
    private bool PlaceGrave(Vector2Int position)
    {
        bool success = false;
        List<Vector2Int> tiles = new List<Vector2Int>();

        Optional<Vector2Int> spot = FloorArea(position, 2, 2);
        if (spot.IsPresent())
        {
            success = true;
            tiles.Add(spot.Value());
            tiles.Add(spot.Value() + Vector2Int.right);
            tiles.Add(spot.Value() + Vector2Int.up);
            tiles.Add(spot.Value() + new Vector2Int(1, 1));
            PlaceSpriteAtArea(spot.Value(), 2, 2, TileSprite.GRAVE);
        }

        //try to create other grave nearby
        int spawn = pseudoRandomNumberGenerator.Next(0, 100);
        if (spawn < spawnChance * 100)
        {
            List<Vector2Int> possiblePositions = GetPossiblePositionsNearby(tiles);
            if (possiblePositions.Count > 0)
            {
                int spawnIndex = pseudoRandomNumberGenerator.Next(0, possiblePositions.Count);
                Vector2Int spawnPosition = possiblePositions[spawnIndex];
                PlaceGrave(spawnPosition);
            }
        }

        return success;
    }

    #region Helper functions

    /// <summary>
    ///     This function checks, if their is a valid area of given width and height containing the given position
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <param name="width">The width of the free area</param>
    /// <param name="height">The height of the free area</param>
    /// <returns>An optinal containing the bottom left of the valid area, if a valid area is found, an empty optional otherwise</returns>
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

    /// <summary>
    ///     This function checks, if the area from the given position and dimensions is free
    /// </summary>
    /// <param name="bottomLeft">The bottom left corner of the area</param>
    /// <param name="width">The width of the area</param>
    /// <param name="height">The height of the area</param>
    /// <returns>True, if the area is free, false otherwise</returns>
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

    /// <summary>
    ///     This function places a given sprite at all positions in a given area
    /// </summary>
    /// <param name="bottomLeft">The bottom left corner of the area</param>
    /// <param name="width">The width of the area</param>
    /// <param name="height">The height of the area</param>
    /// <param name="sprite">The sprite to place</param>
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

    /// <summary>
    ///     This function gets all horizontal expansion options for a given list of positions
    /// </summary>
    /// <param name="positions">The position to check</param>
    /// <returns>A list containing all horizontal expansion options</returns>
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

    /// <summary>
    ///     This function get all possible expansion for the given edge positions, without conflicts with the given positions
    /// </summary>
    /// <param name="positions">The positions to not conflict with</param>
    /// <param name="edgePositions">The edge positions to check for possible expansions</param>
    /// <returns>A list containing tuples of positions and where they can be expanded to</returns>
    private List<Tuple<Vector2Int, Vector2Int>> GetExpandOptions(List<Vector2Int> positions, List<Vector2Int> edgePositions)
    {
        HashSet<Tuple<Vector2Int, Vector2Int>> expandOptions = new HashSet<Tuple<Vector2Int, Vector2Int>>();

        foreach (Vector2Int position in edgePositions)
        {
            //check left
            if (CanExpand(positions, position, position + Vector2Int.left))
            {
                Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position, position + Vector2Int.left);
                expandOptions.Add(expandOption);
            }

            //check up
            if (CanExpand(positions, position, position + Vector2Int.up))
            {
                Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position, position + Vector2Int.up);
                expandOptions.Add(expandOption);
            }

            //check right
            if (CanExpand(positions, position, position + Vector2Int.right))
            {
                Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position, position + Vector2Int.right);
                expandOptions.Add(expandOption);
            }

            //check down
            if (CanExpand(positions, position, position + Vector2Int.down))
            {
                Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position, position + Vector2Int.down);
                expandOptions.Add(expandOption);
            }
        }

        return expandOptions.ToList();
    }

    /// <summary>
    ///     This function checks, if a given position can be expanded in a given direction, without conflicting with any of a given list of positions
    /// </summary>
    /// <param name="positions">The positions to not conflict with</param>
    /// <param name="expansionPoint">The position to expand</param>
    /// <param name="newPosition">The position to expand to</param>
    /// <returns>True, if the position can be expanded, false otherwise</returns>
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

    /// <summary>
    ///     This function counts the amount of neighbors a given position has in a given list
    /// </summary>
    /// <param name="positions">The positions to check</param>
    /// <param name="newPosition">The position to count the neighbors of</param>
    /// <returns>The amount of neighbors contained in the given list</returns>
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

    /// <summary>
    ///     This function returns a list containing all positions in an area of given width and height, starting at a given position
    /// </summary>
    /// <param name="bottomLeft">The bottom left corner of the area</param>
    /// <param name="width">The width of the area</param>
    /// <param name="height">The height of the area</param>
    /// <returns>A list containing all positions in the area</returns>
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

    /// <summary>
    ///     This function gets all options to expand a given list of positions to have at least one vertical and one horizontal neighbor in the given list of positions
    /// </summary>
    /// <param name="positions">The list of positions</param>
    /// <returns>A list of expansion options</returns>
    private List<Tuple<Vector2Int, Vector2Int>> GetBigExpandOptions(HashSet<Vector2Int> positions)
    {
        HashSet<Tuple<Vector2Int, Vector2Int>> expandOptions = new HashSet<Tuple<Vector2Int, Vector2Int>>();

        foreach (Vector2Int position in positions)
        {
            //check left
            if(!positions.Contains(position + Vector2Int.left) && SurroundedByFloor(position + Vector2Int.left))
            {
                if(positions.Contains(position + Vector2Int.up) && SurroundedByFloor(position + new Vector2Int(-1, 1)))
                {
                    //can expand left and left-up
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.left, position + new Vector2Int(-1, 1));
                    expandOptions.Add(expandOption);
                }

                if (positions.Contains(position + Vector2Int.down) && SurroundedByFloor(position + new Vector2Int(-1, -1)))
                {
                    //can expand left and left-down
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.left, position + new Vector2Int(-1, -1));
                    expandOptions.Add(expandOption);
                }
            }

            //check right
            if (!positions.Contains(position + Vector2Int.right) && SurroundedByFloor(position + Vector2Int.right))
            {
                if (positions.Contains(position + Vector2Int.up) && SurroundedByFloor(position + new Vector2Int(1, 1)))
                {
                    //can expand right and right-up
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.right, position + new Vector2Int(1, 1));
                    expandOptions.Add(expandOption);
                }

                if (positions.Contains(position + Vector2Int.down) && SurroundedByFloor(position + new Vector2Int(1, -1)))
                {
                    //can expand right and right-down
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.right, position + new Vector2Int(1, -1));
                    expandOptions.Add(expandOption);
                }
            }

            //check up
            if (!positions.Contains(position + Vector2Int.up) && SurroundedByFloor(position + Vector2Int.up))
            {
                if (positions.Contains(position + Vector2Int.left) && SurroundedByFloor(position + new Vector2Int(-1, 1)))
                {
                    //can expand up and up-left
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.up, position + new Vector2Int(-1, 1));
                    expandOptions.Add(expandOption);
                }

                if (positions.Contains(position + Vector2Int.right) && SurroundedByFloor(position + new Vector2Int(1, 1)))
                {
                    //can expand up and up-right
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.up, position + new Vector2Int(1, 1));
                    expandOptions.Add(expandOption);
                }
            }

            //check down
            if (!positions.Contains(position + Vector2Int.down) && SurroundedByFloor(position + Vector2Int.down))
            {
                if (positions.Contains(position + Vector2Int.left) && SurroundedByFloor(position + new Vector2Int(-1, -1)))
                {
                    //can expand down and down-left
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.down, position + new Vector2Int(-1, -1));
                    expandOptions.Add(expandOption);
                }

                if (positions.Contains(position + Vector2Int.right) && SurroundedByFloor(position + new Vector2Int(1, -1)))
                {
                    //can expand down and down-right
                    Tuple<Vector2Int, Vector2Int> expandOption = new Tuple<Vector2Int, Vector2Int>(position + Vector2Int.down, position + new Vector2Int(1, -1));
                    expandOptions.Add(expandOption);
                }
            }
        }

        return expandOptions.ToList();
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
