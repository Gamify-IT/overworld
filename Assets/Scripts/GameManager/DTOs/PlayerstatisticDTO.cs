using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerStatisticDTO
{
    #region Attributes
    public string id;
    public AreaLocationDTO[] unlockedAreas;
    public AreaLocationDTO[] unlockedDungeons;
    public TeleporterDTO[] unlockedTeleporters;
    public AreaLocationDTO currentArea;
    public string userId;
    public string username;
    public string lastActive;
    public float logoutPositionX;
    public float logoutPositionY;
    public string logoutScene;
    public int currentCharacterIndex;
    public int volumeLevel;
    public int knowledge;
    public int rewards;
    public bool visibility;
    public int credit;
    public string pseudonym;
    public string currentCharacter;
    public string currentAccessory;
    #endregion

    #region Constructors

    public PlayerStatisticDTO(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, TeleporterDTO[] unlockedTeleporters,
         AreaLocationDTO currentArea, string userId, string username, string lastActive, float logoutPositionX, float logoutPositionY,
         string logoutScene, int currentCharacterIndex, int volumeLevel, int knowledge, int rewards, bool visibility, int credit, string pseudonym, string currentCharacter, string currentAccessory)
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
        this.visibility = visibility;
        this.credit = credit;
        this.pseudonym = pseudonym;
        this.currentCharacter = currentCharacter;
        this.currentAccessory = currentAccessory;
    }

    public PlayerStatisticDTO()
    {
        id = "";
        AreaLocationDTO unlockedWorld = new AreaLocationDTO();
        unlockedWorld.worldIndex = 1;
        unlockedWorld.dungeonIndex = 0;
        unlockedAreas = new AreaLocationDTO[1];
        unlockedAreas[0] = unlockedWorld;
        currentArea = unlockedWorld;
        unlockedDungeons = new AreaLocationDTO[0];
        unlockedTeleporters = new TeleporterDTO[0];
        userId = "";
        username = "";
        lastActive = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        logoutPositionX = 21.5f;
        logoutPositionY = 2.5f;
        logoutScene = "World 1";
        currentCharacterIndex = 0;
        volumeLevel = 1;
        knowledge = 0;
        rewards = 0;
        visibility = false;
        credit = 0;
        pseudonym = "";
        currentCharacter = "character_default";
        currentAccessory = "none";
    }
    #endregion

    /// <summary>
    ///     This function converts a <c>PlayerStatisticData</c> object into a <c>PlayerStatisticDTO</c> instance
    /// </summary>
    /// <param name="PlayerStatisticData">The <c>PlayerStatisticData</c> object to convert</param>
    /// <returns>the <c>PlayerStatisticDTO</c> instance</returns>
    public static PlayerStatisticDTO ConvertDataToDTO(PlayerStatisticData playerStatisticData)
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
        string currentCharacter = playerStatisticData.GetCurrentCharacter();
        string currentAccessory = playerStatisticData.GetCurrentAccessory();

        AreaLocationDTO currentArea = playerStatisticData.GetCurrentArea();
        AreaLocationDTO[] unlockedAreas = playerStatisticData.GetUnlockedAreas();
        AreaLocationDTO[] unlockedDungeons = playerStatisticData.GetUnlockedDungeons();
        TeleporterDTO[] unlockedTeleporters = playerStatisticData.GetUnlockedTeleporters();

        PlayerStatisticDTO playerStatistic = new PlayerStatisticDTO(id, unlockedAreas, unlockedDungeons, unlockedTeleporters, currentArea, userId, username, lastActive, 
            logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex, volumeLevel, knowledge, rewards, visibility, credit, pseudonym, currentCharacter, currentAccessory);

        return playerStatistic;
    }

    /// <summary>
    ///     This function converts a json string to a <c>PlayerstatisticDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>PlayerstatisticDTO</c> object containing the data</returns>
    public static PlayerStatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerStatisticDTO>(jsonString);
    }
}