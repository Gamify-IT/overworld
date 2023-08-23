using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a world map
/// </summary>
public class WorldMapData
{
    #region Attributes
    private AreaInformation area;
    private string[,,] tiles;
    private Vector2Int offset;
    private WorldStyle style;
    private List<WorldConnection> worldConnections;
    private List<MinigameSpotData> minigameSpots;
    private List<NpcSpotData> npcSpots;
    private List<BookSpotData> bookSpots;
    private List<BarrierSpotData> barrierSpots;
    private List<TeleporterSpotData> teleporterSpots;
    private List<SceneTransitionSpotData> sceneTransitionSpots;
    #endregion

    public WorldMapData()
    {
        tiles = new string[0,0,0];
        minigameSpots = new List<MinigameSpotData>();
        npcSpots = new List<NpcSpotData>();
        bookSpots = new List<BookSpotData>();
        barrierSpots = new List<BarrierSpotData>();
        teleporterSpots = new List<TeleporterSpotData>();
        sceneTransitionSpots = new List<SceneTransitionSpotData>();
    }

    public WorldMapData(AreaInformation area, string[,,] tiles, Vector2Int offset, WorldStyle style, List<WorldConnection> worldConnections, List<MinigameSpotData> minigameSpots, List<NpcSpotData> npcSpots, List<BookSpotData> bookSpots, List<BarrierSpotData> barrierSpots, List<TeleporterSpotData> teleporterSpots, List<SceneTransitionSpotData> sceneTransitionSpots)
    {
        this.area = area;
        this.tiles = tiles;
        this.offset = offset;
        this.style = style;
        this.worldConnections = worldConnections;
        this.minigameSpots = minigameSpots;
        this.npcSpots = npcSpots;
        this.bookSpots = bookSpots;
        this.barrierSpots = barrierSpots;
        this.teleporterSpots = teleporterSpots;
        this.sceneTransitionSpots = sceneTransitionSpots;
    }

    /// <summary>
    ///     This function converts a <c>WorldMapDTO</c> object into a <c>WorldMapData</c> instance
    /// </summary>
    /// <param name="worldMapDTO">The <c>WorldMapDTO</c> object to convert</param>
    /// <returns></returns>
    public static WorldMapData ConvertDtoToData(WorldMapDTO worldMapDTO)
    {
        Optional<int> dungeonIndex = new Optional<int>();
        if(worldMapDTO.area.dungeonIndex != 0)
        {
            dungeonIndex.SetValue(worldMapDTO.area.dungeonIndex);
        }
        AreaInformation area = new AreaInformation(worldMapDTO.area.worldIndex, dungeonIndex);

        string[,,] tiles = Layout.ConvertLayoutToArray(worldMapDTO.layout);

        Vector2Int offset = new Vector2Int((int) worldMapDTO.offset.x, (int) worldMapDTO.offset.y);

        WorldStyle style = (WorldStyle) System.Enum.Parse(typeof(WorldStyle) , worldMapDTO.style);

        List<WorldConnection> worldConnections = new List<WorldConnection>();
        foreach(WorldConnectionDTO connection in worldMapDTO.worldConnections)
        {
            WorldConnection worldConnection = WorldConnection.ConvertDtoToData(connection);
            worldConnections.Add(worldConnection);
        }

        List<MinigameSpotData> minigameSpots = new List<MinigameSpotData>();
        for(int i = 0; i  <worldMapDTO.minigameSpots.Length; i++)
        {
            minigameSpots.Add(MinigameSpotData.ConvertDtoToData(worldMapDTO.minigameSpots[i]));
        }

        List<NpcSpotData> npcSpots = new List<NpcSpotData>();
        for(int i = 0; i < worldMapDTO.npcSpots.Length; i++)
        {
            npcSpots.Add(NpcSpotData.ConvertDtoToData(worldMapDTO.npcSpots[i]));
        }

        List<BookSpotData> bookSpots = new List<BookSpotData>();
        for(int i = 0; i < worldMapDTO.bookSpots.Length; i++)
        {
            bookSpots.Add(BookSpotData.ConvertDtoToData(worldMapDTO.bookSpots[i]));
        }

        List<BarrierSpotData> barrierSpots = new List<BarrierSpotData>();
        for(int i = 0; i < worldMapDTO.barrierSpots.Length; i++)
        {
            barrierSpots.Add(BarrierSpotData.ConvertDtoToData(worldMapDTO.barrierSpots[i]));
        }

        List<TeleporterSpotData> teleporterSpots = new List<TeleporterSpotData>();
        for(int i = 0; i < worldMapDTO.teleporterSpots.Length; i++)
        {
            teleporterSpots.Add(TeleporterSpotData.ConvertDtoToData(worldMapDTO.teleporterSpots[i]));
        }

        List<SceneTransitionSpotData> sceneTransitionSpots = new List<SceneTransitionSpotData>();
        for(int i = 0; i < worldMapDTO.sceneTransitions.Length; i++)
        {
            sceneTransitionSpots.Add(SceneTransitionSpotData.ConvertDtoToData(worldMapDTO.sceneTransitions[i]));
        }

        WorldMapData data = new WorldMapData(area, tiles, offset, style, worldConnections, minigameSpots, npcSpots, bookSpots, barrierSpots, teleporterSpots, sceneTransitionSpots);
        return data;
    }

