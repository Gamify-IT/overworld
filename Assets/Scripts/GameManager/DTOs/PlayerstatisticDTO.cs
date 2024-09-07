using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerStatisticDTO
{
    #region attributes
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
    public int knowledge;
    public int volumeLevel;
    public int rewards;
    public bool showRewards;
    public string pseudonym;
    #endregion

    #region constructors 
    public PlayerStatisticDTO(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, TeleporterDTO[] unlockedTeleporters,
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
        showRewards = true;
        pseudonym = "";
    }
    #endregion

    /// <summary>
    ///     This function converts a <c>PlayerStatisticData</c> object into a <c>PlayerStatisticDTO</c> instance
    /// </summary>
    /// <param name="PlayerStatisticData">The <c>PlayerStatisticData</c> object to convert</param>
    /// <returns>the <c>PlayerStatisticDTO</c> instance</returns>
    public static PlayerStatisticDTO ConvertDataToDTO(PlayerStatisticData data)
    {
        string id = data.GetId();
        AreaLocationDTO[] unlockedAreas = data.GetUnlockedAreas();
        AreaLocationDTO[] unlockedDungeons = data.GetUnlockedDungeons();
        TeleporterDTO[] unlockedTeleporters = data.GetUnlockedTeleporters();
        AreaLocationDTO currentArea = data.GetCurrentArea();
        string userId = data.GetUserId();
        string username = data.GetUsername();
        string lastActive = data.GetLastActive();
        float logoutPositionX = data.GetLogoutPositionX();
        float logoutPositionY = data.GetLogoutPositionY();
        string logoutScene = data.GetLogoutScene();
        int currentCharacterIndex = data.GetCurrentCharacterIndex();
        int knowledge = data.GetKnowledge();
        int volumeLevel = data.GetVolumeLevel();
        int rewards = data.GetRewards();
        bool showRewards = data.GetShowRewards();
        string pseudonym = data.GetPseudonym();

        PlayerStatisticDTO playerStatistic = new PlayerStatisticDTO(id, unlockedAreas, unlockedDungeons, unlockedTeleporters, 
            currentArea, userId, username, lastActive, logoutPositionX, logoutPositionY, logoutScene, currentCharacterIndex, 
            volumeLevel, knowledge, rewards, showRewards, pseudonym);

        return playerStatistic;
    }

    /// <summary>
    ///     This function converts a json string to a <c>PlayerStatisticDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>PlayerStatisticDTO</c> object containing the data</returns>
    public static PlayerStatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerStatisticDTO>(jsonString);
    }
}