using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatisticData 
{

    private readonly List<AreaLocationDTO> unlockedAreas;
    private readonly List<AreaLocationDTO> completedDungeons;
    private readonly HashSet<TeleporterDTO> unlockedTeleporters;
    private readonly AreaLocationDTO currentArea;
    private readonly string userId;
    private readonly string username;
    private readonly LocalDateTime created;
    private readonly LocalDateTime lastActive;
    private readonly long knowledge;
    private readonly int rewards;

    public PlayerStatisticData(List<AreaLocationDTO> unlockedAreas, List<AreaLocationDTO> completedDungeons, HashSet<TeleporterDTO> unlockedTeleporters, AreaLocationDTO currentArea, string userId, string username, LocalDateTime created, LocalDateTime lastActive, long knowledge, int rewards)
    {
        this.unlockedAreas = unlockedAreas;
        this.completedDungeons = completedDungeons;
        this.unlockedTeleporters = unlockedTeleporters;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        this.created = created;
        this.lastActive = lastActive;
        this.knowledge = knowledge;
        this.rewards = rewards;
    }

    public static PlayerStatisticData ConvertFromPlayerStatisticDTO(PlayerstatisticDTO statistic)
    {
        AreaLocationDTO currentArea = statistic.currentArea;
        string userId = statistic.userId;
        string username = statistic.username;
        long knowledge = statistic.knowledge;
        int rewards = statistic.rewards;
        LocalDateTime created = statistic.created;
        LocalDateTime lastActive = statistic.lastActive;
        List<AreaLocationDTO> unlockedAreas = new List<AreaLocationDTO>();
        List<AreaLocationDTO> completedDungeons = new List<AreaLocationDTO>();
        Set<TeleporterDTO> unlockedTeleporters = new HashSet<TeleporterDTO>();

        foreach (AreaLocationDTO unlockedArea in statistic.unlockedAreas)
        {
            unlockedAreas.Add(unlockedArea);
        }
        foreach (AreaLocationDTO completedDungeon in statistic.completedDungeons)
        {
            completedDungeons.Add(completedDungeon);
        }
        foreach (TeleporterDTO unlockedTeleporter in statistic.unlockedTeleporters)
        {
            unlockedTeleporters.Add(unlockedTeleporter);
        }



        PlayerStatisticData data = new PlayerStatisticData(unlockedAreas, completedDungeons,unlockedTeleporters,currentArea,userId,username, created, lastActive, knowledge,  knowledge, rewards);
        return data;
    }


}