    /// <summary>
    ///     This function returns the size of the world
    /// </summary>
    /// <returns>A <c>Vector2Int</c> containing the size of the world</returns>
    public Vector2Int GetSize()
    {
        return new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));
    }

    #region GetterAndSetter
    public AreaInformation GetArea()
    {
        return area;
    }

    public void SetArea(AreaInformation area)
    {
        this.area = area;
    }

    public string[,,] GetTiles()
    {
        return tiles;
    }

    public void SetTiles(string[,,] tiles)
    {
        this.tiles = tiles;
    }

    public Vector2Int GetOffset()
    {
        return offset;
    }

    public void SetOffset(Vector2Int offset)
    {
        this.offset = offset;
    }

    public WorldStyle GetWorldStyle()
    {
        return style;
    }

    public void SetWorldStyle(WorldStyle style)
    {
        this.style = style;
    }

    public List<WorldConnection> GetWorldConnections()
    {
        return worldConnections;
    }

    public void SetWorldConnections(List<WorldConnection> worldConnections)
    {
        this.worldConnections = worldConnections;
    }

    public List<MinigameSpotData> GetMinigameSpots()
    {
        return minigameSpots;
    }

    public void SetMinigameSpots(List<MinigameSpotData> minigameSpots)
    {
        this.minigameSpots = minigameSpots;
    }

    public List<NpcSpotData> GetNpcSpots()
    {
        return npcSpots;
    }

    public void SetNpcSpots(List<NpcSpotData> npcSpots)
    {
        this.npcSpots = npcSpots;
    }

    public List<BookSpotData> GetBookSpots()
    {
        return bookSpots;
    }

    public void SetBookSpots(List<BookSpotData> bookSpots)
    {
        this.bookSpots = bookSpots;
    }

    public List<BarrierSpotData> GetBarrierSpots()
    {
        return barrierSpots;
    }

    public void SetBarrierSpots(List<BarrierSpotData> barrierSpots)
    {
        this.barrierSpots = barrierSpots;
    }

    public List<TeleporterSpotData> GetTeleporterSpots()
    {
        return teleporterSpots;
    }

    public void SetTeleporterSpots(List<TeleporterSpotData> teleporterSpots)
    {
        this.teleporterSpots = teleporterSpots;
    }

    public List<SceneTransitionSpotData> GetSceneTransitionSpots()
    {
        return sceneTransitionSpots;
    }

    public void SetSceneTransitionSpots(List<SceneTransitionSpotData> sceneTransitionSpots)
    {
        this.sceneTransitionSpots = sceneTransitionSpots;
    }
    #endregion
}
