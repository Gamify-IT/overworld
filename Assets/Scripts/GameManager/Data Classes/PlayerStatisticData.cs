using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to store all information about the player and the session.
/// </summary>
public class PlayerStatisticData 
{
    #region attributes
    private readonly string id;
    public AreaLocationDTO[] unlockedAreas;
    public AreaLocationDTO[] unlockedDungeons;
    public TeleporterDTO[] unlockedTeleporters;
    private AreaLocationDTO currentArea;
    private readonly string userId;
    private string username;
    private string lastActive;
    private float logoutPositionX;
    private float logoutPositionY;
    private string logoutScene;
    private int currentCharacterIndex;
    private int knowledge;
    private int volumeLevel;
    private int rewards;
    private bool visibility;
    private int credit;
    private string pseudonym;
    private string leagueOfPlayer;
    private bool updatedCredit = false;
    private bool updatedPseudonym= false;
    private bool updatedVisibility = false;
    #endregion

    #region Constructor
    /// <summary>
    ///     Constructor for the PlayerStatisticData class, initializing all attributes.
    /// </summary>
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
    #endregion

    /// <summary>
    ///     This function converts a <c>PlayerStatisticDTO</c> object into a <c>PlayerStatisticData</c> instance
    /// </summary>
    /// <param name="playerStatisticDTO">The <c>PlayerStatisticDTO</c> object to convert</param>
    /// <returns>the <c>PlayerStatisticData</c> instance</returns>
    public static PlayerStatisticData ConvertFromPlayerStatisticDTO(PlayerStatisticDTO statistic)
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



    /// <summary>
    ///     Updates the player's credit.
    /// </summary>
    /// <param name="price">The amount to reduce the credit by.</param>
    /// <returns>True if the credit remains positive, otherwise false.</returns>
    public bool updateCredit(int price)
    {
        updatedCredit = true;
        credit = this.credit - price;
        if(credit > 0)
        {
            return true;
        }
    }

    /// <summary>
    ///     Updates the player's pseudonym.
    /// </summary>
    /// <param name="name">The new pseudonym.</param>
    /// <returns>True if the pseudonym was updated, otherwise false.</returns>
    public bool updatePseudonym(string name)
    {
        updatedPseudonym = true;
       
        if (name != null)
        {
            pseudonym = name;
            return true;
        }
    }

    /// <summary>
    ///     Updates the player's visibility.
    /// </summary>
    /// <param name="visibility">The new visibility state.</param>
    /// <returns>Always true.</returns>
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
    }

    /// <summary>
    ///     Calculates the player's league based on rewards.
    /// </summary>
    /// <param name="rewards">The amount of rewards.</param>
    /// <returns>The corresponding league.</returns>
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


    /// <summary>
    ///     Checks if the credit has been updated.
    /// </summary>
    /// <returns>True if the credit was updated, otherwise false.</returns>
    public bool creditIsUpdated()
    {
        return updatedCredit;
    }

    /// <summary>
    ///     Checks if the pseudonym has been updated.
    /// </summary>
    /// <returns>True if the pseudonym was updated, otherwise false.</returns>
    public bool PseudonymIsUpdated()
    {
        return updatedPseudonym;
    }

    /// <summary>
    ///     Checks if the visibility has been updated.
    /// </summary>
    /// <returns>True if the visibility was updated, otherwise false.</returns>
    public bool VisibilityIsUpdated()
    {
        return updatedVisibility;
    }


    #region Getter and Setter


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

    /// <summary>
    ///     Gets the current state of players visibility in the leaderboard
    /// </summary>
    /// <returns>visibility state</returns>
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

    /// <summary>
    ///     Gets the players pseudonym 
    /// </summary>
    /// <returns>pseudonym of player</returns>
    public string GetPseudonym()
    {
        return pseudonym;
    }

    /// <summary>
    ///     Sets the name of the players pseudonym
    /// </summary>
    /// <param name="newPseudonym">pseudonym</param>
    public void SetPseudonym(string newPseudonym)
    {
        this.pseudonym = newPseudonym;
    }

    public int GetCredit()
    {
        return credit;
    }

    /// <summary>
    ///     Sets the credit of the player after something was bought
    /// </summary>
    /// <param name="newCredit">new credit</param>
    public void SetCredit(int newCredit)
    {
        this.credit = credit;
    }


    public int GetVolumeLevel()
    {
        return volumeLevel;
    }

    #endregion

   

    
   
    
}
