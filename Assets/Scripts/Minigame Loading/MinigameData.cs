using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This scripts defines the minigame data that needs to be transfered from the game manager to a minigame object.
 */
public class MinigameData
{
    #region Attributes
    private string game;
    private string configurationID;
    private MinigameStatus status;
    #endregion

    public MinigameData(string game, string configurationID, MinigameStatus status)
    {
        this.game = game;
        this.configurationID = configurationID;
        this.status = status;
    }

    #region GetterAndSetter
    public string getGame()
    {
        return game;
    }

    public void setGame(string game)
    {
        this.game = game;
    }

    public string getConfigurationID()
    {
        return configurationID;
    }

    public void setConfigurationID(string configurationID)
    {
        this.configurationID = configurationID;
    }

    public MinigameStatus getStatus()
    {
        return status;
    }

    public void setStatus(MinigameStatus status)
    {
        this.status = status;
    }
    #endregion
}
