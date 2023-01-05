using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class defines all needed data for a <c>World</c>.
/// </summary>
public class WorldData : IAreaData
{
    #region Attributes

    private string id;
    private int index;
    private string staticName;
    private string topicName;
    private readonly bool active;
    private readonly MinigameData[] minigames;
    private readonly NPCData[] npcs;
    private readonly BookData[] books;
    private readonly DungeonData[] dungeons;
    private readonly TeleporterData[] teleporters;

    #endregion

    #region Constructors

    public WorldData(string id, int index, string staticName, string topicName, bool active, MinigameData[] minigames,
        NPCData[] npcs, DungeonData[] dungeons, BookData[] books, TeleporterData[] teleporters)
    {
        this.id = id;
        this.index = index;
        this.staticName = staticName;
        this.topicName = topicName;
        this.active = active;
        this.minigames = minigames;
        this.npcs = npcs;
        this.books = books;
        this.dungeons = dungeons;
        this.teleporters = teleporters;
    }

    public WorldData()
    {
        id = "";
        index = 0;
        staticName = "";
        topicName = "";
        active = false;
        minigames = new MinigameData[GameSettings.GetMaxMinigames() + 1];
        for (int minigameIndex = 1; minigameIndex < minigames.Length; minigameIndex++)
        {
            minigames[minigameIndex] = new MinigameData();
        }

        npcs = new NPCData[GameSettings.GetMaxNpCs() + 1];
        for (int npcIndex = 1; npcIndex < npcs.Length; npcIndex++)
        {
            npcs[npcIndex] = new NPCData();
        }

        books = new BookData[GameSettings.GetMaxBooks() + 1];
        for (int bookIndex = 1; bookIndex < books.Length; bookIndex++)
        {
            books[bookIndex] = new BookData();
        }

        dungeons = new DungeonData[GameSettings.GetMaxDungeons() + 1];
        for (int dungeonIndex = 1; dungeonIndex < dungeons.Length; dungeonIndex++)
        {
            dungeons[dungeonIndex] = new DungeonData();
        }

        teleporters = new TeleporterData[GameSettings.GetMaxTeleporters() + 1];
        for (int tpIndex = 1; tpIndex < teleporters.Length; tpIndex++)
        {
            teleporters[tpIndex] = new TeleporterData();
        }
    }

    #endregion

    /// <summary>
    ///     This function converts a WorldDTO to WorldData 
    /// </summary>
    /// <param name="dto">The WorldDTO to convert</param>
    /// <returns>The converted WorldData</returns>
    public static WorldData ConvertDtoToData(WorldDTO dto)
    {
        string id = dto.id;
        int index = dto.index;
        string staticName = dto.staticName;
        string topicName = dto.topicName;
        bool active = dto.active;

        MinigameData[] minigames = new MinigameData[GameSettings.GetMaxMinigames() + 1];
        List<MinigameTaskDTO> minigameDTOs = dto.minigameTasks;
        foreach (MinigameTaskDTO minigameDTO in minigameDTOs)
        {
            MinigameData minigameData = MinigameData.ConvertDtoToData(minigameDTO);
            minigames[minigameDTO.index] = minigameData;
        }

        NPCData[] npcs = new NPCData[GameSettings.GetMaxNpCs() + 1];
        List<NPCDTO> npcDTOs = dto.npcs;
        foreach (NPCDTO npcDTO in npcDTOs)
        {
            NPCData npcData = NPCData.ConvertDtoToData(npcDTO);
            npcs[npcDTO.index] = npcData;
        }

        BookData[] books = new BookData[GameSettings.GetMaxBooks() + 1];
        List<BookDTO> bookDTOs = dto.books;
        foreach (BookDTO bookDTO in bookDTOs)
        {
            BookData bookData = BookData.ConvertDtoToData(bookDTO);
            books[bookDTO.index] = bookData;
        }

        DungeonData[] dungeons = new DungeonData[GameSettings.GetMaxDungeons() + 1];
        List<DungeonDTO> dungeonDTOs = dto.dungeons;
        foreach (DungeonDTO dungeonDTO in dungeonDTOs)
        {
            DungeonData dungeonData = DungeonData.ConvertDtoToData(dungeonDTO);
            dungeons[dungeonDTO.index] = dungeonData;
        }

        
        // just for demonstration
        TeleporterData[] teleporters = new TeleporterData[GameSettings.GetMaxTeleporters() + 1];
        

        WorldData data = new WorldData(id, index, staticName, topicName, active, minigames, npcs, dungeons, books, teleporters);
        return data;
    }

    #region GetterAndSetter

