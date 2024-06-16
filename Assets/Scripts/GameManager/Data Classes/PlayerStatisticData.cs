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
    private readonly long knowledge;
    private readonly int rewards;

    

    private string leagueOfPlayer;
   

    public PlayerStatisticData(String id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] completedDungeons, TeleporterDTO[] unlockedTeleporters, AreaLocationDTO currentArea, string userId, string username,  long knowledge, int rewards)
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
        
    }

    public static PlayerStatisticData ConvertFromPlayerStatisticDTO(PlayerstatisticDTO statistic)
    {
        AreaLocationDTO currentArea = statistic.currentArea;
        string userId = statistic.userId;
        string username = statistic.username;
        long knowledge = statistic.knowledge;
        int rewards = statistic.rewards;
        string id = statistic.id;

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

     
    PlayerStatisticData data = new PlayerStatisticData(id, unlockedAreas, completedDungeons,unlockedTeleporters,currentArea,userId,username,  knowledge, rewards);
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


    public int GetRewards()
    {
        return rewards;
    }

    public string GetUsername()
    {
        return username;
    }


    // iteriert durch jedes einzelne Objekt in der DataListe und führt für jedes Objekt da drin die GetLeagues() auf (sehr unnötig, Lösung wird gesucht)

    public string GetLeague()
    {

        return this.leagueOfPlayer;
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
    #endregion

}
