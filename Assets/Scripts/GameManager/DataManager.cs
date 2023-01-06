using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The <c>DataManager</c> stores all required data to set up the objects in the areas.
/// </summary>
public class DataManager : MonoBehaviour
{
    //Singleton
    public static DataManager Instance { get; private set; }

    //Game settigs
    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxBooks;
    private int maxDungeons;

    //Data fields
    private WorldData[] worldData;
    private PlayerstatisticDTO playerData;
    private List<AchievementData> achievementData;
    private Dictionary<Binding, KeyCode> keybindings;

    /// <summary>
    ///     This function sets given data for the specified world
    /// </summary>
    /// <param name="worldIndex">The world to set the data at</param>
    /// <param name="data">The data to set</param>
    public void SetWorldData(int worldIndex, WorldData data)
    {
        if (worldIndex <= 0 || worldIndex > maxWorld)
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
    public void SetWorldData(int worldIndex, WorldDTO data)
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

        if (dungeonIndex <= 0 || dungeonIndex > maxDungeons)
        {
            return null;
        }

        return worldData[worldIndex].getDungeonData(dungeonIndex);
    }

    /// <summary>
    ///     This function returns the player data
    /// </summary>
    /// <returns>The player data</returns>
    public PlayerstatisticDTO GetPlayerData()
    {
        return playerData;
    }

    /// <summary>
    ///     This function checks if a player has unlocked a world.
    /// </summary>
    /// <param name="worldIndex">The index of the world to check</param>
    /// <returns>True, if the player has unlocked the world, false otherwise</returns>
    public bool IsWorldUnlocked(int worldIndex)
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
    public bool IsDungeonUnlocked(int worldIndex, int dungeonIndex)
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

        if (dungeonIndex != 0)
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
    ///     <c>DataManager</c>
    /// </summary>
    /// <param name="playerTaskStatistics">The player minigame statistics data returned from the backend</param>
    public void ProcessMinigameStatisitcs(PlayerTaskStatisticDTO[] playerTaskStatistics)
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
    ///     This function processes the player npc statistcs data returned from the backend and stores the needed data in the
    ///     <c>DataManager</c>
    /// </summary>
    /// <param name="playerNPCStatistics">The player npc statistics data returned from the backend</param>
    public void ProcessNpcStatistics(PlayerNPCStatisticDTO[] playerNPCStatistics)
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

    /// <summary>
    ///     This function processes the player data
    /// </summary>
    /// <param name="playerData">The player statistics returned from the backend</param>
    public void ProcessPlayerStatistics(PlayerstatisticDTO playerStatistics)
    {
        playerData = playerStatistics;
    }

    /// <summary>
    ///     This function processes the achievement statistics data returned from the backend and stores the needed data in the
    ///     <c>DataManager</c>
    /// </summary>
    /// <param name="achievementStatistics">The achievement statistic data returned from the backend</param>
    public void ProcessAchievementStatistics(AchievementStatistic[] achievementStatistics)
    {
        achievementData = new List<AchievementData>();

        if (achievementStatistics == null)
        {
            return;
        }

        foreach (AchievementStatistic statistic in achievementStatistics)
        {
            AchievementData achievement = AchievementData.ConvertFromAchievementStatistic(statistic);
            achievementData.Add(achievement);
        }
    }

    /// <summary>
    ///     This function returns the percentage of completed minigames in the given world
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <returns>The percentage of completed minigames</returns>
    public float GetMinigameProgress(int worldIndex)
    {
        int completedMinigames = 0;
        int minigames = 0;
        for (int minigameIndex = 1; minigameIndex <= GameSettings.GetMaxMinigames(); minigameIndex++)
        {
            WorldData data = worldData[worldIndex];
            if (data.GetEntityDataAt<MinigameData>(minigameIndex) != null)
            {
                if (data.getMinigameStatus(minigameIndex) == global::MinigameStatus.active)
                {
                    minigames++;
                }
                else if (data.getMinigameStatus(minigameIndex) == global::MinigameStatus.done)
                {
                    minigames++;
                    completedMinigames++;
                }
            }
        }

        Debug.Log(completedMinigames + "/" + minigames + " minigames completed");
        if (minigames == 0)
        {
            return 0f;
        }

        return completedMinigames * 1f / (minigames * 1f);
    }

