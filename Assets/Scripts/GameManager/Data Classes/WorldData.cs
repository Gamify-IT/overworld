using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData
{
    #region Attributes
    private string id;
    private int index;
    private string staticName;
    private string topicName;
    private bool active;
    private MinigameData[] minigames;
    private NPCData[] npcs;
    private DungeonData[] dungeons;
    #endregion

    #region Constructors
    public WorldData(string id, int index, string staticName, string topicName, bool active, MinigameData[] minigames, NPCData[] npcs, DungeonData[] dungeons)
    {
        this.id = id;
        this.index = index;
        this.staticName = staticName;
        this.topicName = topicName;
        this.active = active;
        this.minigames = minigames;
        this.npcs = npcs;
        this.dungeons = dungeons;
    }

    public WorldData()
    {
        id = "";
        index = 0;
        staticName = "";
        topicName = "";
        active = false;
        minigames = new MinigameData[GameSettings.getMaxMinigames()+1];
        for(int minigameIndex = 1; minigameIndex < minigames.Length; minigameIndex++)
        {
            minigames[minigameIndex] = new MinigameData();
        }
        npcs = new NPCData[GameSettings.getMaxNPCs()+1];
        for(int npcIndex = 1; npcIndex < npcs.Length; npcIndex++)
        {
            npcs[npcIndex] = new NPCData();
        }
        dungeons = new DungeonData[GameSettings.getMaxDungeons()+1];
        for(int dungeonIndex = 1; dungeonIndex < dungeons.Length; dungeonIndex++)
        {
            dungeons[dungeonIndex] = new DungeonData();
        }
    }
    #endregion

    #region GetterAndSetter
    public void setMinigameStatus(int dungeonIndex, int index, MinigameStatus status)
    {
        if(dungeonIndex != 0)
        {
            if(dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].setMinigameStatus(index, status);
            }
        }
        else if(index < minigames.Length)
        {
            minigames[index].setStatus(status);
        }
    }

    public void setMinigameHighscore(int dungeonIndex, int index, int highscore)
    {
        if (dungeonIndex != 0)
        {
            if (dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].setMinigameHighscore(index, highscore);
            }
        }
        else if (index < minigames.Length)
        {
            minigames[index].setHighscore(highscore);
        }
    }

    public void setNPCStatus(int dungeonIndex, int index, bool completed)
    {
        if(dungeonIndex != 0)
        {
            if(dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].setNPCStatus(index, completed);
            }
        }
        else if(index < npcs.Length)
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

    public MinigameStatus getMinigameStatus(int dungeonIndex, int index)
    {
        if(dungeonIndex < dungeons.Length)
        {
            return dungeons[dungeonIndex].getMinigameStatus(index);
        }
        return MinigameStatus.notConfigurated;
    }

    public MinigameData getMinigameData(int index)
    {
        if(index > 0 && index < minigames.Length)
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

    public DungeonData getDungeonData(int index)
    {
        if (index > 0 && index < dungeons.Length)
        {
            return dungeons[index];
        }
        else
        {
            return null;
        }
    }
    
    public bool isActive()
    {
        return active;
    }

    public bool dungeonIsActive(int dungeonIndex)
    {
        return dungeons[dungeonIndex].isActive();
    }

    public void npcCompleted(int dungeonIndex, int number)
    {
        if(dungeonIndex < dungeons.Length)
        {
            dungeons[dungeonIndex].npcCompleted(number);
        }
    }

    public void npcCompleted(int number)
    {
        if(number < npcs.Length)
        {
            npcs[number].setHasBeenTalkedTo(true);
        }
    }
    #endregion
}
