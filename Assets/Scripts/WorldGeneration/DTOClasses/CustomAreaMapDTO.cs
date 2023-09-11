using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve world map data from Get Requests.
/// </summary>
[Serializable]
public class CustomAreaMapDTO
{
    #region Attributes
    public Layout layout;
    public string style;
    public MinigameSpotDTO[] minigameSpots;
    public NpcSpotDTO[] npcSpots;
    public BookSpotDTO[] bookSpots;
    public BarrierSpotDTO[] barrierSpots;
    public TeleporterSpotDTO[] teleporterSpots;
    public SceneTransitionSpotDTO[] sceneTransitions;
    #endregion

    #region Constructors
    public CustomAreaMapDTO() { }

    public CustomAreaMapDTO(Layout layout, string style, MinigameSpotDTO[] minigameSpots, NpcSpotDTO[] npcSpots, BookSpotDTO[] bookSpots, BarrierSpotDTO[] barrierSpots, TeleporterSpotDTO[] teleporterSpots, SceneTransitionSpotDTO[] sceneTransitions)
    {
        this.layout = layout;
        this.style = style;
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
    public static CustomAreaMapDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<CustomAreaMapDTO>(jsonString);
    }

    /// <summary>
    ///     This function converts a <c>CustomAreaMapData</c> object into a <c>CustomAreaMapDTO</c> instance
    /// </summary>
    /// <param name="areaMapData">The <c>CustomAreaMapData</c> object to convert</param>
    /// <returns></returns>
    public static CustomAreaMapDTO ConvertDataToDto(CustomAreaMapData areaMapData)
    {
        string[,,] tiles = areaMapData.GetTiles();
        Layout layout = Layout.ConvertArrayToLayout(tiles);

        string style = areaMapData.GetWorldStyle().ToString();

        List<MinigameSpotDTO> minigameSpotDtos = new List<MinigameSpotDTO>();
        foreach(MinigameSpotData minigameSpotData in areaMapData.GetMinigameSpots())
        {
            minigameSpotDtos.Add(MinigameSpotDTO.ConvertDataToDto(minigameSpotData));
        }
        MinigameSpotDTO[] minigameSpots = minigameSpotDtos.ToArray();

        List<NpcSpotDTO> npcSpotDtos = new List<NpcSpotDTO>();
        foreach (NpcSpotData npcSpotData in areaMapData.GetNpcSpots())
        {
            npcSpotDtos.Add(NpcSpotDTO.ConvertDataToDto(npcSpotData));
        }
        NpcSpotDTO[] npcSpots = npcSpotDtos.ToArray();

        List<BookSpotDTO> bookSpotDtos = new List<BookSpotDTO>();
        foreach (BookSpotData bookSpotData in areaMapData.GetBookSpots())
        {
            bookSpotDtos.Add(BookSpotDTO.ConvertDataToDto(bookSpotData));
        }
        BookSpotDTO[] bookSpots = bookSpotDtos.ToArray();

        List<BarrierSpotDTO> barrierSpotDtos = new List<BarrierSpotDTO>();
        foreach (BarrierSpotData barrierSpotData in areaMapData.GetBarrierSpots())
        {
            barrierSpotDtos.Add(BarrierSpotDTO.ConvertDataToDto(barrierSpotData));
        }
        BarrierSpotDTO[] barrierSpots = barrierSpotDtos.ToArray();

        List<TeleporterSpotDTO> teleporterSpotDtos = new List<TeleporterSpotDTO>();
        foreach (TeleporterSpotData teleporterSpotData in areaMapData.GetTeleporterSpots())
        {
            teleporterSpotDtos.Add(TeleporterSpotDTO.ConvertDataToDto(teleporterSpotData));
        }
        TeleporterSpotDTO[] teleporterSpots = teleporterSpotDtos.ToArray();

        List<SceneTransitionSpotDTO> sceneTransitionSpotDtos = new List<SceneTransitionSpotDTO>();
        foreach (SceneTransitionSpotData sceneTransitionSpotData in areaMapData.GetSceneTransitionSpots())
        {
            sceneTransitionSpotDtos.Add(SceneTransitionSpotDTO.ConvertDataToDto(sceneTransitionSpotData));
        }
        SceneTransitionSpotDTO[] sceneTransitionSpots = sceneTransitionSpotDtos.ToArray();

        CustomAreaMapDTO data = new CustomAreaMapDTO(layout, style, minigameSpots, npcSpots, bookSpots, barrierSpots, teleporterSpots, sceneTransitionSpots);
        return data;
    }
}