    /// <summary>
    ///     This function returns the percentage of completed minigames in the given dungeon
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    /// <returns>The percentage of completed minigames</returns>
    public float GetMinigameProgress(int worldIndex, int dungeonIndex)
    {
        int completedMinigames = 0;
        int minigames = 0;
        DungeonData dungeonData = worldData[worldIndex].getDungeonData(dungeonIndex);
        if (dungeonData == null)
        {
            return 0f;
        }

        for (int minigameIndex = 1; minigameIndex <= GameSettings.GetMaxMinigames(); minigameIndex++)
        {
            if (dungeonData.GetEntityDataAt<MinigameData>(minigameIndex) != null)
            {
                if (worldData[worldIndex].getMinigameStatus(minigameIndex, dungeonIndex) ==
                    global::MinigameStatus.active)
                {
                    minigames++;
                }
                else if (worldData[worldIndex].getMinigameStatus(minigameIndex, dungeonIndex) ==
                         global::MinigameStatus.done)
                {
                    minigames++;
                    completedMinigames++;
                }
            }
        }

        Debug.Log(completedMinigames + "/" + minigames + " minigames completed");
        if (minigames == 0)
        {
            return 0f;
        }

        return completedMinigames * 1f / (minigames * 1f);
    }


    /// <summary>
    ///     Returns a list of all unlocked teleporters in a world (including its dungeons)
    /// </summary>
    /// <param name="worldIndex"></param>
    /// <returns></returns>
    public List<TeleporterData> GetUnlockedTeleportersInWorld(int worldIndex)
    {
        List<TeleporterData> dataList = new List<TeleporterData>();
        for (int i = 0; i < GameSettings.GetMaxTeleporters(); i++)
        {
            TeleporterData currentData = GetWorldData(worldIndex).GetEntityDataAt<TeleporterData>(i);
            if (currentData != null && currentData.isUnlocked)
            {
                dataList.Add(currentData);
            }
        }

        return dataList;
    }

    /// <summary>
    ///     This function returns all stored achievements
    /// </summary>
    /// <returns>A list containing all achievements</returns>
    public List<AchievementData> GetAchievements()
    {
        Debug.Log("Data Manager, achievements: " + achievementData.Count);
        return achievementData;
    }

    /// <summary>
    ///     This function updates an achievement
    /// </summary>
    /// <param name="title">The title of the achievement</param>
    /// <param name="newProgress">The new progress of the achievement</param>
    /// <returns>True if the acheivement is just now completed, false otherwise</returns>
    public bool UpdateAchievement(AchievementTitle title, int newProgress)
    {
        AchievementData achievement = GetAchievement(title);
        if(achievement != null)
        {
            return achievement.UpdateProgress(newProgress);
        }
        return false;
    }

    /// <summary>
    ///     This function increases an achievements progress by a given increment
    /// </summary>
    /// <param name="title">The title of the achievement</param>
    /// <param name="increment">The amount to increase the progress</param>
    /// <returns>True if the acheivement is just now completed, false otherwise</returns>
    public bool IncreaseAchievementProgress(AchievementTitle title, int increment)
    {
        AchievementData achievement = GetAchievement(title);
        if (achievement != null)
        {
            int newProgress = achievement.GetProgress() + increment;
            Debug.Log("New Progress of '" + title + "': " + achievement.GetProgress());
            return achievement.UpdateProgress(newProgress);
        }
        return false;
    }

    /// <summary>
    ///     This function returns the achievement with the given title
    /// </summary>
    /// <param name="title">The title of the achievement to look for</param>
    /// <returns>The <c>AchievementData</c> corresponding with the given title if present, null otherwise</returns>
    public AchievementData GetAchievement(AchievementTitle title)
    {
        foreach(AchievementData achievement in achievementData)
        {
            if(achievement.GetTitle().Equals(title.ToString()))
            {
                return achievement;
            }
        }
        return null;
    }