    /// <summary>
    ///     This function sets the status of a minigame in the world or in a dungeon of the world.
    /// </summary>
    /// <param name="dungeonIndex">The dungeons index of the minigame (0 if world)</param>
    /// <param name="index">The index of the minigame in its area</param>
    /// <param name="status">The status to be set</param>
    public void setMinigameStatus(int dungeonIndex, int index, MinigameStatus status)
    {
        if (dungeonIndex != 0)
        {
            if (dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].SetMinigameStatus(index, status);
            }
        }
        else if (index < minigames.Length)
        {
            minigames[index].SetStatus(status);
        }
    }

    /// <summary>
    ///     This function sets the highscore of a minigame in the world or in a dungeon of the world.
    /// </summary>
    /// <param name="dungeonIndex">The dungeons index of the minigame (0 if world)</param>
    /// <param name="index">The index of the minigame in its area</param>
    /// <param name="highscore">The highscore to be set</param>
    public void setMinigameHighscore(int dungeonIndex, int index, int highscore)
    {
        if (dungeonIndex != 0)
        {
            if (dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].SetMinigameHighscore(index, highscore);
            }
        }
        else if (index < minigames.Length)
        {
            minigames[index].SetHighscore(highscore);
        }
    }

    /// <summary>
    ///     This function sets the status of a NPC in the world or in a dungeon of the world.
    /// </summary>
    /// <param name="dungeonIndex">The dungeons index of the minigame (0 if world)</param>
    /// <param name="index">The index of the NPC in its area</param>
    /// <param name="completed">The status to be set</param>
    public void setNPCStatus(int dungeonIndex, int index, bool completed)
    {
        if (dungeonIndex != 0)
        {
            if (dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].SetNpcStatus(index, completed);
            }
        }
        else if (index < npcs.Length)
        {
            npcs[index].SetHasBeenTalkedTo(completed);
        }
    }

    public void UnlockTeleporter(int dungeonIndex, int index)
    {
        if (dungeonIndex != 0)
        {
            if (dungeonIndex < dungeons.Length)
            {
                dungeons[dungeonIndex].UnlockTeleporter(index);
            }
        }
        else if (index < npcs.Length)
        {
            teleporters[index].isUnlocked = true;
        }
    }

    /// <summary>
    ///     This function returns the status of a minigame in the world.
    /// </summary>
    /// <param name="index">This index of the minigame</param>
    /// <returns>The status of the minigame, <c>notConfigurated</c> if invalid index</returns>
    public MinigameStatus getMinigameStatus(int index)
    {
        if (index < minigames.Length)
        {
            return minigames[index].GetStatus();
        }

        return MinigameStatus.notConfigurated;
    }

    /// <summary>
    ///     This function returns the status of a minigame in a dungeon of the world.
    /// </summary>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    /// <param name="index">This index of the minigame</param>
    /// <returns>The status of the minigame, <c>notConfigurated</c> if invalid index</returns>
    public MinigameStatus getMinigameStatus(int dungeonIndex, int index)
    {
        if (dungeonIndex < dungeons.Length)
        {
            return dungeons[dungeonIndex].GetMinigameStatus(index);
        }

        return MinigameStatus.notConfigurated;
    }

    /// <summary>
    ///     This function returns the data of a dungeon of the world.
    /// </summary>
    /// <param name="index">This index of the dungeon</param>
    /// <returns>The data of the dungeon, <c>null</c> if invalid index</returns>
    public DungeonData getDungeonData(int index)
    {
        if (index > 0 && index < dungeons.Length)
        {
            return dungeons[index];
        }

        return null;
    }

    /// <summary>
    /// This function returns the data array of the game entity type specified by the generic parameter T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    public T GetEntityDataAt<T>(int index) where T : IGameEntityData
    {
        IGameEntityData entityData;
        if (typeof(T) == typeof(BookData) && IsIndexInArrayRange(index, books))
        {
            entityData = books[index];
        }
        else if (typeof(T) == typeof(NPCData) && IsIndexInArrayRange(index, npcs))
        {
            entityData = npcs[index];
        }
        else if (typeof(T) == typeof(MinigameData) && IsIndexInArrayRange(index, minigames))
        {
            entityData = minigames[index];
        }
        else if (typeof(T) == typeof(TeleporterData) && IsIndexInArrayRange(index, teleporters))
        {
            entityData = teleporters[index];
        }
        else
        {
            throw new ArgumentOutOfRangeException("There exists no Data Array for " + typeof(T).FullName);
        }
        return (T)entityData;
    }

    private bool IsIndexInArrayRange(int index, IGameEntityData[] data)
    {
        return index > 0 && index < data.Length;
    }

    /// <summary>
    ///     This function returns whether the world is set as active or not.
    /// </summary>
    /// <returns>The active status of the world</returns>
    public bool isActive()
    {
        return active;
    }

    /// <summary>
    ///     This function returns whether a dungeon of the world is set as active or not.
    /// </summary>
    /// <param name="dungeonIndex">This index of the dungeon</param>
    /// <returns>The active status of the dungeon</returns>
    public bool dungeonIsActive(int dungeonIndex)
    {
        return dungeons[dungeonIndex].IsActive();
    }

    /// <summary>
    ///     This function sets a NPC of a dungeon of the world as completed.
    /// </summary>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    /// <param name="number">The index of the NPC</param>
    public void npcCompleted(int dungeonIndex, int number)
    {
        if (dungeonIndex < dungeons.Length)
        {
            dungeons[dungeonIndex].NpcCompleted(number);
        }
    }

    /// <summary>
    ///     This function sets a NPC of the world as completed.
    /// </summary>
    /// <param name="number">The index of NPC</param>
    public void npcCompleted(int number)
    {
        if (number < npcs.Length)
        {
            npcs[number].SetHasBeenTalkedTo(true);
        }
    }

    /// <summary>
    /// This function the data of all minigames in the world.
    /// </summary>
    /// <returns>The data of the minigames</returns>
    public MinigameData[] GetMinigameData()
    {
        return minigames;
    }

    public void SetTeleporterData(TeleporterConfig config)
    {
        if (config.dungeonID != 0)
        {
            getDungeonData(config.dungeonID).SetTeleporterData(config);
        }
        else
        {
            TeleporterData data = teleporters[config.index];
            bool isUnlocked = false;
            if (data != null)
            {
                isUnlocked = data.isUnlocked;
            }
            teleporters[config.index] = new TeleporterData(config.name, config.worldID, config.dungeonID, config.index, new Vector2(config.x, config.y), isUnlocked);
        }
    }

    public TeleporterData GetTeleporterData(int dungeonID, int index)
    {
        if (dungeonID != 0)
        {
            return getDungeonData(dungeonID).GetEntityDataAt<TeleporterData>(index);
        }
        return teleporters[index];
    }

    #endregion
}