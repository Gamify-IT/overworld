using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class contains all data to manage a world map
/// </summary>
public class WorldMapData
{
    #region Attributes
    //grid data
    private List<MinigameSpotData> minigameSpots;
    private List<NpcSpotData> npcSpots;
    private List<BookSpotData> bookSpots;
    private List<BarrierSpotData> barrierSpots;
    private List<TeleporterSpotData> teleporterSpots;
    private List<SceneTransitionSpotData> sceneTransitionSpots;
    #endregion

    public WorldMapData(List<MinigameSpotData> minigameSpots, List<NpcSpotData> npcSpots, List<BookSpotData> bookSpots, List<BarrierSpotData> barrierSpots, List<TeleporterSpotData> teleporterSpots, List<SceneTransitionSpotData> sceneTransitionSpots)
    {
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

        WorldMapData data = new WorldMapData(minigameSpots, npcSpots, bookSpots, barrierSpots, teleporterSpots, sceneTransitionSpots);
        return data;
    }

    #region GetterAndSetter
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
