using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStatisticData 
{
    private readonly string id;
    private readonly List<AreaLocationDTO> unlockedAreas;
    private readonly List<AreaLocationDTO> completedDungeons;
    private readonly HashSet<TeleporterDTO> unlockedTeleporters;
    private readonly AreaLocationDTO currentArea;
    private readonly string userId;
    private readonly string username;
    private readonly long knowledge;
    private readonly int rewards;
    private string Wanderer;
    private string Explorer;
    private string Pathfinder;
    private string Trailblazer;
    List<string> leagues;

    public PlayerStatisticData(String id, List<AreaLocationDTO> unlockedAreas, List<AreaLocationDTO> completedDungeons, HashSet<TeleporterDTO> unlockedTeleporters, AreaLocationDTO currentArea, string userId, string username,  long knowledge, int rewards)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.completedDungeons = completedDungeons;
        this.unlockedTeleporters = unlockedTeleporters;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        
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
        string id = statistic.id;
       
        List<AreaLocationDTO> unlockedAreas = new List<AreaLocationDTO>();
        List<AreaLocationDTO> completedDungeons = new List<AreaLocationDTO>();
        HashSet<TeleporterDTO> unlockedTeleporters = new HashSet<TeleporterDTO>();

        foreach (AreaLocationDTO unlockedArea in statistic.unlockedAreas)
        {
            unlockedAreas.Add(unlockedArea);
        }
        foreach (AreaLocationDTO completedDungeon in statistic.unlockedDungeons)
        {
            completedDungeons.Add(completedDungeon);
        }
        foreach (TeleporterDTO unlockedTeleporter in statistic.unlockedTeleporters)
        {
            unlockedTeleporters.Add(unlockedTeleporter);
        }



        PlayerStatisticData data = new PlayerStatisticData(id, unlockedAreas, completedDungeons,unlockedTeleporters,currentArea,userId,username,  knowledge, rewards);
        return data;
    }
    
    #region Getter


    public int GetRewards()
    {
        return rewards;
    }

    public string GetUsername()
    {
        return username;
    }


    //league muss noch definiert werden?
    public List<string> GetLeagues()
    {
        leagues.Add(Wanderer);
        leagues.Add(Explorer);
        leagues.Add(Pathfinder);
        leagues.Add(Trailblazer);

        return leagues;
    }

    public List<string> GetWorldNames()
    {
        List<string> worldNames = new List<string>();
        worldNames.Add("World 1");
        worldNames.Add("World 2");
        worldNames.Add("World 3");
        worldNames.Add("World 4");

        return worldNames;
    }


    public List<string> GetWorlds()
    {
        return null;
    }

    public List<string> GetMinigames()
    {
        return null;
    }
    #endregion

}
