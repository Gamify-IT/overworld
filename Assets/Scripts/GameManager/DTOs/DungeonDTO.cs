using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to retrieve data from Get Requests.
/// </summary>
[System.Serializable]
public class DungeonDTO
{
    #region Attributes
    public string id;
    public int index;
    public string staticName;
    public string topicName;
    public bool active;
    public List<MinigameTaskDTO> minigameTasks;
    public List<NPCDTO> npcs;
    #endregion

    #region Constructors
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
    #endregion

    /// <summary>
    /// This function converts a json string to a <c>DungeonDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>DungeonDTO</c> object containing the data</returns>
    public static DungeonDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DungeonDTO>(jsonString);
    }
}
