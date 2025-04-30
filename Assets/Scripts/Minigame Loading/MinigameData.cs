using Unity.VisualScripting;
/// <summary>
///     This class defines all needed data for a <c>Minigame</c>.
/// </summary>
public class MinigameData : IGameEntityData
{
    public MinigameData(string game, string configurationID, MinigameStatus status, int highscore, string description)
    {
        this.game = game;
        this.configurationID = configurationID;
        this.status = status;
        this.highscore = highscore;
        this.description = description;
    }

    public MinigameData()
    {
        game = "";
        configurationID = "";
        status = MinigameStatus.notConfigurated;
        highscore = 0;
    }

    #region Attributes

    private string game;
    private string configurationID;
    private MinigameStatus status;
    private int highscore;
    private string description;

    #endregion

    /// <summary>
    ///     This function converts a MinigameTaskDTO to MinigameData 
    /// </summary>
    /// <param name="dto">The MinigameTaskDTO to convert</param>
    /// <returns>The converted MinigameData</returns>
    public static MinigameData ConvertDtoToData(MinigameTaskDTO dto)
    {
        string game = dto.game;
        string configurationId = dto.configurationId;
        string description = dto.description;
        MinigameStatus status = global::MinigameStatus.notConfigurated;
        int highscore = 0;

        if (configurationId != "" && configurationId != null && configurationId != "NONE")
        {
            status = global::MinigameStatus.active;
        }

        MinigameData data = new MinigameData(game, configurationId, status, highscore, description);
        return data;
    }

    #region GetterAndSetter

    public string GetGame()
    {
        return game;
    }

    public void SetGame(string game)
    {
        this.game = game;
    }

    public string GetConfigurationID()
    {
        return configurationID;
    }

    public void SetConfigurationID(string configurationID)
    {
        this.configurationID = configurationID;
    }

    public MinigameStatus GetStatus()
    {
        return status;
    }

    public void SetStatus(MinigameStatus status)
    {
        this.status = status;
    }

    public int GetHighscore()
    {
        return highscore;
    }

    public void SetHighscore(int highscore)
    {
        this.highscore = highscore;
    }

    public string GetDescription()
    {
        return description;
    }

    #endregion
}