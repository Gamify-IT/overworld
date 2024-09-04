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
    private string lastActive;
    private float logoutPositionX;
    private float logoutPositionY;
    private string logoutScene;
    private int currentCharacterIndex;
    private readonly int knowledge;
    private int volumeLevel;
    private readonly int rewards;
    private bool visibility;
    private int credit;
    private string pseudonym;
    private string leagueOfPlayer;
    private bool updatedCredit = false;
    private bool updatedPseudonym= false;
    private bool updatedVisibility = false;
   

    public PlayerStatisticData(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] completedDungeons, TeleporterDTO[] unlockedTeleporters, AreaLocationDTO currentArea, string userId, string username, string lastActive, float logoutPositionX, float logoutPositionY,
         string logoutScene, int currentCharacterIndex, int volumeLevel, int knowledge, int rewards, bool visibility,int credit, string pseudonym)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = completedDungeons;
        this.unlockedTeleporters = unlockedTeleporters;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        this.lastActive = lastActive;
        this.logoutPositionX = logoutPositionX;
        this.logoutPositionY = logoutPositionY;
        this.logoutScene = logoutScene;
        this.currentCharacterIndex = currentCharacterIndex;
        this.volumeLevel = volumeLevel;
        this.knowledge = knowledge;
        this.rewards = rewards;
        this.leagueOfPlayer = calculateLeagueOfPlayer(rewards);
        this.visibility = visibility;
        this.credit = credit;
        this.pseudonym = pseudonym;
    }

    public static PlayerStatisticData ConvertFromPlayerStatisticDTO(PlayerstatisticDTO statistic)
    {
        AreaLocationDTO currentArea = statistic.currentArea;
        string userId = statistic.userId;
        string username = statistic.username;
        string lastActive = statistic.lastActive;

        float logoutPositionX = statistic.logoutPositionX;
        float logoutPositionY = statistic.logoutPositionY;
        string logoutScene = statistic.logoutScene;
        int currentCharacterIndex = statistic.currentCharacterIndex;
        int volumeLevel = statistic.volumeLevel;

        int knowledge = statistic.knowledge;
        int rewards = statistic.rewards;
        string id = statistic.id;
        bool visibility = statistic.visibility;
        int credit = statistic.credit;
        string pseudonym = statistic.pseudonym;



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

     
    PlayerStatisticData data = new PlayerStatisticData(id, unlockedAreas, completedDungeons,unlockedTeleporters,currentArea,userId,username, lastActive,
            logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex, volumeLevel, knowledge, rewards, visibility,credit, pseudonym);
        return data;
    }




    public bool updateCredit(int price)
    {
        updatedCredit = true;
        credit = this.credit - price;
        if(credit > 0)
        {
            return true;
        }
        return false;
    }

    public bool updatePseudonym(string name)
    {
        updatedPseudonym = true;
       
        if (name != null)
        {
            pseudonym = name;
            return true;
        }
        return false;
    }

    public bool updateVisibility(bool visibility)
    {
        updatedVisibility = true;

        if (visibility)
        {
            this.visibility = true;
        }
        else
        {
            this.visibility = false;
            
        }

        return true;
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

    public bool creditIsUpdated()
    {
        return updatedCredit;
    }

    public bool PseudonymIsUpdated()
    {
        return updatedPseudonym;
    }

    public bool VisibilityIsUpdated()
    {
        return updatedVisibility;
    }


    #region Getter

    public string GetLastActive()
    {
        return lastActive;
    }

    public void SetLastActive(string lastActive)
    {
        this.lastActive = lastActive;
    }

    public float GetLogoutPositionX()
    {
        return logoutPositionX;
    }

    public void SetLogoutPositionX(float xPosition)
    {
        logoutPositionX = xPosition;
    }

    public float GetLogoutPositionY()
    {
        return logoutPositionY;
    }

    public void SetLogoutPositionY(float YPosition)
    {
        logoutPositionY = YPosition;
    }

    /// <summary>
    ///     Gets the name of the current scene
    /// </summary>
    /// <returns>scene name</returns>
    public string GetLogoutScene()
    {
        return logoutScene;
    }

    /// <summary>
    ///     Sets the name of the current scene
    /// </summary>
    /// <param name="sceneName">scene name</param>
    public void SetLogoutScene(string logoutScene)
    {
        this.logoutScene = logoutScene;
    }

    /// <summary>
    ///     Gets the character index of the currently selected character by the player
    /// </summary>
    /// <returns>index of the character position in the list</returns>
    public int GetCurrentCharacterIndex()
    {
        return currentCharacterIndex;
    }

    /// <summary>
    ///     Updates the character index if the character is changed by the player
    /// </summary>
    /// <param name="index">index of the newly, selected character</param>
    public void SetCurrentCharacterIndex(int index)
    {
        currentCharacterIndex = index;
    }


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

    public bool GetVisibility()
    {
        return visibility;
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

    public string GetPseudonym()
    {
        return pseudonym;
    }

    public int GetCredit()
    {
        return credit;
    }

    public int GetVolumeLevel()
    {
        return volumeLevel;
    }

    #endregion

  

 
    public static PlayerstatisticDTO ConvertToPlayerstatisticDTO(PlayerStatisticData playerStatisticData)
    {
        string id = playerStatisticData.GetId();
        string userId = playerStatisticData.GetUserId();
        string username = playerStatisticData.GetUsername();
        string lastActive = playerStatisticData.GetLastActive();
        float logoutPositionX = playerStatisticData.GetLogoutPositionX();
        float logoutPositionY = playerStatisticData.GetLogoutPositionY();
        string logoutScene = playerStatisticData.GetLogoutScene();
        int currentCharacterIndex = playerStatisticData.GetCurrentCharacterIndex();
        int knowledge = playerStatisticData.GetKnowledge();
        int volumeLevel = playerStatisticData.GetVolumeLevel();

        int rewards = playerStatisticData.GetRewards();
        bool visibility = playerStatisticData.GetVisibility();
        int credit = playerStatisticData.GetCredit();
        string pseudonym = playerStatisticData.GetPseudonym();
        AreaLocationDTO currentArea = playerStatisticData.GetCurrentArea();
        AreaLocationDTO[] unlockedAreas = playerStatisticData.GetUnlockedAreas();
        AreaLocationDTO[] unlockedDungeons = playerStatisticData.GetUnlockedDungeons();
        TeleporterDTO[] unlockedTeleporters = playerStatisticData.GetUnlockedTeleporters();


       PlayerstatisticDTO playerStatistic = new PlayerstatisticDTO(id, unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, userId, username, lastActive, logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex,volumeLevel,knowledge, rewards, visibility, credit, pseudonym);


        return playerStatistic;
    }

    public void SetPseudonym(string newPseudonym)
    {
        this.pseudonym = newPseudonym;
    }

    public void SetCredit(int newCredit)
    {
        this.credit = credit;
    }

    
}
