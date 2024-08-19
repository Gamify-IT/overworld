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
    private AreaLocationDTO[] unlockedAreas;
    private AreaLocationDTO[] unlockedDungeons;
    private TeleporterDTO[] unlockedTeleporters;
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
    private bool showRewards;
    private string pseudonym;
    private string leagueOfPlayer;
    #endregion

    #region constructur
    public PlayerStatisticData(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, TeleporterDTO[] unlockedTeleporters,
         AreaLocationDTO currentArea, string userId, string username, string lastActive, float logoutPositionX, float logoutPositionY,
         string logoutScene, int currentCharacterIndex, int volumeLevel, int knowledge, int rewards, bool showRewards, string pseudonym)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = unlockedDungeons;
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
        this.showRewards = showRewards;
        this.pseudonym = pseudonym;
        leagueOfPlayer = CalculateLeagueOfPlayer(rewards);
    }
    #endregion

    /// <summary>
    ///     This function converts a <c>PlayerStatisticDTO</c> object into a <c>PlayerStatisticData</c> instance
    /// </summary>
    /// <param name="playerStatisticDTO">The <c>PlayerStatisticDTO</c> object to convert</param>
    /// <returns>the <c>PlayerStatisticData</c> instance</returns>
    public static PlayerStatisticData ConvertDtoToData(PlayerStatisticDTO dto)
    {        
        string id = dto.id;
        AreaLocationDTO[] unlockedAreas = new AreaLocationDTO[dto.unlockedAreas.Length];
        AreaLocationDTO[] unlockedDungeons = new AreaLocationDTO[dto.unlockedDungeons.Length];
        TeleporterDTO[] unlockedTeleporters = new TeleporterDTO[dto.unlockedTeleporters.Length];
        AreaLocationDTO currentArea = dto.currentArea;
        string userId = dto.userId;
        string username = dto.username;
        string lastActive = dto.lastActive;
        float logoutPositionX = dto.logoutPositionX;
        float logoutPositionY = dto.logoutPositionY;
        string logoutScene = dto.logoutScene;
        int currentCharacterIndex = dto.currentCharacterIndex;
        int volumeLevel = dto.volumeLevel;
        int knowledge = dto.knowledge;
        int rewards = dto.rewards;
        bool showRewards = dto.showRewards;
        string pseudonym = dto.pseudonym;

        for (int i = 0; i < dto.unlockedAreas.Length; i++)
        {
            unlockedAreas[i] = dto.unlockedAreas[i];
        }

        for (int i = 0; i < dto.unlockedDungeons.Length; i++)
        {
            unlockedDungeons[i] = dto.unlockedDungeons[i];
        }

        for (int i = 0; i < dto.unlockedTeleporters.Length; i++)
        {
            unlockedTeleporters[i] = dto.unlockedTeleporters[i];
        }


        PlayerStatisticData data = new PlayerStatisticData(id, unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, userId, username, lastActive, 
            logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex, volumeLevel, knowledge, rewards, showRewards, pseudonym);

        return data;
    }

    public string CalculateLeagueOfPlayer(int rewards)
    {
        if(rewards < 100)
        {
            return "Wanderer";
        }
        else if (rewards < 200)
        {
            return "Explorer";
        }
        else if (rewards < 300)
        {
            return "Pathfinder";
        }
        else
        {
            return "Trailblazer";
        }
        return "-";
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

    public string GetPseudonym()
    {
        return pseudonym;
    }

    public bool GetVisibility()
    {
        return showRewards;
    }

    public void SetPseudonym(string newPseudonym)
    {
        this.pseudonym = newPseudonym;
    }

    public void SetVisibility(bool update)
    {
        showRewards = update;
    }
#endregion

}