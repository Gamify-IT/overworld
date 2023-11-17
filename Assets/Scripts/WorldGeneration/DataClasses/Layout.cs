using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout
{
    private AreaInformation area;
    private TileSprite[,,] tileSprites;
    private CellType[,] cellTypes;
    private LayoutGeneratorType generatorType;
    private string seed;
    private int accessability;
    private WorldStyle style;

    public Layout(AreaInformation area, TileSprite[,,] tileSprites, CellType[,] cellTypes, LayoutGeneratorType generatorType, string seed, int accessability, WorldStyle style)
    {
        this.area = area;
        this.tileSprites = tileSprites;
        this.cellTypes = cellTypes;
        this.generatorType = generatorType;
        this.seed = seed;
        this.accessability = accessability;
        this.style = style;
    }

    #region Add / Remove dungeon spots

    //add dungeon spots
    public void AddDungeonSpots(List<SceneTransitionSpotData> dungeonSpots, Vector2Int offset)
    {
        foreach(SceneTransitionSpotData dungeonSpot in dungeonSpots)
        {
            switch(dungeonSpot.GetStyle())
            {
                case DungeonStyle.HOUSE:
                    AddHouseDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;

                case DungeonStyle.TRAPDOOR:
                    AddTrapdoorDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;

                case DungeonStyle.GATE:
                    AddGateDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;

                case DungeonStyle.CAVE_ENTRANCE:
                    AddCaveEntranceDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;
            }
        }
    }
    
    //adds house dungeon entrance
    private void AddHouseDungeonSpot(Vector2 position)
    {
        Debug.Log("Creating house dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(2.5f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        for (int x = posX; x < posX + 5; x++)
        {
            for (int y = posY; y < posY + 5; y++)
            {
                tileSprites[x, y, 4] = TileSprite.HOUSE;
            }
        }
    }

    //adds trapdoor dungeon entrance
    private void AddTrapdoorDungeonSpot(Vector2 position)
    {
        Debug.Log("Creating trapdoor dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(1.0f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        for (int x = posX; x < posX + 2; x++)
        {
            for (int y = posY; y < posY + 2; y++)
            {
                tileSprites[x, y, 4] = TileSprite.TRAPDOOR;
            }
        }
    }

    //adds gate dungeon entrance
    private void AddGateDungeonSpot(Vector2 position)
    {
        Debug.Log("Creating gate dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(2.0f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        for (int x = posX; x < posX + 4; x++)
        {
            for (int y = posY - 1; y < posY + 3; y++)
            {
                tileSprites[x, y, 3] = TileSprite.GATE;
            }
        }

        for (int x = posX + 1; x < posX + 3; x++)
        {
            for (int y = posY; y < posY + 2; y++)
            {
                tileSprites[x, y, 2] = TileSprite.UNDEFINED;
            }
        }
    }

    //adds cave entrance dungeon entrance
    private void AddCaveEntranceDungeonSpot(Vector2 position)
    {
        Debug.Log("Creating cave dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(0.5f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        //add cave entrance tiles
        tileSprites[posX, posY, 0] = TileSprite.CAVE_FLOOR;
        tileSprites[posX, posY, 3] = TileSprite.CAVE_ENTRANCE;
        tileSprites[posX, posY + 1, 3] = TileSprite.CAVE_ENTRANCE;

        //remove wall tiles (for colliders)
        tileSprites[posX, posY, 2] = TileSprite.UNDEFINED;
        tileSprites[posX, posY + 1, 2] = TileSprite.UNDEFINED;
    }

    //remove given dungeon spots
    public void RemoveDungeonSpots(List<SceneTransitionSpotData> dungeonSpots, Vector2Int offset)
    {
        foreach (SceneTransitionSpotData dungeonSpot in dungeonSpots)
        {
            switch (dungeonSpot.GetStyle())
            {
                case DungeonStyle.HOUSE:
                    RemoveHouseDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;

                case DungeonStyle.TRAPDOOR:
                    RemoveTrapdoorDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;

                case DungeonStyle.GATE:
                    RemoveGateDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;

                case DungeonStyle.CAVE_ENTRANCE:
                    RemoveCaveEntranceDungeonSpot(dungeonSpot.GetPosition() - offset);
                    break;
            }
        }
    }

    //removes house dungeon entrance
    private void RemoveHouseDungeonSpot(Vector2 position)
    {
        Debug.Log("Removing house dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(2.5f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        for (int x = posX; x < posX + 5; x++)
        {
            for (int y = posY; y < posY + 5; y++)
            {
                tileSprites[x, y, 4] = TileSprite.UNDEFINED;
            }
        }
    }

    //removes trapdoor dungeon entrance
    private void RemoveTrapdoorDungeonSpot(Vector2 position)
    {
        Debug.Log("Removing trapdoor dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(1.0f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        for (int x = posX; x < posX + 2; x++)
        {
            for (int y = posY; y < posY + 2; y++)
            {
                tileSprites[x, y, 4] = TileSprite.UNDEFINED;
            }
        }
    }

    //removes gate dungeon entrance
    private void RemoveGateDungeonSpot(Vector2 position)
    {
        Debug.Log("Removing gate dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(2.0f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        for (int x = posX; x < posX + 4; x++)
        {
            for (int y = posY - 1; y < posY + 3; y++)
            {
                tileSprites[x, y, 3] = TileSprite.UNDEFINED;
            }
        }

        for (int x = posX + 1; x < posX + 3; x++)
        {
            for (int y = posY; y < posY + 2; y++)
            {
                if(y == posY)
                {
                    tileSprites[x, y, 2] = TileSprite.CAVE_WALL_BOTTOM_MID;
                }
                else
                {
                    tileSprites[x, y, 2] = TileSprite.CAVE_WALL_TOP_MID;
                }                
            }
        }
    }

    //removes cave entrance dungeon entrance
    private void RemoveCaveEntranceDungeonSpot(Vector2 position)
    {
        Debug.Log("Removing cave dungeon entrance at " + position.ToString());
        Vector2 bottomLeftCorner = position - new Vector2(0.5f, 0.5f);

        int posX = Mathf.FloorToInt(bottomLeftCorner.x);
        int posY = Mathf.FloorToInt(bottomLeftCorner.y);

        //remove cave entrance tiles
        tileSprites[posX, posY, 3] = TileSprite.UNDEFINED;
        tileSprites[posX, posY + 1, 3] = TileSprite.UNDEFINED;

        //add wall
        tileSprites[posX, posY, 2] = TileSprite.CAVE_WALL_BOTTOM_MID;
        tileSprites[posX, posY + 1, 2] = TileSprite.CAVE_WALL_TOP_MID;
    }
    #endregion

    /// <summary>
    ///     This function converts a <c>LayoutDTO</c> object into a <c>Layout</c> instance
    /// </summary>
    /// <param name="layoutDTO">The <c>LayoutDTO</c> object to convert</param>
    /// <returns>The converted <c>Layout</c> object</returns>
    public static Layout ConvertDtoToData(LayoutDTO layoutDTO)
    {
        if(layoutDTO.sizeX == 0 && layoutDTO.sizeY == 0)
        {
            AreaInformation dummyArea = new AreaInformation(0, new Optional<int>());
            TileSprite[,,] dummyTilSprites = new TileSprite[0, 0, 0];
            CellType[,] dummyCellTypes = new CellType[0, 0];
            LayoutGeneratorType dummyGenerator = LayoutGeneratorType.CELLULAR_AUTOMATA;
            string dummySeed = "";
            int dummyAccessiblity = 0;
            WorldStyle dummyStyle = WorldStyle.CUSTOM;

            Layout dummyLayout = new Layout(dummyArea, dummyTilSprites, dummyCellTypes, dummyGenerator, dummySeed, dummyAccessiblity, dummyStyle);
            return dummyLayout;
        }

        int worldIndex = layoutDTO.area.worldIndex;
        Optional<int> dungeonIndex = new Optional<int>();
        if(layoutDTO.area.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(layoutDTO.area.dungeonIndex);
        }
        AreaInformation area = new AreaInformation(worldIndex, dungeonIndex);

        string seed = layoutDTO.seed;
        Vector2Int size = new Vector2Int(layoutDTO.sizeX, layoutDTO.sizeY);
        int accessability = layoutDTO.accessability;
        LayoutGeneratorType generatorType = (LayoutGeneratorType) System.Enum.Parse(typeof(LayoutGeneratorType), layoutDTO.generatorType);
        WorldStyle style = (WorldStyle) System.Enum.Parse(typeof(WorldStyle), layoutDTO.style);

        List<WorldConnection> worldConnections = GetWorldConnections(area);

        LayoutGenerator layoutGenerator = new CellularAutomataGenerator(seed, size, accessability, worldConnections);

        switch (generatorType)
        {
            case LayoutGeneratorType.CELLULAR_AUTOMATA:
                layoutGenerator = new CellularAutomataGenerator(seed, size, accessability, worldConnections);
                break;

            case LayoutGeneratorType.DRUNKARDS_WALK:
                layoutGenerator = new DrunkardsWalkGenerator(seed, size, accessability, worldConnections);
                break;

            case LayoutGeneratorType.ISLAND_CELLULAR_AUTOMATA:
                layoutGenerator = new IslandsGenerator(seed, size, accessability, worldConnections, RoomGenerator.CELLULAR_AUTOMATA);
                break;

            case LayoutGeneratorType.ISLAND_DRUNKARDS_WALK:
                layoutGenerator = new IslandsGenerator(seed, size, accessability, worldConnections, RoomGenerator.DRUNKARDS_WALK);
                break;
        }

        //generate layout
        layoutGenerator.GenerateLayout();
        CellType[,] baseLayout = layoutGenerator.GetLayout();

        //polish layout
        LayoutPolisher polisher = new LayoutPolisher(style, baseLayout);
        CellType[,] polishedLayout = polisher.Polish();

        //convert layout
        LayoutConverter converter = new SavannaConverter(polishedLayout);

        switch (style)
        {
            case WorldStyle.SAVANNA:
                converter = new SavannaConverter(polishedLayout);
                break;

            case WorldStyle.CAVE:
                converter = new CaveConverter(polishedLayout);
                break;

            case WorldStyle.BEACH:
                converter = new BeachConverter(polishedLayout);
                break;

            case WorldStyle.FOREST:
                converter = new ForestConverter(polishedLayout);
                break;
        }

        converter.Convert();
        TileSprite[,,] tileLayout = converter.GetTileSprites();

        Layout data = new Layout(area, tileLayout, polishedLayout, generatorType, seed, accessability, style);
        return data;
    }

    private static List<WorldConnection> GetWorldConnections(AreaInformation area)
    {
        List<WorldConnection> worldConnections = new List<WorldConnection>();

        if(!area.IsDungeon())
        {
            string path = "AreaInfo/World" + area.GetWorldIndex();
            TextAsset targetFile = Resources.Load<TextAsset>(path);
            string json = targetFile.text;
            AreaInformationDTO dto = AreaInformationDTO.CreateFromJSON(json);
            AreaInformationData data = AreaInformationData.ConvertDtoToData(dto);
            worldConnections = data.GetWorldConnections();
        }

        return worldConnections;
    }

    #region Getter And Setter
    public AreaInformation GetArea()
    {
        return area;
    }

    public void SetArea(AreaInformation area)
    {
        this.area = area;
    }
    
    public TileSprite[,,] GetTileSprites()
    {
        return tileSprites;
    }

    public void SetTileSprites(TileSprite[,,] tileSprites)
    {
        this.tileSprites = tileSprites;
    }

    public CellType[,] GetCellTypes()
    {
        return cellTypes;
    }

    public void SetTileSprites(CellType[,] cellTypes)
    {
        this.cellTypes = cellTypes;
    }

    public LayoutGeneratorType GetGeneratorType()
    {
        return generatorType;
    }

    public void SetGeneratorType(LayoutGeneratorType generatorType)
    {
        this.generatorType = generatorType;
    }

    public string GetSeed()
    {
        return seed;
    }

    public void SetSeed(string seed)
    {
        this.seed = seed;
    }

    public int GetAccessability()
    {
        return accessability;
    }

    public void SetAccessability(int accessability)
    {
        this.accessability = accessability;
    }

    public WorldStyle GetStyle()
    {
        return style;
    }

    public void SetStyle(WorldStyle style)
    {
        this.style = style;
    }
    #endregion
}
