using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    #region Singleton

    public static DataManager Instance { get; private set; }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            SetupDataManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Attributes

    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxBooks;
    private int maxDungeons;

    private WorldData[] worldData;
    private PlayerstatisticDTO playerData;

    #endregion

    /// <summary>
    ///     This function initializes the <c>DataManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    private void SetupDataManager()
    {
        maxWorld = GameSettings.GetMaxWorlds();
        maxMinigames = GameSettings.GetMaxMinigames();
        maxNPCs = GameSettings.GetMaxNpCs();
        maxBooks = GameSettings.GetMaxBooks();
        maxDungeons = GameSettings.GetMaxDungeons();

        worldData = new WorldData[maxWorld + 1];
        playerData = new PlayerstatisticDTO();

        for (int worldIndex = 0; worldIndex <= maxWorld; worldIndex++)
        {
            worldData[worldIndex] = new WorldData();
        }
    }

    /// <summary>
    ///     This function sets given data for the specified world
    /// </summary>
    /// <param name="worldIndex">The world to set the data at</param>
    /// <param name="data">The data to set</param>
    public void SetData(int worldIndex, WorldData data)
    {
        if(worldIndex <= 0 || worldIndex > maxWorld)
        {
            return;
        }
        worldData[worldIndex] = data;
    }

    /// <summary>
    ///     This function converts a WorldDTO into WorldData and sets it for the specified world
    /// </summary>
    /// <param name="worldIndex">The world to set the data at</param>
    /// <param name="data">The dto to convert and set</param>
    public void SetData(int worldIndex, WorldDTO data)
    {
        if (worldIndex <= 0 || worldIndex > maxWorld)
        {
            return;
        }
        WorldData convertedData = WorldData.ConvertDtoToData(data);
        worldData[worldIndex] = convertedData;
    }

    /// <summary>
    ///     This function returns the data of a given world
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <returns>The data of the world, if present, null otherwise</returns>
    public WorldData GetWorldData(int worldIndex)
    {
        if (worldIndex <= 0 || worldIndex > maxWorld)
        {
            return null;
        }
        return worldData[worldIndex];
    }

    /// <summary>
    ///     This function returns the data of a given dungeon
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    /// <returns>The data of the dungeon, if present, null otherwise</returns>
    public DungeonData GetDungeonData(int worldIndex, int dungeonIndex)
    {
        if (worldIndex <= 0 || worldIndex > maxWorld)
        {
            return null;
        }
        if(dungeonIndex <= 0 || dungeonIndex > maxDungeons)
        {
            return null;
        }
        return worldData[worldIndex].getDungeonData(dungeonIndex);
    }

    /// <summary>
    ///     This function checks if a player has unlocked a world.
    /// </summary>
    /// <param name="worldIndex">The index of the world to check</param>
    /// <returns>True, if the player has unlocked the world, false otherwise</returns>
    public bool PlayerHasWorldUnlocked(int worldIndex)
    {
        for (int i = 0; i < playerData.unlockedAreas.Length; i++)
        {
            if (playerData.unlockedAreas[i].worldIndex == worldIndex &&
                playerData.unlockedAreas[i].dungeonIndex == 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This function checks if a player has unlocked a dungeon.
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon to check</param>
    /// <returns>True, if the player has unlocked the dungeon, false otherwise</returns>
    public bool PlayerHasDungeonUnlocked(int worldIndex, int dungeonIndex)
    {
        for (int i = 0; i < playerData.unlockedAreas.Length; i++)
        {
            if (playerData.unlockedAreas[i].worldIndex == worldIndex &&
                playerData.unlockedAreas[i].dungeonIndex == dungeonIndex)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This function marks an NPC as completed
    /// </summary>
    /// <param name="worldIndex">The index of the world the NPC is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the NPC is in (0 if in world)</param>
    /// <param name="number">The number of the NPC in its area</param>
    public void CompleteNPC(int worldIndex, int dungeonIndex, int number)
    {
        if (worldIndex <= 0 || worldIndex > maxWorld)
        {
            return;
        }
        if(dungeonIndex != 0)
        {
            if (dungeonIndex <= 0 || dungeonIndex > maxDungeons)
            {
                return;
            }
            worldData[worldIndex].npcCompleted(dungeonIndex, number);
        }
        else
        {
            worldData[worldIndex].npcCompleted(number);
        }
    }

    /// <summary>
    ///     This function processes the player minigame statistics data returned form backend and stores the needed data in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="playerTaskStatistics">The player minigame statistics data returned from the backend</param>
    public void ProcessPlayerTaskStatisitcs(PlayerTaskStatisticDTO[] playerTaskStatistics)
    {
        if (playerTaskStatistics == null)
        {
            return;
        }

        foreach (PlayerTaskStatisticDTO statistic in playerTaskStatistics)
        {
            int worldIndex = statistic.minigameTask.area.worldIndex;

            if (worldIndex < 0 || worldIndex >= worldData.Length)
            {
                break;
            }

            int dungeonIndex = statistic.minigameTask.area.dungeonIndex;
            int index = statistic.minigameTask.index;
            int highscore = statistic.highscore;
            bool completed = statistic.completed;
            MinigameStatus status = global::MinigameStatus.notConfigurated;
            if (MinigameStatus(worldIndex, dungeonIndex, index) != global::MinigameStatus.notConfigurated)
            {
                if (completed)
                {
                    status = global::MinigameStatus.done;
                }
                else
                {
                    status = global::MinigameStatus.active;
                }
            }

            worldData[worldIndex].setMinigameStatus(dungeonIndex, index, status);
            worldData[worldIndex].setMinigameHighscore(dungeonIndex, index, highscore);
        }
    }

    /// <summary>
    ///     This function returns the status of a minigame in a world or dungeon.
    /// </summary>
    /// <param name="worldIndex">The index of the world the minigame is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the minigame is in (0 if in world)</param>
    /// <param name="index">The index of the minigame in its area</param>
    /// <returns>The status of the minigame, <c>notConfigurated if not found</c></returns>
    private MinigameStatus MinigameStatus(int worldIndex, int dungeonIndex, int index)
    {
        if (worldIndex < 0 || worldIndex >= worldData.Length)
        {
            return global::MinigameStatus.notConfigurated;
        }

        if (dungeonIndex != 0)
        {
            return worldData[worldIndex].getMinigameStatus(dungeonIndex, index);
        }

        return worldData[worldIndex].getMinigameStatus(index);
    }

    /// <summary>
    ///     This function processes the player npc statistcs data returned from the backend and stores the needed data in the
    ///     <c>GameManager</c>
    /// </summary>
    /// <param name="playerNPCStatistics">The player npc statistics data returned from the backend</param>
    public void ProcessPlayerNpcStatistics(PlayerNPCStatisticDTO[] playerNPCStatistics)
    {
        if (playerNPCStatistics == null)
        {
            return;
        }

        foreach (PlayerNPCStatisticDTO statistic in playerNPCStatistics)
        {
            int worldIndex = statistic.npc.area.worldIndex;
            int dungeonIndex = statistic.npc.area.dungeonIndex;
            int index = statistic.npc.index;
            bool completed = statistic.completed;

            if (worldIndex < worldData.Length)
            {
                worldData[worldIndex].setNPCStatus(dungeonIndex, index, completed);
            }
        }
    }
}
