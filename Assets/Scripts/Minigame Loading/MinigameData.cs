/// <summary>
///     This class defines all needed data for a <c>Minigame</c>.
/// </summary>
public class MinigameData
{
    public MinigameData(string game, string configurationID, MinigameStatus status, int highscore)
    {
        this.game = game;
        this.configurationID = configurationID;
        this.status = status;
        this.highscore = highscore;
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
        MinigameStatus status = global::MinigameStatus.notConfigurated;
        int highscore = 0;

        if (configurationId != "" && configurationId != null && configurationId != "NONE")
        {
            status = global::MinigameStatus.active;
        }

        MinigameData data = new MinigameData(game, configurationId, status, highscore);
        return data;
    }

    #region GetterAndSetter

    /// <summary>
    ///     This function returns the name of the minigame.
    /// </summary>
    /// <returns>game</returns>
    public string GetGame()
    {
        return game;
    }

    /// <summary>
    ///     This function sets the name of the minigame.
    /// </summary>
    /// <param name="game">The name for the minigame</param>
    public void SetGame(string game)
    {
        this.game = game;
    }

    /// <summary>
    ///     This function returns the configuration ID of the minigame.
    /// </summary>
    /// <returns>configurationID</returns>
    public string GetConfigurationID()
    {
        return configurationID;
    }

    /// <summary>
    ///     This function sets the configuration ID of the minigame.
    /// </summary>
    /// <param name="configurationID">The configurationID for the minigame</param>
    public void SetConfigurationID(string configurationID)
    {
        this.configurationID = configurationID;
    }

    /// <summary>
    ///     This function returns the status of the minigame.
    /// </summary>
    /// <returns>status</returns>
    public MinigameStatus GetStatus()
    {
        return status;
    }

    /// <summary>
    ///     This function sets the status of the minigame.
    /// </summary>
    /// <param name="status">Status for the minigame</param>
    public void SetStatus(MinigameStatus status)
    {
        this.status = status;
    }

    /// <summary>
    ///     This function returns the highscore of the minigame.
    /// </summary>
    /// <returns>highscore</returns>
    public int GetHighscore()
    {
        return highscore;
    }

    /// <summary>
    ///     This function sets the highscore of the minigame by configurationId.
    /// </summary>
    /// <param name="highscore">The highscore of the minigame by configurationId</param>
    public void SetHighscore(int highscore)
    {
        this.highscore = highscore;
    }

    #endregion
}