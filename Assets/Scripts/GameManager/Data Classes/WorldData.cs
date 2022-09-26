/// <summary>
///     This class defines all needed data for a <c>World</c>.
/// </summary>
public class WorldData
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

    #endregion

    #region Constructors

    public WorldData(string id, int index, string staticName, string topicName, bool active, MinigameData[] minigames,
        NPCData[] npcs, DungeonData[] dungeons, BookData[] books)
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
    }

    #endregion

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
    ///     This function returns the data of a minigame in the world.
    /// </summary>
    /// <param name="index">This index of the minigame</param>
    /// <returns>The data of the minigame, <c>null</c> if invalid index</returns>
    public MinigameData getMinigameData(int index)
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
    public NPCData getNPCData(int index)
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
    public BookData getBookData(int index)
    {
        if (index > 0 && index < books.Length)
        {
            return books[index];
        }

        return null;
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

    #endregion
}