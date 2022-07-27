using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonDTO
{
    public string id;
    public int index;
    public string staticName;
    public string topicName;
    public bool active;
    public List<MinigameTaskDTO> minigameTasks;
    public List<NPCDTO> npcs;

    public DungeonDTO(string id, int index, string staticName, string topicName, bool active, List<MinigameTaskDTO> minigameTasks, List<NPCDTO> npcs)
    {
        this.id = id;
        this.index = index;
        this.staticName = staticName;
        this.topicName = topicName;
        this.active = active;
        this.minigameTasks = minigameTasks;
        this.npcs = npcs;
    }

    public DungeonDTO() { }

    public static DungeonDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DungeonDTO>(jsonString);
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

    public string getStaticName()
    {
        return staticName;
    }

    public void setStaticName(string staticName)
    {
        this.staticName = staticName;
    }

    public string getTopicName()
    {
        return topicName;
    }

    public void setTopicName(string topicName)
    {
        this.topicName = topicName;
    }

    public bool getActive()
    {
        return active;
    }

    public void setActive(bool active)
    {
        this.active = active;
    }

    public List<MinigameTaskDTO> getMinigameTasks()
    {
        return minigameTasks;
    }

    public void setMinigameTasks(List<MinigameTaskDTO> minigameTasks)
    {
        this.minigameTasks = minigameTasks;
    }

    public List<NPCDTO> getNPCs()
    {
        return npcs;
    }

    public void setNPCs(List<NPCDTO> npcs)
    {
        this.npcs = npcs;
    }
    #endregion
}