    /// <summary>
    ///     This function returns all stored keybindings
    /// </summary>
    /// <returns>A List containing all keybindings</returns>
    public List<Keybinding> GetKeybindings()
    {
        return KeybindingsAsList();
    }

    /// <summary>
    ///     This function changes the keybind of the given <c>Binding</c> to the given <c>KeyCode</c>
    /// </summary>
    /// <param name="keybinding">The binding to change</param>
    public void ChangeKeybind(Keybinding keybinding)
    {
        Binding binding = keybinding.GetBinding();
        keybindings[binding] = keybinding.GetKey();
        GameEvents.current.KeybindingChange(binding);
    }

    /// <summary>
    ///     This function returns the <c>KeyCode</c> for the given <c>Binding</c>
    /// </summary>
    /// <param name="binding">The binding the <c>KeyCode</c> should be returned for</param>
    /// <returns>The <c>KeyCode</c> of the binding if present, KeyCode.NONE otherwise</returns>
    public KeyCode GetKeyCode(Binding binding)
    {
        KeyCode keyCode = KeyCode.None;
        if (keybindings.ContainsKey(binding))
        {
            keyCode = keybindings[binding];
        }

        return keyCode;
    }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetupDataManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        achievementData = GetDummyAchievements();
        keybindings = GetDefaultKeybindings();

        for (int worldIndex = 0; worldIndex <= maxWorld; worldIndex++)
        {
            worldData[worldIndex] = new WorldData();
        }
    }

    /// <summary>
    ///     This function resets the keybindings
    /// </summary>
    public void ResetKeybindings()
    {
        keybindings = GetDefaultKeybindings();        
    }

    private List<AchievementData> GetDummyAchievements()
    {
        List<string> categories1 = new() { "Exploring" };
        AchievementData achievement1 = new AchievementData("GO_FOR_A_WALK", "Walk 10 tiles", categories1, "achievement2", 10, 0, false);
        AchievementData achievement2 = new AchievementData("GO_FOR_A_LONGER_WALK", "Walk 15 tiles", categories1, "achievement2", 15, 0, false);
        AchievementData achievement3 = new AchievementData("TALK_TO_NPCS", "Talkt to 15 NPCs", categories1, "achievement2", 5, 0, false);

        List<AchievementData> achievements = new List<AchievementData>() { achievement1, achievement2, achievement3 };

        return achievements;
    }

    /// <summary>
    ///     This function returns the default keybindings
    /// </summary>
    /// <returns>A dictionary containing the default keybindings</returns>
    private Dictionary<Binding, KeyCode> GetDefaultKeybindings()
    {
        Dictionary<Binding, KeyCode> keybindings = new Dictionary<Binding, KeyCode>();

        keybindings.Add(Binding.MOVE_UP, KeyCode.W);
        keybindings.Add(Binding.MOVE_LEFT, KeyCode.A);
        keybindings.Add(Binding.MOVE_DOWN, KeyCode.S);
        keybindings.Add(Binding.MOVE_RIGHT, KeyCode.D);
        keybindings.Add(Binding.SPRINT, KeyCode.LeftShift);
        keybindings.Add(Binding.INTERACT, KeyCode.E);
        keybindings.Add(Binding.CANCEL, KeyCode.Escape);
        keybindings.Add(Binding.MINIMAP_ZOOM_IN, KeyCode.P);
        keybindings.Add(Binding.MINIMAP_ZOOM_OUT, KeyCode.O);
        keybindings.Add(Binding.GAME_ZOOM_IN, KeyCode.Alpha0);
        keybindings.Add(Binding.GAME_ZOOM_OUT, KeyCode.Alpha9);

        return keybindings;
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
    ///     This function returns all keybindings as a list of <c>Keybinding</c>
    /// </summary>
    /// <returns>A list containing all keybindings</returns>
    private List<Keybinding> KeybindingsAsList()
    {
        List<Keybinding> keybindingsList = new List<Keybinding>();

        foreach (KeyValuePair<Binding, KeyCode> pair in keybindings)
        {
            Keybinding newBinding = new Keybinding(pair.Key, pair.Value);
            keybindingsList.Add(newBinding);
        }

        return keybindingsList;
    }
}