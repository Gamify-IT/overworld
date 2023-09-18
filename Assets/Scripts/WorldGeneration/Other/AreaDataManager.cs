using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AreaDataManager
{
    #region Attributes
    Dictionary<int, WorldAreas> worldAreas;

    private int maxDungeons;
    private int maxMinigames;
    private int maxNPCs;
    private int maxBooks;
    private int maxTeleporters;
    #endregion

    public AreaDataManager()
    {
        worldAreas = new Dictionary<int, WorldAreas>();
        maxDungeons = 0;
        maxMinigames = 0;
        maxNPCs = 0;
        maxBooks = 0;
        maxTeleporters = 0;
    }

    /// <summary>
    ///     This function loads all area data from the backend and stores them in the <c>areas</c> dictionary
    /// </summary>
    public async UniTask<bool> FetchData()
    {
        Debug.Log("Loading area data");
        bool loadingError = false;

        int amountWorlds = GameSettings.GetMaxWorlds();

        for (int worldIndex = 1; worldIndex <= amountWorlds; worldIndex++)
        {
            bool successfulLoading = await FetchWorldData(worldIndex);
            if(!successfulLoading)
            {
                loadingError = true;
            }
        }

        //DEBUG:
        Debug.Log("Max Dungeons: " + maxDungeons);
        Debug.Log("Max Minigames: " + maxMinigames);
        Debug.Log("Max NPCs: " + maxNPCs);
        Debug.Log("Max Books: " + maxBooks);
        Debug.Log("Max Teleporters: " + maxTeleporters);

        return loadingError;
    }

    /// <summary>
    ///     This function retrieves the data for the world and all its dungeons
    /// </summary>
    /// <param name="worldIndex">The world to fetch the data from</param>
    /// <returns>True, if everything went fine, false otherwise </returns>
    private async UniTask<bool> FetchWorldData(int worldIndex)
    {
        bool successfulLoading = true;

        AreaInformation currentArea = new AreaInformation(worldIndex, new Optional<int>());
        worldAreas.Add(worldIndex, new WorldAreas());

        AreaData worldData = await FetchData(currentArea);
        if (worldData == null)
        {
            successfulLoading = false;
            worldData = LoadLocalData(currentArea);
        }
        UpdateMaxObjectCount(worldData);
        worldAreas[worldIndex].AddArea(0, worldData);

        int amountDungeons;
        if (worldData.IsGeneratedArea())
        {
            amountDungeons = worldData.GetAreaMapData().GetSceneTransitionSpots().Count;
        }
        else
        {
            amountDungeons = 4;
        }

        for (int dungeonIndex = 1; dungeonIndex <= amountDungeons; dungeonIndex++)
        {
            bool dungeonSuccessful = await FetchDungeonData(worldIndex, dungeonIndex);
            if(!dungeonSuccessful)
            {
                successfulLoading = false;
            }
        }

        return successfulLoading;
    }

    /// <summary>
    ///     This function retrieves the data for the world and all its dungeons
    /// </summary>
    /// <param name="worldIndex">The world the dungeon is in</param>
    /// <param name="dungeonIndex">The dungeon to fetch the data from</param>
    /// <returns>True, if everything went fine, false otherwise </returns>
    private async UniTask<bool> FetchDungeonData(int worldIndex, int dungeonIndex)
    {
        bool successfulLoading = true;

        AreaInformation currentArea = new AreaInformation(worldIndex, new Optional<int>(dungeonIndex));
        AreaData dungeonData = await FetchData(currentArea);
        if (dungeonData == null)
        {
            successfulLoading = false;
            dungeonData = LoadLocalData(currentArea);
        }
        UpdateMaxObjectCount(dungeonData);
        worldAreas[worldIndex].AddArea(dungeonIndex, dungeonData);

        return successfulLoading;
    }

    /// <summary>
    ///     This function loads the data for the given area from the backend
    /// </summary>
    /// <param name="currentArea">The area to load</param>
    /// <returns>The <c>AreaData</c>, if the request was successful, <c>null</c> otherwise</returns>
    private async UniTask<AreaData> FetchData(AreaInformation currentArea)
    {
        string backendPath = GameSettings.GetOverworldBackendPath();

        //TODO: add specific path
        string path = backendPath + "";

        //TODO: add backend calling
        return null;
    }

    /// <summary>
    ///     This function loads the current area data from a local json file
    /// </summary>
    /// <returns>The <c>AreaData</c> stored in the local files</returns>
    private AreaData LoadLocalData(AreaInformation currentArea)
    {
        string path;
        if (currentArea.IsDungeon())
        {
            path = "Areas/Dungeon" + currentArea.GetWorldIndex() + "-" + currentArea.GetDungeonIndex();
        }
        else
        {
            path = "Areas/World" + currentArea.GetWorldIndex();
        }

        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AreaDTO areaDTO = AreaDTO.CreateFromJSON(json);
        AreaData areaData = AreaData.ConvertDtoToData(areaDTO);
        areaData.SetArea(currentArea);
        return areaData;
    }

    private void UpdateMaxObjectCount(AreaData areaData)
    {
        int amountDungeons = 0;
        int amountMinigames;
        int amountNpcs;
        int amountBooks;
        int amountTeleporters;

        //retrieve values
        if(areaData.IsGeneratedArea())
        {
            if(!areaData.GetArea().IsDungeon())
            {
                amountDungeons = areaData.GetAreaMapData().GetSceneTransitionSpots().Count;
            }
            amountMinigames = areaData.GetAreaMapData().GetMinigameSpots().Count;
            amountNpcs = areaData.GetAreaMapData().GetNpcSpots().Count;
            amountBooks = areaData.GetAreaMapData().GetBookSpots().Count;
            amountTeleporters = areaData.GetAreaMapData().GetTeleporterSpots().Count;
        }
        else
        {
            amountDungeons = 4;
            amountMinigames = 12;
            amountNpcs = 10;
            amountBooks = 5;
            amountTeleporters = 6;
        }

        //check if higher than current max values
        if(amountDungeons > maxDungeons)
        {
            maxDungeons = amountDungeons;
        }

        if(amountMinigames > maxMinigames)
        {
            maxMinigames = amountMinigames;
        }

        if(amountNpcs > maxNPCs)
        {
            maxNPCs = amountNpcs;
        }

        if(amountBooks > maxBooks)
        {
            maxBooks = amountBooks;
        }

        if(amountTeleporters > maxTeleporters)
        {
            maxTeleporters = amountTeleporters;
        }
    }

    /// <summary>
    ///     This function returns the area data for the requested area
    /// </summary>
    /// <param name="area">The area identifier</param>
    /// <returns>An optional containing the <c>AreaData</c>, if present, an empty optional otherwise</returns>
    public Optional<AreaData> GetAreaData(AreaInformation area)
    {
        Debug.Log("Requesting area data");
        int dungeonIndex = 0;
        if(area.IsDungeon())
        {
            dungeonIndex = area.GetDungeonIndex();
        }

        return worldAreas[area.GetWorldIndex()].GetArea(dungeonIndex);
    }
}
