using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AreaDataManager
{
    #region Attributes
    Dictionary<int, WorldAreas> worldAreas;
    #endregion

    public AreaDataManager()
    {
        worldAreas = new Dictionary<int, WorldAreas>();
    }

    /// <summary>
    ///     This function loads all area data from the backend and stores them in the <c>areas</c> dictionary
    /// </summary>
    public async UniTask<bool> FetchData()
    {
        Debug.Log("Loading area data");
        bool loadingError = false;

        int amountWorlds = GameSettings.GetMaxWorlds();

        AreaInformation currentArea = new AreaInformation(1, new Optional<int>());

        for (int worldIndex = 1; worldIndex <= amountWorlds; worldIndex++)
        {
            currentArea = new AreaInformation(worldIndex, new Optional<int>());
            worldAreas.Add(worldIndex, new WorldAreas());

            AreaData worldData = await FetchData(currentArea);
            if(worldData == null)
            {
                worldData = LoadLocalData(currentArea);
            }
            Debug.Log("World " + worldIndex + " generated: " + worldData.IsGeneratedArea());
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
                currentArea.SetDungeonIndex(dungeonIndex);
                AreaData dungeonData = await FetchData(currentArea);
                if(dungeonData == null)
                {
                    dungeonData = LoadLocalData(currentArea);
                }
                Debug.Log("Dungeon " + worldIndex + "-" + dungeonIndex + " generated: " + dungeonData.IsGeneratedArea());
                worldAreas[worldIndex].AddArea(dungeonIndex, dungeonData);
            }
        }

        return loadingError;
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
