using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class WorldDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>WorldDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>WorldDTO</c> object containing the data</returns>
    public static WorldDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<WorldDTO>(jsonString);
    }

    #region Attributes

    public string id;
    public int index;
    public string staticName;
    public string topicName;
    public bool active;
    public List<MinigameTaskDTO> minigameTasks;
    public List<NPCDTO> npcs;
    public List<BookDTO> books;
    public List<DungeonDTO> dungeons;

    #endregion

    #region Constructors

    public WorldDTO(string id, int index, string staticName, string topicName, bool active,
        List<MinigameTaskDTO> minigameTasks, List<NPCDTO> npcs, List<DungeonDTO> dungeons, List<BookDTO> books)
    {
        this.id = id;
        this.index = index;
        this.staticName = staticName;
        this.topicName = topicName;
        this.active = active;
        this.minigameTasks = minigameTasks;
        this.npcs = npcs;
        this.dungeons = dungeons;
        this.books = books;
    }

    public WorldDTO()
    {
    }

    #endregion
}