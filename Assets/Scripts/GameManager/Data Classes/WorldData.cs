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
        minigames = new MinigameData[1];
        npcs = new NPCData[1];
        dungeons = new DungeonData[1];
    }
    #endregion

    #region GetterAndSetter
    public void setMinigameStatus(int index, MinigameStatus status)
    {
        if(index < minigames.Length)
        {
            minigames[index].setStatus(status);
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
    #endregion
}
