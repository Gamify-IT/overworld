using System;

/// <summary>
///     This class defines all needed data for a player statistic. 
/// </summary>
public class PlayerstatisticData
{
    #region Attributes 
    private string id;
    private AreaLocationDTO[] unlockedAreas;
    private AreaLocationDTO[] unlockedDungeons;
    private TeleporterDTO[] unlockedTeleporters;
    private AreaLocationDTO currentArea;
    private readonly string userId;
    private string username;
    private float logoutPositionX;
    private float logoutPositionY;
    private string logoutScene;
    private int logoutWorldIndex;
    private int logoutDungeonIndex;
    private int knowledge;
    #endregion

    #region Constructor
    public PlayerstatisticData(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, TeleporterDTO[] unlockedTeleporters,
        AreaLocationDTO currentArea, string userId, string username, float logoutPositionX, float logoutPositionY, string logoutScene,
        int logoutWorldIndex, int logoutDungeonIndex, int knowledge)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = unlockedDungeons;
        this.unlockedTeleporters = unlockedTeleporters;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        this.logoutPositionX = logoutPositionX;
        this.logoutPositionY = logoutPositionY;
        this.logoutScene = logoutScene;
        this.logoutWorldIndex = logoutWorldIndex;
        this.logoutDungeonIndex = logoutDungeonIndex;
        this.knowledge = knowledge;
    }
    #endregion

    public static PlayerstatisticData ConvertDtoToData(PlayerStatisticDTO dto)
    {
        PlayerstatisticData data = new PlayerstatisticData(dto.id, dto.unlockedAreas, dto.unlockedDungeons, dto.unlockedTeleporters, dto.currentArea,
            dto.userId, dto.username, dto.logoutPositionX, dto.logoutPositionY, dto.logoutScene, dto.logoutWorldIndex,
            dto.logoutDungeonIndex, dto.knowledge);

        return data;
    }

    #region GetterAndSetter
    public AreaLocationDTO[] GetUnlockedAreas()
    {
        return unlockedAreas;
    }

    public float GetLogoutPositionX()
    {
        return logoutPositionX;
    }

    public void SetLogoutPositionX(float xPos)
    {
        logoutPositionX = xPos;
    }

    public float GetLogoutPositionY()
    {
        return logoutPositionY;
    }

    public void SetLogoutPositionY(float yPos)
    {
        logoutPositionY = yPos;
    }

    public string GetLogoutScene()
    {
        return logoutScene;
    }

    public void SetLogoutScene(string sceneName)
    {
        logoutScene = sceneName;
    }

    public int GetLogoutWorldIndex()
    {
        return logoutWorldIndex;
    }

    public void SetLogoutWorldIndex(int worldIndex)
    {
        logoutWorldIndex = worldIndex;
    }

    public int GetLogoutDungeonIndex()
    {
        return logoutDungeonIndex;
    }

    public void SetLogoutDungeonIndex(int dungeonIndex)
    {
        logoutDungeonIndex = dungeonIndex;
    }
    #endregion
}