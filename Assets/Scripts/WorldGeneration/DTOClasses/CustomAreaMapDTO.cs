using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to transfer a <c>CustomAreaMapData</c> from and to the overworld backend
/// </summary>
[Serializable]
public class CustomAreaMapDTO
{
    #region Attributes
    public LayoutDTO layout;
    public MinigameSpotDTO[] minigameSpots;
    public NpcSpotDTO[] npcSpots;
    public BookSpotDTO[] bookSpots;
    public BarrierSpotDTO[] barrierSpots;
    public TeleporterSpotDTO[] teleporterSpots;
    public SceneTransitionSpotDTO[] sceneTransitionSpots;
    #endregion

    #region Constructors
    public CustomAreaMapDTO() { }

    public CustomAreaMapDTO(LayoutDTO layout, MinigameSpotDTO[] minigameSpots, NpcSpotDTO[] npcSpots, BookSpotDTO[] bookSpots, BarrierSpotDTO[] barrierSpots, TeleporterSpotDTO[] teleporterSpots, SceneTransitionSpotDTO[] sceneTransitionSpots)
    {
        this.layout = layout;
        this.minigameSpots = minigameSpots;
        this.npcSpots = npcSpots;
        this.bookSpots = bookSpots;
        this.barrierSpots = barrierSpots;
        this.teleporterSpots = teleporterSpots;
        this.sceneTransitionSpots = sceneTransitionSpots;
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
        LayoutDTO layout = LayoutDTO.ConvertDataToDto(areaMapData.GetLayout());

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

        CustomAreaMapDTO data = new CustomAreaMapDTO(layout, minigameSpots, npcSpots, bookSpots, barrierSpots, teleporterSpots, sceneTransitionSpots);
        return data;
    }
}
