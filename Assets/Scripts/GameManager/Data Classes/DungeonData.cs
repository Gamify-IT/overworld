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
        minigames = new MinigameData[1];
        npcs = new NPCData[1];
    }
    #endregion

    #region GetterAndSetter
    public MinigameStatus getMinigameStatus(int index)
    {
        if (index < minigames.Length)
        {
            return minigames[index].getStatus();
        }
        return MinigameStatus.notConfigurated;
    }
    #endregion
}
