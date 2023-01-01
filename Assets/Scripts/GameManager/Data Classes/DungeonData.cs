using System.Collections.Generic;
using System;

/// <summary>
///     This class defines all needed data for a <c>Dungeon</c>.
/// </summary>
public class DungeonData : IAreaData
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

    #endregion

    #region Constructors

    public DungeonData(string id, int index, string staticName, string topicName, bool active, MinigameData[] minigames,
        NPCData[] npcs, BookData[] books)
    {
        this.id = id;
        this.index = index;
        this.staticName = staticName;
        this.topicName = topicName;
        this.active = active;
        this.minigames = minigames;
        this.npcs = npcs;
        this.books = books;
    }

    public DungeonData()
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
    }

    #endregion

    /// <summary>
    ///     This function converts a DungeonDTO to DungeonData 
    /// </summary>
    /// <param name="dto">The DungeonDTO to convert</param>
    /// <returns>The converted DungeonData</returns>
    public static DungeonData ConvertDtoToData(DungeonDTO dto)
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

        DungeonData data = new DungeonData(id, index, staticName, topicName, active, minigames, npcs, books);
        return data;
    }

    #region GetterAndSetter

    /// <summary>
    ///     This function sets the status of a minigame in the dungeon.
    /// </summary>
    /// <param name="index">The index of the minigame in its area</param>
    /// <param name="status">The status to be set</param>
    public void SetMinigameStatus(int index, MinigameStatus status)
    {
        if (index < minigames.Length)
        {
            minigames[index].SetStatus(status);
        }
    }

    /// <summary>
    ///     This function sets the highscore of a minigame in the dungeon.
    /// </summary>
    /// <param name="index">The index of the minigame in its area</param>
    /// <param name="highscore">The highscore to be set</param>
    public void SetMinigameHighscore(int index, int highscore)
    {
        if (index < minigames.Length)
        {
            minigames[index].SetHighscore(highscore);
        }
    }

    /// <summary>
    ///     This function sets the status of a NPC in the dungeon.
    /// </summary>
    /// <param name="index">The index of the NPC in its area</param>
    /// <param name="completed">The status to be set</param>
    public void SetNpcStatus(int index, bool completed)
    {
        if (index < npcs.Length)
        {
            npcs[index].SetHasBeenTalkedTo(completed);
        }
    }

    /// <summary>
    ///     This function returns the status of a minigame in the dungeon.
    /// </summary>
    /// <param name="index">This index of the minigame</param>
    /// <returns>The status of the minigame, <c>notConfigurated</c> if invalid index</returns>
    public MinigameStatus GetMinigameStatus(int index)
    {
        if (index < minigames.Length)
        {
            return minigames[index].GetStatus();
        }

        return MinigameStatus.notConfigurated;
    }

    /// <summary>
    ///     This function returns the data of a minigame in the dungeon.
    /// </summary>
    /// <param name="index">This index of the minigame</param>
    /// <returns>The data of the minigame, <c>null</c> if invalid index</returns>
    public MinigameData GetMinigameData(int index)
    {
        if (index > 0 && index < minigames.Length)
        {
            return minigames[index];
        }

        return null;
    }

    /// <summary>
    ///     This function returns the data of a NPC in the world.
    /// </summary>
    /// <param name="index">This index of the NPC</param>
    /// <returns>The data of the NPC, <c>null</c> if invalid index</returns>
    public NPCData GetNpcData(int index)
    {
        if (index > 0 && index < npcs.Length)
        {
            return npcs[index];
        }

        return null;
    }

    /// <summary>
    ///     This function returns the data of a Book in the world.
    /// </summary>
    /// <param name="index">This index of the Book</param>
    /// <returns>The data of the Book, <c>null</c> if invalid index</returns>
    public BookData GetBookData(int index)
    {
        if (index > 0 && index < books.Length)
        {
            return books[index];
        }

        return null;
    }

    /// <summary>
    ///     This function returns whether the dungeon is set as active or not.
    /// </summary>
    /// <returns>The active status of the dungeon</returns>
    public bool IsActive()
    {
        return active;
    }

    /// <summary>
    ///     This function sets a NPC of the dungeon as completed.
    /// </summary>
    /// <param name="number">The index of NPC</param>
    public void NpcCompleted(int number)
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

    public IGameEntityData GetEntityDataAt<T>(int index) where T : IGameEntity
    {
        IGameEntityData entityData;
        if (typeof(T) == typeof(Book))
        {
            entityData = books[index];
        }
        else if (typeof(T) == typeof(NPC))
        {
            entityData = npcs[index];
        }
        else if (typeof(T) == typeof(Minigame))
        {
            entityData = minigames[index];
        }
        else
        {
            throw new ArgumentOutOfRangeException("There exists no Data Array for " + typeof(T).FullName);
        }
        return entityData;
    }

    /// <summary>
    /// Calls the constructor of the given GameEntityData class on the given array position 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    void IAreaData.InitializeEmptyDataAt<T>(int index)
    {
        if (typeof(T) == typeof(Book))
        {
            books[index] = new BookData();
        }
        else if (typeof(T) == typeof(NPC))
        {
            npcs[index] = new NPCData();
        }
        else if (typeof(T) == typeof(Minigame))
        {
            minigames[index] = new MinigameData();
        }
        else
        {
            throw new ArgumentOutOfRangeException("Could not initilize data. There exists no Data Array for " + typeof(T).FullName);
        }
    }

    #endregion
}