using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a generated area, including the layout and object spots
/// </summary>
public class CustomAreaMapData
{
    #region Attributes
    private Layout layout;
    private List<MinigameSpotData> minigameSpots;
    private List<NpcSpotData> npcSpots;
    private List<BookSpotData> bookSpots;
    private List<BarrierSpotData> barrierSpots;
    private List<TeleporterSpotData> teleporterSpots;
    private List<SceneTransitionSpotData> sceneTransitionSpots;
    #endregion

    public CustomAreaMapData()
    {
        minigameSpots = new List<MinigameSpotData>();
        npcSpots = new List<NpcSpotData>();
        bookSpots = new List<BookSpotData>();
        barrierSpots = new List<BarrierSpotData>();
        teleporterSpots = new List<TeleporterSpotData>();
        sceneTransitionSpots = new List<SceneTransitionSpotData>();
    }

    public CustomAreaMapData(Layout layout)
    {
        this.layout = layout;
        minigameSpots = new List<MinigameSpotData>();
        npcSpots = new List<NpcSpotData>();
        bookSpots = new List<BookSpotData>();
        barrierSpots = new List<BarrierSpotData>();
        teleporterSpots = new List<TeleporterSpotData>();
        sceneTransitionSpots = new List<SceneTransitionSpotData>();
    }

    public CustomAreaMapData(Layout layout, List<MinigameSpotData> minigameSpots, List<NpcSpotData> npcSpots, List<BookSpotData> bookSpots, List<BarrierSpotData> barrierSpots, List<TeleporterSpotData> teleporterSpots, List<SceneTransitionSpotData> sceneTransitionSpots)
    {
        this.layout = layout;
        this.minigameSpots = minigameSpots;
        this.npcSpots = npcSpots;
        this.bookSpots = bookSpots;
        this.barrierSpots = barrierSpots;
        this.teleporterSpots = teleporterSpots;
        this.sceneTransitionSpots = sceneTransitionSpots;
    }

    /// <summary>
    ///     This function converts a <c>CustomAreaMapDTO</c> object into a <c>CustomAreaMapData</c> instance
    /// </summary>
    /// <param name="areaMapDTO">The <c>CustomAreaMapDTO</c> object to convert</param>
    /// <returns></returns>
    public static CustomAreaMapData ConvertDtoToData(CustomAreaMapDTO areaMapDTO)
    {
        Layout layout = Layout.ConvertDtoToData(areaMapDTO.layout);

        List<MinigameSpotData> minigameSpots = new List<MinigameSpotData>();
        for(int i = 0; i  <areaMapDTO.minigameSpots.Length; i++)
        {
            minigameSpots.Add(MinigameSpotData.ConvertDtoToData(areaMapDTO.minigameSpots[i]));
        }

        List<NpcSpotData> npcSpots = new List<NpcSpotData>();
        for(int i = 0; i < areaMapDTO.npcSpots.Length; i++)
        {
            npcSpots.Add(NpcSpotData.ConvertDtoToData(areaMapDTO.npcSpots[i]));
        }

        List<BookSpotData> bookSpots = new List<BookSpotData>();
        for(int i = 0; i < areaMapDTO.bookSpots.Length; i++)
        {
            bookSpots.Add(BookSpotData.ConvertDtoToData(areaMapDTO.bookSpots[i]));
        }

        List<BarrierSpotData> barrierSpots = new List<BarrierSpotData>();
        for(int i = 0; i < areaMapDTO.barrierSpots.Length; i++)
        {
            barrierSpots.Add(BarrierSpotData.ConvertDtoToData(areaMapDTO.barrierSpots[i]));
        }

        List<TeleporterSpotData> teleporterSpots = new List<TeleporterSpotData>();
        for(int i = 0; i < areaMapDTO.teleporterSpots.Length; i++)
        {
            teleporterSpots.Add(TeleporterSpotData.ConvertDtoToData(areaMapDTO.teleporterSpots[i]));
        }

        List<SceneTransitionSpotData> sceneTransitionSpots = new List<SceneTransitionSpotData>();
        for(int i = 0; i < areaMapDTO.sceneTransitionSpots.Length; i++)
        {
            sceneTransitionSpots.Add(SceneTransitionSpotData.ConvertDtoToData(areaMapDTO.sceneTransitionSpots[i]));
        }

        CustomAreaMapData data = new CustomAreaMapData(layout, minigameSpots, npcSpots, bookSpots, barrierSpots, teleporterSpots, sceneTransitionSpots);
        return data;
    }

    /// <summary>
    ///     This function returns the size of the world
    /// </summary>
    /// <returns>A <c>Vector2Int</c> containing the size of the world</returns>
    public Vector2Int GetSize()
    {
        int sizeX = layout.GetTileSprites().GetLength(0);
        int sizeY = layout.GetTileSprites().GetLength(1);

        return new Vector2Int(sizeX, sizeY);
    }

    #region GetterAndSetter
    public Layout GetLayout()
    {
        return layout;
    }

    public void SetLayout(Layout layout)
    {
        this.layout = layout;
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
