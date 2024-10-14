using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to store all information about the player and the session.
/// </summary>
public class PlayerStatisticData
{
    #region Attributes
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
    private bool updatedPseudonym = false;
    private bool updatedVisibility = false;
    private bool updatedCharacterIndex = false;
    private bool updatedAccessoryIndex = false;
    private string currentCharacter;
    private string currentAccessory;
    #endregion

    #region Constructor
    /// <summary>
    ///     Constructor for the PlayerStatisticData class, initializing all attributes.
    /// </summary>
    public PlayerStatisticData(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] completedDungeons, TeleporterDTO[] unlockedTeleporters, AreaLocationDTO currentArea, string userId, string username, string lastActive, float logoutPositionX, float logoutPositionY,
         string logoutScene, int currentCharacterIndex, int volumeLevel, int knowledge, int rewards, bool visibility, int credit, string pseudonym, string currentCharacter, string currentAccessory)
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
        this.leagueOfPlayer = CalculateLeagueOfPlayer(rewards);
        this.visibility = visibility;
        this.credit = credit;
        this.pseudonym = pseudonym;
        this.currentCharacter = currentCharacter;
        this.currentAccessory = currentAccessory;
    }
    #endregion

    /// <summary>
    ///     This function converts a <c>PlayerStatisticDTO</c> object into a <c>PlayerStatisticData</c> instance
    /// </summary>
    /// <param name="playerStatisticDTO">The <c>PlayerStatisticDTO</c> object to convert</param>
    /// <returns>the <c>PlayerStatisticData</c> instance</returns>
    public static PlayerStatisticData ConvertDtoToData(PlayerStatisticDTO statistic)
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
        string currentCharacter = statistic.currentCharacter;
        string currentAccessory = statistic.currentAccessory;

        AreaLocationDTO[] unlockedAreas = new AreaLocationDTO[statistic.unlockedAreas.Length];
        AreaLocationDTO[] unlockedDungeons = new AreaLocationDTO[statistic.unlockedDungeons.Length];
        TeleporterDTO[] unlockedTeleporters = new TeleporterDTO[statistic.unlockedTeleporters.Length];

        for (int i = 0; i < statistic.unlockedAreas.Length; i++)
        {
            unlockedAreas[i] = statistic.unlockedAreas[i];
        }

        for (int i = 0; i < statistic.unlockedDungeons.Length; i++)
        {
            unlockedDungeons[i] = statistic.unlockedDungeons[i];
        }

        for (int i = 0; i < statistic.unlockedTeleporters.Length; i++)
        {
            unlockedTeleporters[i] = statistic.unlockedTeleporters[i];
        }

        PlayerStatisticData data = new PlayerStatisticData(id, unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, userId, username, lastActive,
            logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex, volumeLevel, knowledge, rewards, visibility, credit, pseudonym, currentCharacter, currentAccessory);
        return data;
    }

    /// <summary>
    ///     Updates the player's character.
    /// </summary>
    /// <param name="currentCharacterIndex"></param>
    /// <returns>True if the credit remains positive, otherwise false.</returns>
    public bool UpdateCharacter(string currentCharacterIndex)
    {
        updatedCharacterIndex = true;
        currentCharacter = currentCharacterIndex;
        return true;
    }

    /// <summary>
    ///     Updates the player's accessory.
    /// </summary>
    /// <param name="currentAccessoryIndex"></param>
    /// <returns>True if the credit remains positive, otherwise false.</returns>
    public bool UpdateAccessory(string currentAccessoryIndex)
    {
        updatedAccessoryIndex = true;
        currentAccessory = currentAccessoryIndex;
        return true;
    }

    /// <summary>
    ///     Updates the player's credit.
    /// </summary>
    /// <param name="price">The amount to reduce the credit by.</param>
    /// <returns>True if the credit remains positive, otherwise false.</returns>
    public bool UpdateCredit(int price)
    {
        updatedCredit = true;
        credit = this.credit - price;
        return true;
    }

    /// <summary>
    ///     Updates the player's pseudonym.
    /// </summary>
    /// <param name="name">The new pseudonym.</param>
    /// <returns>True if the pseudonym was updated, otherwise false.</returns>
    public bool UpdatePseudonym(string name)
    {
        updatedPseudonym = true;
        pseudonym = name;
        return true;

    }

    /// <summary>
    ///     Updates the player's visibility.
    /// </summary>
    /// <param name="visibility">The new visibility state.</param>
    /// <returns>Always true.</returns>
    public bool UpdateVisibility(bool visibility)
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

    /// <summary>
    ///     Calculates the player's league based on rewards.
    /// </summary>
    /// <param name="rewards">The amount of rewards.</param>
    /// <returns>The corresponding league.</returns>
    public string CalculateLeagueOfPlayer(int rewards)
    {
        if (rewards < 450)
        {
            return "Wanderer";
        }
        else if (rewards < 900)
        {
            return "Explorer";
        }
        else if (rewards < 1350)
        {
            return "Pathfinder";
        }
        else
        {
            return "Trailblazer";
        }
    }


    /// <summary>
    ///     Checks if the credit has been updated.
    /// </summary>
    /// <returns>True if the credit was updated, otherwise false.</returns>
    public bool CreditIsUpdated()
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

    public bool CharacterIsUpdated()
    {
        return updatedCharacterIndex;
    }

    public bool AccessoryIsUpdated()
    {
        return updatedAccessoryIndex;
    }

    #region Getter and Setter
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

    /// <summary>
    ///     Updates the current area of the player
    /// </summary>
    /// <param name="area">current area</param>
    public void SetCurrentArea(AreaLocationDTO area)
    {
        currentArea = area;
    }

    public string GetUserId()
    {
        return userId;
    }

    public string GetUsername()
    {
        return username;
    }

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

    public int GetKnowledge()
    {
        return knowledge;
    }

    /// <summary>
    ///     Gets the current volume level
    /// </summary>
    /// <returns>volume level</returns>
    public int GetVolumeLevel()
    {
        return volumeLevel;
    }

    /// <summary>
    ///     Updates the current volume level chosen by the player
    /// </summary>
    /// <param name="volumeLevel">current volume level</param>
    public void SetVolumeLevel(int volumeLevel)
    {
        this.volumeLevel = volumeLevel;
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
        int index = currentArea.worldIndex;

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

    public string GetCurrentCharacter()
    {
        return currentCharacter;
    }

    public void SetCurrentCharacter(string newCharacter)
    {
        this.currentCharacter = newCharacter;
    }

    public string GetCurrentAccessory()
    {
        return currentAccessory;
    }

    public void SetCurrentAccessory(string newAccessory)
    {
        this.currentAccessory = newAccessory;
    }
    #endregion
}