using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout
{
    private AreaInformation area;
    private TileSprite[,,] tiles;
    private LayoutGeneratorType generatorType;
    private string seed;
    private int accessability;
    private WorldStyle style;

    public Layout(AreaInformation area, TileSprite[,,] tiles, LayoutGeneratorType generatorType, string seed, int accessability, WorldStyle style)
    {
        this.area = area;
        this.tiles = tiles;
        this.generatorType = generatorType;
        this.seed = seed;
        this.accessability = accessability;
        this.style = style;
    }

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
            TileSprite[,,] dummyTiles = new TileSprite[0, 0, 0];
            LayoutGeneratorType dummyGenerator = LayoutGeneratorType.CELLULAR_AUTOMATA;
            string dummySeed = "";
            int dummyAccessiblity = 0;
            WorldStyle dummyStyle = WorldStyle.CUSTOM;

            Layout dummyLayout = new Layout(dummyArea, dummyTiles, dummyGenerator, dummySeed, dummyAccessiblity, dummyStyle);
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

        Layout data = new Layout(area, tileLayout, generatorType, seed, accessability, style);
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
    
    public TileSprite[,,] GetTiles()
    {
        return tiles;
    }

    public void SetTiles(TileSprite[,,] tiles)
    {
        this.tiles = tiles;
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
