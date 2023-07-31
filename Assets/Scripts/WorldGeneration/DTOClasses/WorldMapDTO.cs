using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve world map data from Get Requests.
/// </summary>
[Serializable]
public class WorldMapDTO
{
    #region Attributes
    public string[,,] tiles;
    public MinigameSpotDTO[] minigameSpots;
    public NpcSpotDTO[] npcSpots;
    public BookSpotDTO[] bookSpots;
    public BarrierSpotDTO[] barrierSpots;
    public TeleporterSpotDTO[] teleporterSpots;
    public SceneTransitionSpotDTO[] sceneTransitions;
    #endregion

    #region Constructors
    public WorldMapDTO() { }

    public WorldMapDTO(string[,,] tiles, MinigameSpotDTO[] minigameSpots, NpcSpotDTO[] npcSpots, BookSpotDTO[] bookSpots, BarrierSpotDTO[] barrierSpots, TeleporterSpotDTO[] teleporterSpots, SceneTransitionSpotDTO[] sceneTransitions)
    {
        this.tiles = tiles;
        this.minigameSpots = minigameSpots;
        this.npcSpots = npcSpots;
        this.bookSpots = bookSpots;
        this.barrierSpots = barrierSpots;
        this.teleporterSpots = teleporterSpots;
        this.sceneTransitions = sceneTransitions;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>WorldMapDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>WorldMapDTO</c> object containing the data</returns>
    public static WorldMapDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<WorldMapDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>WorldMapData</c> object into a <c>WorldMapDTO</c> instance
    /// </summary>
    /// <param name="worldMapData">The <c>WorldMapData</c> object to convert</param>
    /// <returns></returns>
    public static WorldMapDTO ConvertDataToDto(WorldMapData worldMapData)
    {
        string[,,] tiles = worldMapData.GetTiles();

        List<MinigameSpotDTO> minigameSpotDtos = new List<MinigameSpotDTO>();
        foreach(MinigameSpotData minigameSpotData in worldMapData.GetMinigameSpots())
        {
            minigameSpotDtos.Add(MinigameSpotDTO.ConvertDataToDto(minigameSpotData));
        }
        MinigameSpotDTO[] minigameSpots = minigameSpotDtos.ToArray();

        List<NpcSpotDTO> npcSpotDtos = new List<NpcSpotDTO>();
        foreach (NpcSpotData npcSpotData in worldMapData.GetNpcSpots())
        {
            npcSpotDtos.Add(NpcSpotDTO.ConvertDataToDto(npcSpotData));
        }
        NpcSpotDTO[] npcSpots = npcSpotDtos.ToArray();

        List<BookSpotDTO> bookSpotDtos = new List<BookSpotDTO>();
        foreach (BookSpotData bookSpotData in worldMapData.GetBookSpots())
        {
            bookSpotDtos.Add(BookSpotDTO.ConvertDataToDto(bookSpotData));
        }
        BookSpotDTO[] bookSpots = bookSpotDtos.ToArray();

        List<BarrierSpotDTO> barrierSpotDtos = new List<BarrierSpotDTO>();
        foreach (BarrierSpotData barrierSpotData in worldMapData.GetBarrierSpots())
        {
            barrierSpotDtos.Add(BarrierSpotDTO.ConvertDataToDto(barrierSpotData));
        }
        BarrierSpotDTO[] barrierSpots = barrierSpotDtos.ToArray();

        List<TeleporterSpotDTO> teleporterSpotDtos = new List<TeleporterSpotDTO>();
        foreach (TeleporterSpotData teleporterSpotData in worldMapData.GetTeleporterSpots())
        {
            teleporterSpotDtos.Add(TeleporterSpotDTO.ConvertDataToDto(teleporterSpotData));
        }
        TeleporterSpotDTO[] teleporterSpots = teleporterSpotDtos.ToArray();

        List<SceneTransitionSpotDTO> sceneTransitionSpotDtos = new List<SceneTransitionSpotDTO>();
        foreach (SceneTransitionSpotData sceneTransitionSpotData in worldMapData.GetSceneTransitionSpots())
        {
            sceneTransitionSpotDtos.Add(SceneTransitionSpotDTO.ConvertDataToDto(sceneTransitionSpotData));
        }
        SceneTransitionSpotDTO[] sceneTransitionSpots = sceneTransitionSpotDtos.ToArray();

        WorldMapDTO data = new WorldMapDTO(tiles, minigameSpots, npcSpots, bookSpots, barrierSpots, teleporterSpots, sceneTransitionSpots);
        return data;
    }
}
