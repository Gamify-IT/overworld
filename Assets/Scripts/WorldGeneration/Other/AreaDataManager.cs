using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
///     This class manages the <c>AreaData</c> for all areas in the play mode
/// </summary>
public class AreaDataManager
{
    Dictionary<int, WorldAreas> worldAreas;

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

        string courseID = GameSettings.GetCourseID();
        string path = GameSettings.GetOverworldBackendPath() + "/courses/" + courseID + "/area";
        Optional<List<AreaDTO>> areas = await RestRequest.GetListRequest<AreaDTO>(path);

        int amountWorlds = areas.Value().Count;
        Debug.Log(amountWorlds + "worlds found for this course");

        for (int worldIndex = 1; worldIndex <= amountWorlds; worldIndex++)
        {
            bool successfulLoading = await FetchWorldData(worldIndex);
            if(!successfulLoading)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This function retrieves the data for the world and all its dungeons
    /// </summary>
    /// <param name="worldIndex">The world to fetch the data from</param>
    /// <returns>True, if everything went fine, false otherwise </returns>
    private async UniTask<bool> FetchWorldData(int worldIndex)
    {
        AreaInformation currentArea = new AreaInformation(worldIndex, new Optional<int>());
        worldAreas.Add(worldIndex, new WorldAreas());

        AreaData worldData = await FetchData(currentArea);
        if (worldData == null)
        {
            return false;
        }
        worldAreas[worldIndex].AddArea(0, worldData);

        int amountDungeons;
        if (worldData.IsGeneratedArea())
        {
            UpdateTeleporters(currentArea, worldData.GetAreaMapData().GetTeleporterSpots());
            amountDungeons = worldData.GetAreaMapData().GetSceneTransitionSpots().Count;
        }
        else
        {
            UpdateTeleporters(currentArea);
            amountDungeons = 4;
        }

        for (int dungeonIndex = 1; dungeonIndex <= amountDungeons; dungeonIndex++)
        {
            bool dungeonSuccessful = await FetchDungeonData(worldIndex, dungeonIndex);
            if(!dungeonSuccessful)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     This function retrieves the data for the world and all its dungeons
    /// </summary>
    /// <param name="worldIndex">The world the dungeon is in</param>
    /// <param name="dungeonIndex">The dungeon to fetch the data from</param>
    /// <returns>True, if everything went fine, false otherwise </returns>
    private async UniTask<bool> FetchDungeonData(int worldIndex, int dungeonIndex)
    {
        AreaInformation currentArea = new AreaInformation(worldIndex, new Optional<int>(dungeonIndex));
        AreaData dungeonData = await FetchData(currentArea);
        if (dungeonData == null)
        {
            return false;
        }
        worldAreas[worldIndex].AddArea(dungeonIndex, dungeonData);

        if(dungeonData.IsGeneratedArea())
        {
            UpdateTeleporters(currentArea, dungeonData.GetAreaMapData().GetTeleporterSpots());
        }
        else
        {
            UpdateTeleporters(currentArea);
        }

        return true;
    }

    /// <summary>
    ///     This function loads the data for the given area from the backend
    /// </summary>
    /// <param name="currentArea">The area to load</param>
    /// <returns>The <c>AreaData</c>, if the request was successful, <c>null</c> otherwise</returns>
    private async UniTask<AreaData> FetchData(AreaInformation currentArea)
    {
        //get specific path
        string path = GameSettings.GetOverworldBackendPath() + "/courses/" + GameSettings.GetCourseID() + "/area/" + currentArea.GetWorldIndex().ToString();

        if(currentArea.IsDungeon())
        {
            path += "/dungeon/" + currentArea.GetDungeonIndex();
        }

        //---------------------
        // area data error when starting overworld with run config
        //---------------------

        //fetch area data from backend
        Optional<AreaDTO> areaDTO = await RestRequest.GetRequest<AreaDTO>(path);
        if(areaDTO.IsPresent())
        {
            AreaData areaData = AreaData.ConvertDtoToData(areaDTO.Value());
            return areaData;
        }

        return null;
    }

    /// <summary>
    ///     This function gets the local stored area data as dummy data
    /// </summary>
    public void GetDummyData()
    {
        worldAreas = new Dictionary<int, WorldAreas>();

        int amountWorlds = GameSettings.GetMaxWorlds();

        for (int worldIndex = 1; worldIndex <= amountWorlds; worldIndex++)
        {
            Debug.Log("Loading local world: " + worldIndex);
            AreaInformation currentArea = new AreaInformation(worldIndex, new Optional<int>());
            worldAreas.Add(worldIndex, new WorldAreas());

            //get world data
            AreaData worldData = LoadLocalData(currentArea);
            worldAreas[worldIndex].AddArea(0, worldData);

            //get amount of dungeons
            int amountDungeons;
            if (worldData.IsGeneratedArea())
            {
                UpdateTeleporters(currentArea, worldData.GetAreaMapData().GetTeleporterSpots());
                amountDungeons = worldData.GetAreaMapData().GetSceneTransitionSpots().Count;
            }
            else
            {
                UpdateTeleporters(currentArea);
                amountDungeons = 4;
            }

            //get dungeon data
            for (int dungeonIndex = 1; dungeonIndex <= amountDungeons; dungeonIndex++)
            {
                Debug.Log("Loading local dungeon: " + worldIndex + "-" + dungeonIndex);
                currentArea = new AreaInformation(worldIndex, new Optional<int>(dungeonIndex));
                AreaData dungeonData = LoadLocalData(currentArea);
                worldAreas[worldIndex].AddArea(dungeonIndex, dungeonData);

                if(dungeonData.IsGeneratedArea())
                {
                    UpdateTeleporters(currentArea, dungeonData.GetAreaMapData().GetTeleporterSpots());
                }
                else
                {
                    UpdateTeleporters(currentArea);
                }
            }
        }
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

    /// <summary>
    ///     This function updates the position and unlocked flag of the given teleporter spots
    /// </summary>
    /// <param name="areaIdentifier">The area the teleports are in</param>
    /// <param name="teleporters">The teleport spots</param>
    private void UpdateTeleporters(AreaInformation areaIdentifier, List<TeleporterSpotData> teleporters)
    {
        Debug.Log("Update teleports of " + areaIdentifier.GetWorldIndex() + "-" + areaIdentifier.GetDungeonIndex());
        foreach (TeleporterSpotData spot in teleporters)
        {
            string name = spot.GetName();
            int worldId = spot.GetArea().GetWorldIndex();
            int dungeonId = 0;
            if(spot.GetArea().IsDungeon())
            {
                dungeonId = spot.GetArea().GetDungeonIndex();
            }
            int number = spot.GetIndex();
            Vector2 position = spot.GetPosition();
            TeleporterData data = new TeleporterData(name, worldId, dungeonId, number, position, false);
            DataManager.Instance.AddTeleporterInformation(areaIdentifier, number, data);
        }
    }

    /// <summary>
    ///     This function updates the position and unlocked flag with the default teleporters
    /// </summary>
    /// <param name="areaIdentifier">The area the teleports are in</param>
    private void UpdateTeleporters(AreaInformation areaIdentifier)
    {
        List<TeleporterSpotData> teleporterSpots = GetDefaultTeleporters(areaIdentifier);
        UpdateTeleporters(areaIdentifier, teleporterSpots);
    }

    /// <summary>
    ///     This function retrieves the default teleporters for the given area
    /// </summary>
    /// <param name="areaIdentifier">The area to get the default teleporters from</param>
    /// <returns>A list containing the default teleporters</returns>
    private List<TeleporterSpotData> GetDefaultTeleporters(AreaInformation areaIdentifier)
    {
        string path;
        if (areaIdentifier.IsDungeon())
        {
            path = "AreaInfo/DungeonDefaultObjects";
        }
        else
        {
            path = "AreaInfo/World" + areaIdentifier.GetWorldIndex() + "DefaultObjects";
        }

        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        CustomAreaMapDTO dto = CustomAreaMapDTO.CreateFromJSON(json);
        CustomAreaMapData data = CustomAreaMapData.ConvertDtoToData(dto);

        foreach (TeleporterSpotData teleporterSpot in data.GetTeleporterSpots())
        {
            teleporterSpot.SetArea(areaIdentifier);

            if (areaIdentifier.IsDungeon())
            {
                string teleporterName = "Dungeon " + areaIdentifier.GetWorldIndex() + "-" + areaIdentifier.GetDungeonIndex() + " " + teleporterSpot.GetName();
                teleporterSpot.SetName(teleporterName);
            }
        }

        return data.GetTeleporterSpots();
    }
}
