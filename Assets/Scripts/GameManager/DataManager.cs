using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

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
    private AreaDataManager areaDataManager;
    private WorldData[] worldData;
    private PlayerstatisticDTO playerData;
    private List<AchievementData> achievementData;
    private Dictionary<Binding, KeyCode> keybindings;
    private Dictionary<String, int> wanderer;
    private Dictionary<String, int> explorer;
    private Dictionary<String, int> pathfinder;
    private Dictionary<String, int> trailblazer;




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
    ///     This function unlocks a teleporter
    /// </summary>
    /// <param name="worldIndex">The index of the world the NPC is in</param>
    /// <param name="dungeonIndex">The index of the dungeon the NPC is in (0 if in world)</param>
    /// <param name="number">The number of the NPC in its area</param>
    public void ActivateTeleporter(int worldIndex, int dungeonIndex, int number)
    {
        worldData[worldIndex].UnlockTeleporter(dungeonIndex, number);
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


    public void ProcessallPlayerStatistics(PlayerstatisticDTO[] playerStatisticDTOs)
    {
        
            foreach (PlayerstatisticDTO playerDTO in playerStatisticDTOs)
            {
                if (playerDTO.rewards >= 0 && playerDTO.rewards <= 100)
                {
                    wanderer.Add(playerDTO.username, playerDTO.rewards);
                }
            }
        
    }

    public List<String, int> wandererList()
    {
        return wanderer;
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
        foreach (TeleporterDTO teleporterDTO in playerData.unlockedTeleporters)
        {
            int worldIndex = teleporterDTO.area.worldIndex;
            int dungeonIndex = teleporterDTO.area.dungeonIndex;
            int number = teleporterDTO.index;
            GetWorldData(worldIndex).UnlockTeleporter(dungeonIndex, number);
        }
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
            Debug.Log("achievements list is null");
            return;
        }
        Debug.Log("Process " + achievementStatistics.Length + " achievements");

        foreach (AchievementStatistic statistic in achievementStatistics)
        {
            AchievementData achievement = AchievementData.ConvertFromAchievementStatistic(statistic);
            Debug.Log("Processed achievement: " + achievement.GetTitle());
            achievementData.Add(achievement);
        }
    }

    /// <summary>
    ///     This function checks for a given array of <c>KeycodeDTO</c>s whether they are valid or not.
    ///     If so, they are set as the bindings, otherwise the default bindings are set.
    /// </summary>
    /// <param name="keybindingDTOs"></param>
    public void ProcessKeybindings(KeybindingDTO[] keybindingDTOs)
    {
        List<Keybinding> keybindings = ConvertKeybindings(keybindingDTOs);
        if (ValidKeybindings(keybindings))
        {
            Debug.Log("Keybindings valid");
            SetKeybindings(keybindings);
        }
        else
        {
            Debug.Log("Keybindings invalid");
            GameManager.Instance.ResetKeybindings();
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
        WorldData worldData = GetWorldData(worldIndex);
        for (int i = 1; i < GameSettings.GetMaxTeleporters() + 1; i++)
        {
            TeleporterData currentData = worldData.GetEntityDataAt<TeleporterData>(i);
            if (currentData != null && currentData.isUnlocked)
            {
                dataList.Add(currentData);
            }
        }

        for (int i = 1; i < GameSettings.GetMaxDungeons() + 1; i++)
        {
            DungeonData dungeonData = worldData.getDungeonData(i);

            if (dungeonData == null)
            {
                continue;
            }

            for (int j = 1; j < GameSettings.GetMaxTeleporters() + 1; j++)
            {
                TeleporterData currentData = dungeonData.GetEntityDataAt<TeleporterData>(j);
                if (currentData != null && currentData.isUnlocked)
                {
                    dataList.Add(currentData);
                }
            }
        }

        Debug.Log("UnlockedTPs in World " + worldIndex + ": " + dataList.Count);
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
        if (achievement != null)
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
            //Debug.Log("New Progress of '" + title + "': " + achievement.GetProgress());
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
        foreach (AchievementData achievement in achievementData)
        {
            if (achievement.GetTitle().Equals(title.ToString()))
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
    /// <returns>True if the key was changed, false otherwise</returns>
    public bool ChangeKeybind(Keybinding keybinding)
    {
        Binding binding = keybinding.GetBinding();
        KeyCode keyCode = keybinding.GetKey();

        bool keyChanged = false;

        if(keybindings[binding] != keyCode)
        {
            keybindings[binding] = keyCode;
            GameEvents.current.KeybindingChange(binding);
            keyChanged = true;
            Debug.Log("Changed binding " + binding + " to: " + keybinding.GetKey().ToString());
        }
        return keyChanged;
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
    ///     This function triggers the loading of the area data
    /// </summary>
    /// <returns>False, if loading was successful, true otherwise</returns>
    public async UniTask<bool> FetchAreaData()
    {
        bool loadingError = await areaDataManager.FetchData();
        return loadingError;
    }

    /// <summary>
    ///     This function gets the local stored area data as dummy data
    /// </summary>
    public void GetDummyAreaData()
    {
        areaDataManager.GetDummyData();
    }

    /// <summary>
    ///     This function returns the area data for the requested area
    /// </summary>
    /// <param name="areaInformation">The area identifier</param>
    /// <returns>An optional containing the <c>AreaData</c>, if present, an empty optional otherwise</returns>
    public Optional<AreaData> GetAreaData(AreaInformation areaInformation)
    {
        return areaDataManager.GetAreaData(areaInformation);
    }

    public void AddTeleporterInformation(AreaInformation areaIdentifier, int number, TeleporterData data)
    {
        //get teleporter status
        int worldIndex = data.worldID;
        int dungeonIndex = data.dungeonID;

        foreach(TeleporterDTO unlockedTeleporter in playerData.unlockedTeleporters)
        {
            if(unlockedTeleporter.area.worldIndex == worldIndex &&
                unlockedTeleporter.area.dungeonIndex == dungeonIndex &&
                unlockedTeleporter.index == number)
            {
                data.isUnlocked = true;
            }
        }

        Debug.Log("Update teleporter " + worldIndex + "-" + dungeonIndex + "-" + number + ": unlocked: " + data.isUnlocked);

        //save information
        if(areaIdentifier.IsDungeon())
        {
            worldData[worldIndex].SetTeleporterData(dungeonIndex, number, data);
        }
        else
        {
            worldData[worldIndex].SetTeleporterData(number, data);
        }
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
        maxNPCs = GameSettings.GetMaxNpcs();
        maxBooks = GameSettings.GetMaxBooks();
        maxDungeons = GameSettings.GetMaxDungeons();

        areaDataManager = new AreaDataManager();

        worldData = new WorldData[maxWorld + 1];
        playerData = new PlayerstatisticDTO();
        InitKeybindingsDictionary();

        for (int worldIndex = 0; worldIndex <= maxWorld; worldIndex++)
        {
            worldData[worldIndex] = new WorldData();
        }
    }

    /// <summary>
    ///     This function initializes the keybindings dictionary
    /// </summary>
    private void InitKeybindingsDictionary()
    {
        Dictionary<Binding, KeyCode> keybindings = new Dictionary<Binding, KeyCode>();

        keybindings.Add(Binding.MOVE_UP, KeyCode.None);
        keybindings.Add(Binding.MOVE_LEFT, KeyCode.None);
        keybindings.Add(Binding.MOVE_DOWN, KeyCode.None);
        keybindings.Add(Binding.MOVE_RIGHT, KeyCode.None);
        keybindings.Add(Binding.SPRINT, KeyCode.None);
        keybindings.Add(Binding.INTERACT, KeyCode.None);
        keybindings.Add(Binding.CANCEL, KeyCode.None);
        keybindings.Add(Binding.MINIMAP_ZOOM_IN, KeyCode.None);
        keybindings.Add(Binding.MINIMAP_ZOOM_OUT, KeyCode.None);
        keybindings.Add(Binding.GAME_ZOOM_IN, KeyCode.None);
        keybindings.Add(Binding.GAME_ZOOM_OUT, KeyCode.None);

        this.keybindings = keybindings;
    }

    /// <summary>
    ///     This function converts an array of <c>KeybindingDTO</c>s to a list of <c>Keybinding</c>s
    /// </summary>
    /// <param name="keybindingDTOs">The array of <c>KeybindingDTO</c>s to convert</param>
    /// <returns></returns>
    private List<Keybinding> ConvertKeybindings(KeybindingDTO[] keybindingDTOs)
    {
        List<Keybinding> keybindings = new List<Keybinding>();
        foreach(KeybindingDTO keybindingDTO in keybindingDTOs)
        {
            try
            {
                Keybinding keybinding = Keybinding.ConvertDTO(keybindingDTO);
                keybindings.Add(keybinding);
            }
            catch (ArgumentException)
            {
                Debug.Log("Could not convert the binding: " + keybindingDTO.binding + " -> " + keybindingDTO.key);
            }
        }
        return keybindings;
    }

    /// <summary>
    ///     This function checks whether the given keybindings are valid or not.
    ///     That means each binding has a unique key, so no keys can be bound to multiple binding, 
    ///     nor can a binding occour multiple times or not at all. 
    /// </summary>
    /// <param name="keybindings"></param>
    /// <returns></returns>
    private bool ValidKeybindings(List<Keybinding> keybindings)
    {
        bool validBindings = true;

        List<KeyCode> keyCodes = new List<KeyCode>();
        List<Binding> bindings = new List<Binding>();
        Dictionary<Binding, bool> bindingContained = new Dictionary<Binding, bool>();
        foreach(Binding bindingValue in Enum.GetValues(typeof(Binding)))
        {
            bindingContained.Add(bindingValue, false);
        }

        foreach (Keybinding keybinding in keybindings)
        {
            KeyCode keyCode = keybinding.GetKey();
            Binding binding = keybinding.GetBinding();
            
            if(keyCodes.Contains(keyCode))
            {
                Debug.Log("Multiple uses of keyCode: " + keyCode);
                validBindings = false;
                break;
            }
            keyCodes.Add(keyCode);

            if(bindings.Contains(binding))
            {
                Debug.Log("Multiple bindings for: " + binding);
                validBindings = false;
                break;
            }
            bindings.Add(binding);

            bindingContained[binding] = true;
        }

        if(validBindings)
        {
            foreach (Binding bindingValue in Enum.GetValues(typeof(Binding)))
            {
                if (!bindingContained[bindingValue])
                {
                    Debug.Log("No binding for: " + bindingValue);
                    validBindings = false;
                }
            }
        }        

        return validBindings;
    }

    /// <summary>
    ///     This function sets the given keybindings as the active ones
    /// </summary>
    /// <param name="keybindings"></param>
    private void SetKeybindings(List<Keybinding> keybindings)
    {
        foreach(Keybinding keybinding in keybindings)
        {
            ChangeKeybind(keybinding);
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
    ///     Reads the teleporter config and assigns its data to the TeleporterData array of its belonging IAreaData
    /// </summary>
    public void ReadTeleporterConfig()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TeleporterConfig/TeleporterConfigJson");
        var manager = JsonUtility.FromJson<TeleporterConfigManager>(textAsset.text);
        Debug.Log("Number of found teleporters: " + manager.teleporters.Length);
        foreach (TeleporterConfig config in manager.teleporters)
        {
            int worldID = config.worldID;
            WorldData worldData = GetWorldData(worldID);
            if (worldData == null)
            {
                Debug.LogError("Could not assign teleporter config data to worldData. WorldData is null.");
                continue;
            }

            worldData.SetTeleporterData(config);
        }
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
    
    /// <summary>
    ///     This functions returns an information text about the barrier.
    /// </summary>
    /// <param name="type">The type of the barrier</param>
    /// <param name="originWorldIndex">The index of the world the barrier is in</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns></returns>
    public string GetBarrierInfoText(BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        if (type == BarrierType.worldBarrier)
        {
            return GenerateWorldBarrierInfoText(originWorldIndex, destinationAreaIndex);
        }

        return GenerateDungeonBarrierInfoText(originWorldIndex, destinationAreaIndex);

    }

    /// <summary>
    ///     This methods generates the info text for a world barrier.
    /// </summary>
    /// <param name="originWorldIndex">The index of the area the barrier is blocking access to</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns>The generated info text</returns>
    private string GenerateWorldBarrierInfoText(int originWorldIndex, int destinationAreaIndex)
    {
        if (GetWorldData(destinationAreaIndex).isActive())
        {
            for (int i = destinationAreaIndex - 1; i > 0; i--)
            {
                if (!IsWorldUnlocked(destinationAreaIndex - i))
                {
                    int highestActiveDungeon = getHighestActiveDungeon(destinationAreaIndex - i);

                    if (highestActiveDungeon > 0)
                    {
                        return "YOU HAVE TO UNLOCK DUNGEON " + (destinationAreaIndex - i) + "-" + highestActiveDungeon +
                               " FIRST";
                    }
                    
                    return "YOU HAVE TO UNLOCK WORLD " + (destinationAreaIndex - i) + " FIRST";
                }
            }

            for (int i = 4; i > 0; i--)
            {
                if (GetWorldData(originWorldIndex).getDungeonData(i).IsActive() &&
                    !IsDungeonUnlocked(originWorldIndex, i))
                {
                    return "YOU HAVE TO UNLOCK DUNGEON " + originWorldIndex + "-" + i + " FIRST";
                }
            }

            if (destinationAreaIndex == originWorldIndex + 1)
            {
                return GenerateWorldBarrierInfoTextWithoutInBetweenWorld(originWorldIndex);
            }
            else
            {
                return GenerateWorldBarrierInfoTextWithInBetweenWorld(originWorldIndex, destinationAreaIndex);
            }
        }
            
        return "NOT UNLOCKABLE IN THIS GAME VERSION";
    }
    
    /// <summary>
    ///     This methods generates the info text for a world barrier without an world between the origin & the destination one.
    /// </summary>
    /// <param name="originWorldIndex">The index of the world the barrier is in</param>
    /// <returns>The generated info text</returns>
    private string GenerateWorldBarrierInfoTextWithoutInBetweenWorld(int originWorldIndex)
    {
        int highestUnlockedDungeon = getHighestUnlockedDungeon(originWorldIndex);
        int highestActiveDungeon = getHighestActiveDungeon(originWorldIndex);

        if (highestActiveDungeon == 0)
        {
            int activeMinigameCount = getActiveMinigameCount(originWorldIndex);
            
            if (activeMinigameCount == 0)
            {
                return "ERROR - PLEASE CONTACT THE DEVELOPERS";
            }
            
            return "COMPLETE " + activeMinigameCount + " MORE MINIGAMES TO UNLOCK THIS AREA.";
        }
        else if (highestActiveDungeon == highestUnlockedDungeon)
        {
            int activeMinigameCount = getActiveMinigameCount(originWorldIndex, highestUnlockedDungeon);

            if (activeMinigameCount == 0)
            {
                return "ERROR - PLEASE CONTACT THE DEVELOPERS";
            }
            
            return "COMPLETE " + activeMinigameCount + " MORE MINIGAMES IN DUNGEON " + originWorldIndex +
                   "-" + highestUnlockedDungeon + " TO UNLOCK THIS AREA.";
        }
        
        return "NOT UNLOCKABLE IN THIS GAME VERSION";
    }
    
    /// <summary>
    ///     This methods generates the info text for a world barrier with an world between the origin & the destination one.
    /// </summary>
    /// <param name="originWorldIndex">The index of the area the barrier is blocking access to</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns>The generated info text</returns>
    private string GenerateWorldBarrierInfoTextWithInBetweenWorld(int originWorldIndex, int destinationAreaIndex)
    {
        int inBetweenWorld = 0;

        if (destinationAreaIndex > originWorldIndex + 1)
        {
            inBetweenWorld = destinationAreaIndex - 1;
        }
        
        int highestUnlockedDungeon = getHighestUnlockedDungeon(inBetweenWorld);
        int highestActiveDungeon = getHighestActiveDungeon(inBetweenWorld);

        if (highestActiveDungeon == 0)
        { 
            int activeMinigameCount = getActiveMinigameCount(inBetweenWorld);
            
            if (activeMinigameCount == 0)
            {
                return "ERROR - PLEASE CONTACT THE DEVELOPERS";
            }
            
            return "COMPLETE " + activeMinigameCount + " MORE MINIGAMES IN WORLD " + inBetweenWorld +
                   " TO UNLOCK THIS AREA.";
        }
        else if (highestActiveDungeon == highestUnlockedDungeon)
        {
            int activeMinigameCount = getActiveMinigameCount(inBetweenWorld, highestUnlockedDungeon);

            if (activeMinigameCount == 0)
            {
                return "ERROR - PLEASE CONTACT THE DEVELOPERS";
            }
            
            return "COMPLETE " + activeMinigameCount + " MORE MINIGAMES IN DUNGEON " + inBetweenWorld +
                   "-" + highestUnlockedDungeon + " TO UNLOCK THIS AREA.";
        }
        else if(highestActiveDungeon > highestUnlockedDungeon)
        {
            return "YOU HAVE TO UNLOCK DUNGEON " + inBetweenWorld + "-" + (highestActiveDungeon) +
                   " FIRST";
        }
        
        return "NOT UNLOCKABLE IN THIS GAME VERSION";
    }
    
    /// <summary>
    ///     This methods generates the info text for a dungeon barrier.
    /// </summary>
    /// <param name="originWorldIndex">The index of the area the barrier is blocking access to</param>
    /// <param name="destinationAreaIndex">The index of the area the barrier is blocking access to</param>
    /// <returns>The generated info text</returns>
    private string GenerateDungeonBarrierInfoText(int originWorldIndex, int destinationAreaIndex)
    {
        if (GetWorldData(originWorldIndex).getDungeonData(destinationAreaIndex)
                .IsActive())
        {
            for (int i = 1; i < destinationAreaIndex; i++)
            {
                if (GetWorldData(originWorldIndex).getDungeonData(destinationAreaIndex - i)
                        .IsActive() &&
                    !IsDungeonUnlocked(originWorldIndex, destinationAreaIndex - i))
                {
                    return "YOU HAVE TO UNLOCK DUNGEON " + originWorldIndex + "-" + (destinationAreaIndex - i) +
                           " FIRST";
                }
            }

            if (destinationAreaIndex == 1)
            {
                int activeMinigameCount = getActiveMinigameCount(originWorldIndex);

                if (activeMinigameCount == 0)
                {
                    return "ERROR - PLEASE CONTACT THE DEVELOPERS";
                }
                
                return "COMPLETE " + activeMinigameCount + " MORE MINIGAMES TO UNLOCK THIS AREA.";
            }
            else
            {
                int activeMinigameCount = getActiveMinigameCount(originWorldIndex, destinationAreaIndex -1);

                if (activeMinigameCount == 0)
                {
                    return "ERROR - PLEASE CONTACT THE DEVELOPERS";
                }
                
                return "COMPLETE " + activeMinigameCount + " MORE MINIGAMES IN DUNGEON " + originWorldIndex + "-" +
                       (destinationAreaIndex - 1) + " TO UNLOCK THIS AREA.";
            }
        }

        return "NOT UNLOCKABLE IN THIS GAME VERSION";
    }
    
    /// <summary>
    ///     This methods calculates the highest unlocked dungeon in given world.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <returns>The index of the highest unlocked dungeon</returns>
    private int getHighestUnlockedDungeon(int worldIndex)
    {
        int highestUnlockedDungeon = 0;
        
        for (int i = 0; i < 4; i++)
        {
            if (IsDungeonUnlocked(worldIndex, i + 1))
            {
                highestUnlockedDungeon = i + 1;
            }
        }

        return highestUnlockedDungeon;
    }

    /// <summary>
    ///     This methods calculates the highest active dungeon in given world.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <returns>The index of the highest active dungeon</returns>
    private int getHighestActiveDungeon(int worldIndex)
    {
        int highestActiveDungeon = 0;
        
        for (int i = 0; i < 4; i++)
        {
            if (GetWorldData(worldIndex).getDungeonData(i + 1).IsActive())
            {
                highestActiveDungeon = i + 1;
            }
        }

        return highestActiveDungeon;
    }
    
    /// <summary>
    ///     This method calculates the number of active minigames in given world
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <returns>Number of active minigames</returns>
    private int getActiveMinigameCount(int worldIndex)
    {
        int activeMinigameCount = 0;

        foreach (MinigameData minigameData in GetWorldData(worldIndex)
                     .GetMinigameData())
        {
            if (minigameData.GetStatus() == global::MinigameStatus.active)
            {
                activeMinigameCount++;
            }
        }
        
        return activeMinigameCount;
    }
    
    /// <summary>
    ///     This method calculates the number of active minigames in given dungeon
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    /// <returns>Number of active minigames</returns>
    private int getActiveMinigameCount(int worldIndex, int dungeonIndex)
    {
        int activeMinigameCount = 0;

        foreach (MinigameData minigameData in GetWorldData(worldIndex)
                     .getDungeonData(dungeonIndex).GetMinigameData())
        {
            if (minigameData.GetStatus() == global::MinigameStatus.active)
            {
                activeMinigameCount++;
            }
        }
        
        return activeMinigameCount;
    }
}