using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinigameTaskDTO
{
    private string id;
    private int index;
    private string game;
    private string configurationId;

    public MinigameTaskDTO(string id, int index, string game, string configurationId)
    {
        this.id = id;
        this.index = index;
        this.game = game;
        this.configurationId = configurationId;
    }

    #region GetterAndSetter
    public string getId()
    {
        return id;
    }

    public void setId(string id)
    {
        this.id = id;
    }

    public int getIndex()
    {
        return index;
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    public string getGame()
    {
        return game;
    }

    public void setGame(string game)
    {
        this.game = game;
    }

    public string getConfigurationId()
    {
        return configurationId;
    }

    public void setConfigurationId(string configurationId)
    {
        this.configurationId = configurationId;
    }
    #endregion
}
