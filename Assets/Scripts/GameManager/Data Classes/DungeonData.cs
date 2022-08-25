using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    #region Attributes
    private string id;
    private int index;
    private string staticName;
    private string topicName;
    private bool active;
    private MinigameData[] minigames;
    private NPCData[] npcs;
    #endregion

    #region Constructors
    public DungeonData(string id, int index, string staticName, string topicName, bool active, MinigameData[] minigames, NPCData[] npcs)
    {
        this.id = id;
        this.index = index;
        this.staticName = staticName;
        this.topicName = topicName;
        this.active = active;
        this.minigames = minigames;
        this.npcs = npcs;
    }

    public DungeonData()
    {
        id = "";
        index = 0;
        staticName = "";
        topicName = "";
        active = false;
        minigames = new MinigameData[GameSettings.getMaxMinigames()+1];
        for (int minigameIndex = 1; minigameIndex < minigames.Length; minigameIndex++)
        {
            minigames[minigameIndex] = new MinigameData();
        }
        npcs = new NPCData[GameSettings.getMaxNPCs()+1];
        for (int npcIndex = 1; npcIndex < npcs.Length; npcIndex++)
        {
            npcs[npcIndex] = new NPCData();
        }
    }
    #endregion

    #region GetterAndSetter
    public void setMinigameStatus(int index, MinigameStatus status)
    {
        if (index < minigames.Length)
        {
            minigames[index].setStatus(status);
        }
    }

    public void setMinigameHighscore(int index, int highscore)
    {
        if (index < minigames.Length)
        {
            minigames[index].setHighscore(highscore);
        }
    }

    public void setNPCStatus(int index, bool completed)
    {
        if (index < npcs.Length)
        {
            npcs[index].setHasBeenTalkedTo(completed);
        }
    }

    public MinigameStatus getMinigameStatus(int index)
    {
        if (index < minigames.Length)
        {
            return minigames[index].getStatus();
        }
        return MinigameStatus.notConfigurated;
    }

    public MinigameData getMinigameData(int index)
    {
        if (index > 0 && index < minigames.Length)
        {
            return minigames[index];
        }
        else
        {
            return null;
        }
    }

    public NPCData getNPCData(int index)
    {
        if (index > 0 && index < npcs.Length)
        {
            return npcs[index];
        }
        else
        {
            return null;
        }
    }
    #endregion
}
