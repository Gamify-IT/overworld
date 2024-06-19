using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStatisticData 
{
    private readonly string id;
    public AreaLocationDTO[] unlockedAreas;
    public AreaLocationDTO[] unlockedDungeons;
    public TeleporterDTO[] unlockedTeleporters;
    private readonly AreaLocationDTO currentArea;
    private readonly string userId;
    private readonly string username;
    private readonly int knowledge;
    private readonly int rewards;
    private bool showRewards;



    private string leagueOfPlayer;
   

    public PlayerStatisticData(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] completedDungeons, TeleporterDTO[] unlockedTeleporters, AreaLocationDTO currentArea, string userId, string username,  int knowledge, int rewards, bool showRewards)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = completedDungeons;
        this.unlockedTeleporters = unlockedTeleporters;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        
        this.knowledge = knowledge;
        this.rewards = rewards;
        this.leagueOfPlayer = calculateLeagueOfPlayer(rewards);
        this.showRewards = showRewards;        
    }

    public static PlayerStatisticData ConvertFromPlayerStatisticDTO(PlayerstatisticDTO statistic)
    {
        AreaLocationDTO currentArea = statistic.currentArea;
        string userId = statistic.userId;
        string username = statistic.username;
        int knowledge = statistic.knowledge;
        int rewards = statistic.rewards;
        string id = statistic.id;
        bool showRewards = statistic.showRewards;
        

        AreaLocationDTO[] unlockedAreas = new AreaLocationDTO[statistic.unlockedAreas.Length];
        AreaLocationDTO[] completedDungeons = new AreaLocationDTO[statistic.unlockedDungeons.Length];
        TeleporterDTO[] unlockedTeleporters = new TeleporterDTO[statistic.unlockedTeleporters.Length];

        for (int i = 0; i < statistic.unlockedAreas.Length; i++)
        {
            unlockedAreas[i] = statistic.unlockedAreas[i];
        }

        for (int i = 0; i < statistic.unlockedDungeons.Length; i++)
        {
            completedDungeons[i] = statistic.unlockedDungeons[i];
        }

        for (int i = 0; i < statistic.unlockedTeleporters.Length; i++)
        {
            unlockedTeleporters[i] = statistic.unlockedTeleporters[i];
        }

     
    PlayerStatisticData data = new PlayerStatisticData(id, unlockedAreas, completedDungeons,unlockedTeleporters,currentArea,userId,username,  knowledge, rewards, showRewards);
        return data;
    }


    public string calculateLeagueOfPlayer(int rewards)
    {
        if(rewards < 100)
        {
            return "Wanderer";
        }else if (rewards < 200)
        {
            return "Explorer";
        }else if (rewards < 300)
        {
            return "Pathfinder";
        }
        else
        {
            return "Trailblazer";
        }

        return "-";

    }

    #region Getter


    public string GetId()
    {
        return id;
    }

    public AreaLocationDTO[] GetUnlockedAreas()
    {
        return unlockedAreas;
    }

    public AreaLocationDTO[] GetUnlockedDungeons()
    {
        return unlockedDungeons;
    }

    public TeleporterDTO[] GetUnlockedTeleporters()
    {
        return unlockedTeleporters;
    }

    public AreaLocationDTO GetCurrentArea()
    {
        return currentArea;
    }

    public string GetUserId()
    {
        return userId;
    }

    public string GetUsername()
    {
        return username;
    }

    public int GetKnowledge()
    {
        return knowledge;
    }

    public int GetRewards()
    {
        return rewards;
    }

    public bool GetShowRewards()
    {
        return showRewards;
    }

    public string GetLeague()
    {
        return leagueOfPlayer;
    }

    public string GetWorld()
    {
        int index = this.currentArea.worldIndex;

        switch (index)
        {
            case 1:
                return "World 1";
            case 2:
                return "World 2";
            case 3:
                return "World 3";
            case 4:
                return "World 4";
            default:
                return "Error Text"; 
        }
    }

    #endregion



    // public List<string> GetMinigames()
    //{
    //  List<string> minigames = new List<string>();

    //minigames.Add("Bugfinder");
    //minigames.Add("Chickenshock");
    //minigames.Add("Crosswordpuzzle");
    //minigames.Add("Finitequiz");
    //minigames.Add("Memory");
    //minigames.Add("Towercrush");


    // return minigames;
    // }

    public void updateVisibility(bool update)
    {
        showRewards = update;
    }

    public static PlayerstatisticDTO ConvertToPlayerstatisticDTO(PlayerStatisticData playerStatisticData)
    {
        string id = playerStatisticData.GetId();
        string userId = playerStatisticData.GetUserId();
        string username = playerStatisticData.GetUsername();
        int knowledge = playerStatisticData.GetKnowledge();
        int rewards = playerStatisticData.GetRewards();
        bool showRewards = playerStatisticData.GetShowRewards();
        AreaLocationDTO currentArea = playerStatisticData.GetCurrentArea();
        AreaLocationDTO[] unlockedAreas = playerStatisticData.GetUnlockedAreas();
        AreaLocationDTO[] unlockedDungeons = playerStatisticData.GetUnlockedDungeons();
        TeleporterDTO[] unlockedTeleporters = playerStatisticData.GetUnlockedTeleporters();


       PlayerstatisticDTO playerStatistic = new PlayerstatisticDTO(id, unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, userId, username, knowledge, rewards, showRewards);


        return playerStatistic;
    }


}
