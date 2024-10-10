using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerStatisticDTO
{
    #region Attributes
    private readonly string id;
    private readonly string userId;
    private readonly string username;
    private AreaLocationDTO[] unlockedAreas;
    private AreaLocationDTO[] unlockedDungeons;
    private TeleporterDTO[] unlockedTeleporters;
    private AreaLocationDTO currentArea;
    private string lastActive;
    private float logoutPositionX;
    private float logoutPositionY;
    private string logoutScene;
    private int currentCharacterIndex;
    private int volumeLevel;
    private int knowledge;
    private int rewards;
    private bool visibility;
    private int credit;
    private string pseudonym;
    private string currentCharacter;
    private string currentAccessory;
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

    #region Getter and Setter
    public string GetId()
    {
        return this.id;
    }

    public string GetUserId()
    {
        return this.userId;
    }

    public string GetUsername()
    {
        return this.username;
    }

    public AreaLocationDTO[] GetUnlockedAreas()
    {
        return this.unlockedAreas;
    }

    public void SetUnlockedAreas(AreaLocationDTO[] value)
    {
        this.unlockedAreas = value;
    }

    public AreaLocationDTO[] GetUnlockedDungeons()
    {
        return this.unlockedDungeons;
    }

    public void SetUnlockedDungeons(AreaLocationDTO[] value)
    {
        this.unlockedDungeons = value;
    }

    public TeleporterDTO[] GetUnlockedTeleporters()
    {
        return this.unlockedTeleporters;
    }

    public void SetUnlockedTeleporters(TeleporterDTO[] value)
    {
        this.unlockedTeleporters = value;
    }

    public AreaLocationDTO GetCurrentArea()
    {
        return this.currentArea;
    }

    public void SetCurrentArea(AreaLocationDTO value)
    {
        this.currentArea = value;
    }

    public string GetLastActive()
    {
        return this.lastActive;
    }

    public void SetLastActive(string value)
    {
        this.lastActive = value;
    }

    public float GetLogoutPositionX()
    {
        return this.logoutPositionX;
    }

    public void SetLogoutPositionX(float value)
    {
        this.logoutPositionX = value;
    }

    public float GetLogoutPositionY()
    {
        return this.logoutPositionY;
    }

    public void SetLogoutPositionY(float value)
    {
        this.logoutPositionY = value;
    }

    public string GetLogoutScene()
    {
        return this.logoutScene;
    }

    public void SetLogoutScene(string value)
    {
        this.logoutScene = value;
    }

    public int GetCurrentCharacterIndex()
    {
        return this.currentCharacterIndex;
    }

    public void SetCurrentCharacterIndex(int value)
    {
        this.currentCharacterIndex = value;
    }

    public int GetKnowledge()
    {
        return this.knowledge;
    }

    public void SetKnowledge(int value)
    {
        this.knowledge = value;
    }

    public int GetVolumeLevel()
    {
        return this.volumeLevel;
    }

    public void SetVolumeLevel(int value)
    {
        this.volumeLevel = value;
    }

    public int GetRewards()
    {
        return this.rewards;
    }

    public void SetRewards(int value)
    {
        this.rewards = value;
    }

    public bool GetVisibility()
    {
        return this.visibility;
    }

    public void SetVisibility(bool value)
    {
        this.visibility = value;
    }

    public int GetCredit()
    {
        return this.credit;
    }

    public void SetCredit(int newCredit)
    {
        this.credit = newCredit;
    }

    public string GetPseudonym()
    {
        return this.pseudonym;
    }

    public void SetPseudonym(string value)
    {
        this.pseudonym = value;
    }

    public string GetCurrentCharacter()
    {
        return this.currentCharacter;
    }

    public void SetCurrentCharacter(string character)
    {
        this.currentCharacter = character;
    }

    public string GetCurrentAccessory()
    {
        return this.currentAccessory;
    }

    public void SetCurrentAccessory(string accessory)
    {
        this.currentAccessory = accessory;
    }

    #endregion
}