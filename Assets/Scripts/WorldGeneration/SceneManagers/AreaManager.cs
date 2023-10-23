using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
///     This class manages the generation and setup of the area
/// </summary>
public class AreaManager : MonoBehaviour
{
    //TEMP
    [SerializeField] private int worldIndex;
    [SerializeField] private int dungeonIndex;

    [SerializeField] private AreaBuilder areaBuilder;
    [SerializeField] private GeneratorUIManager generatorUI;
    [SerializeField] private InspectorUIManager inspectorUI;

    private string courseID;
    private AreaData areaData;
    private AreaInformation areaIdentifier;
    private AreaInformationData areaInformation;
    private AreaGenerator areaGenerator;
    private ObjectPositionGenerator objectPositionGenerator;
    private ObjectGenerator objectGenerator;

    /// <summary>
    ///     This function sets up the area, if the game is in PLAY mode
    /// </summary>
    private void Start()
    {
        //if not in play mode: do nothing
        if(GameSettings.GetGamemode() != Gamemode.PLAY)
        {
            return;
        }

        //get area identifier
        //if area is a dungeon, get which one
        if(dungeonIndex != 0)
        {
            areaIdentifier = LoadSubScene.areaExchange;
            worldIndex = areaIdentifier.GetWorldIndex();
            dungeonIndex = areaIdentifier.GetDungeonIndex();
        }
        else
        {
            areaIdentifier = new AreaInformation(worldIndex, new Optional<int>());
        }

        //get area information
        areaInformation = GetAreaInformation(areaIdentifier);

        //Get AreaData from DataManager
        Optional<AreaData> result = DataManager.Instance.GetAreaData(areaIdentifier);
        if(!result.IsPresent())
        {
            Debug.LogError("No data found for area " + worldIndex + "-" + dungeonIndex);
            return;
        }
        areaData = result.Value();

        //if generated area: set layout and object
        if(areaData.IsGeneratedArea())
        {
            Debug.Log("Generated Area: " + worldIndex + "-" + dungeonIndex);

            //Create layout
            areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetTiles(), areaInformation);

            //Create objects
            areaBuilder.SetupAreaObjects(areaData.GetAreaMapData());
        }
        else
        {
            Debug.Log("Default Area: " + worldIndex + "-" + dungeonIndex);
            CustomAreaMapData areaMapData = LoadDefaultObjectInfo();
            if (areaIdentifier.IsDungeon())
            {
                areaMapData = SetAreaIdentifier(areaMapData);
            }

            //Create objects
            areaBuilder.SetupAreaObjects(areaMapData);
        }      
    }

    /// <summary>
    ///     This function loads the default object data for the current area
    /// </summary>
    /// <returns>A <c>CustomAreaMapData</c> object containing all default objects of the area</returns>
    private CustomAreaMapData LoadDefaultObjectInfo()
    {
        //Setup path
        string path = "AreaInfo/";
        if(areaIdentifier.IsDungeon())
        {
            path += "DungeonDefaultObjects";
        }
        else
        {
            path += "World" + areaIdentifier.GetWorldIndex() + "DefaultObjects";
        }
        Debug.Log("Loading default objects via: " + path);

        //Retrieve data
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;

        //Cast to CustomAreaMapData
        CustomAreaMapDTO dto = CustomAreaMapDTO.CreateFromJSON(json);
        CustomAreaMapData data = CustomAreaMapData.ConvertDtoToData(dto);

        return data;
    }

    /// <summary>
    ///     This function sets up the area with the layout and objects and the inspector UI
    /// </summary>
    /// <param name="areaData">The data of the area</param>
    /// <param name="areaIdentifier">The area identifier</param>
    /// <param name="cameraController">The camera</param>
    public void SetupInspector(string courseID, AreaData areaData, AreaInformation areaIdentifier, CameraMovement cameraController)
    {
        this.courseID = courseID;
        this.areaData = areaData;
        this.areaIdentifier = areaIdentifier;
        areaInformation = GetAreaInformation(areaIdentifier);

        CustomAreaMapData areaMapData;
        if (areaData.IsGeneratedArea())
        {
            areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetTiles(), areaInformation);
            areaMapData = areaData.GetAreaMapData();
        }
        else
        {
            areaMapData = GetDefaultAreaMapData();
        }

        inspectorUI.Setup(areaMapData, areaInformation, cameraController);
    }

    /// <summary>
    ///     This function sets up the area with the layout and objects and the generator UI
    /// </summary>
    /// <param name="areaData">The data of the area</param>
    /// <param name="areaIdentifier">The area identifier</param>
    /// <param name="cameraController">The camera</param>
    public void SetupGenerator(string courseID, AreaData areaData, AreaInformation areaIdentifier, CameraMovement cameraController)
    {
        //store infos
        this.courseID = courseID;
        worldIndex = areaData.GetArea().GetWorldIndex();
        dungeonIndex = 0;
        if(areaData.GetArea().IsDungeon())
        {
            dungeonIndex = areaData.GetArea().GetDungeonIndex();
        }

        this.areaData = areaData;
        this.areaIdentifier = areaIdentifier;
        areaInformation = GetAreaInformation(areaIdentifier);
        objectGenerator = new ObjectGenerator(areaIdentifier);
        if(areaData.IsGeneratedArea())
        {
            objectPositionGenerator = new ObjectPositionGenerator(areaData.GetAreaMapData().GetTiles(), areaInformation.GetObjectOffset());
        }

        //setup area
        SetupArea();

        //setup ui
        generatorUI.SetupUI(areaData, areaInformation, cameraController);
    }

    #region Setup Functions
    /// <summary>
    ///     This function retrieves generic information about the area, stored locally
    /// </summary>
    /// <param name="areaIdentifier">The area identifier</param>
    /// <returns>An <c>AreaInformationData</c> object containing the generic information</returns>
    private AreaInformationData GetAreaInformation(AreaInformation areaIdentifier)
    {
        string path;
        if (areaIdentifier.IsDungeon())
        {
            path = "AreaInfo/Dungeon";
        }
        else
        {
            path = "AreaInfo/World" + areaIdentifier.GetWorldIndex();
        }
        Debug.Log("Path: " + path);
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AreaInformationDTO areaInformationDTO = AreaInformationDTO.CreateFromJSON(json);
        AreaInformationData areaInformationData = AreaInformationData.ConvertDtoToData(areaInformationDTO);
        return areaInformationData;
    }

    /// <summary>
    ///     This function sets up the area with the layout and objects
    /// </summary>
    private void SetupArea()
    {
        if (areaData.IsGeneratedArea())
        {
            areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetTiles(), areaInformation);
            areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
        }
        else
        {
            CustomAreaMapData defaultObjects = GetDefaultAreaMapData();
            if(areaIdentifier.IsDungeon())
            {
                defaultObjects = SetAreaIdentifier(defaultObjects);
            }

            areaBuilder.SetupPlaceholderObjects(defaultObjects);
        }
    }

    /// <summary>
    ///     This function retrieves the information about the default objects in the area
    /// </summary>
    /// <returns>An <c>CustomAreaMapData</c> object containing the default objects</returns>
    private CustomAreaMapData GetDefaultAreaMapData()
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
        Debug.Log("Path: " + path);
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        CustomAreaMapDTO dto = CustomAreaMapDTO.CreateFromJSON(json);
        CustomAreaMapData data = CustomAreaMapData.ConvertDtoToData(dto);
        return data;
    }

    /// <summary>
    ///     This funciton changes the area identifier to the current one (needed for dungeons only)
    /// </summary>
    /// <param name="data">The <c>CustomAreaMapData</c> to change</param>
    /// <returns>The <c>CustomAreaMapData</c> with changed area identifier </returns>
    private CustomAreaMapData SetAreaIdentifier(CustomAreaMapData data)
    {
        foreach (MinigameSpotData minigameSpot in data.GetMinigameSpots())
        {
            minigameSpot.SetArea(areaIdentifier);
        }
        foreach (NpcSpotData npcSpot in data.GetNpcSpots())
        {
            npcSpot.SetArea(areaIdentifier);
        }
        foreach (BookSpotData bookSpot in data.GetBookSpots())
        {
            bookSpot.SetArea(areaIdentifier);
        }
        foreach (TeleporterSpotData teleporterSpot in data.GetTeleporterSpots())
        {
            teleporterSpot.SetArea(areaIdentifier);
        }
        foreach (SceneTransitionSpotData sceneTransitionSpot in data.GetSceneTransitionSpots())
        {
            sceneTransitionSpot.SetArea(areaIdentifier);
            sceneTransitionSpot.SetAreaToLoad(new AreaInformation(areaIdentifier.GetWorldIndex(), new Optional<int>()));
        }

        return data;
    }
    #endregion

    #region Area Generation Function

    #region Resetting and Saving
    /// <summary>
    ///     This function resets the area to the default, manually created one
    /// </summary>
    public void ResetArea()
    {
        areaData.Reset();
        SaveArea();
    }

    /// <summary>
    ///     This function checks, whether the current area can be saved (at least one minigame spot needed)
    /// </summary>
    /// <returns>True, if the area can be saved, false otherwise</returns>
    public bool IsAreaSaveable()
    {
        if(areaData.IsGeneratedArea())
        {
            if(areaData.GetAreaMapData().GetMinigameSpots().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///     This function saves the current area data to the backend
    /// </summary>
    public async void SaveArea()
    {
        AreaDTO areaDTO = AreaDTO.ConvertDataToDto(areaData);
        string json = JsonUtility.ToJson(areaDTO, true);
        string path;

#if UNITY_EDITOR
        //use local files in editor
        if (areaIdentifier.IsDungeon())
        {
            path = "Assets/Resources/Areas/Dungeon" + areaIdentifier.GetWorldIndex() + "-" + areaIdentifier.GetDungeonIndex() + ".json";
        }
        else
        {
            path = "Assets/Resources/Areas/World" + areaIdentifier.GetWorldIndex() + ".json";
        }
        WriteToJsonFile(json, path);
        return;
#endif
        //send area map data to backend
        path = GameSettings.GetOverworldBackendPath() + "/courses/" + courseID + "/areaMaps/" + areaIdentifier.GetWorldIndex();
        if(areaIdentifier.IsDungeon())
        {
            path += "/dungeon/" + areaIdentifier.GetDungeonIndex();
        }

        bool success = await RestRequest.PutRequest(path, json);
        if(success)
        {
            //show success message
        }
        else
        {
            //show error message
        }
    }

    //THIS FUNCTION IS ONLY NEEDED UNTIL BACKEND LOADING AND STORING IS IMPLEMENTED!
    private void WriteToJsonFile(string json, string path)
    {
        using (FileStream fs = File.Create(path))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    #endregion

    #region Generate Area
    /// <summary>
    ///     This function generates a layout
    /// </summary>
    /// <param name="size">The size of the layout</param>
    /// <param name="style">The style of the new layout</param>
    /// <param name="accessability">How much area is accessable</param>
    public void GenerateLayout(Vector2Int size, WorldStyle style, float accessability)
    {
        //Setup area generator and object and generate layout
        List<WorldConnection> worldConnections = areaInformation.GetWorldConnections();
        areaGenerator = new AreaGenerator(size, style, accessability, worldConnections);
        areaGenerator.GenerateLayout();
        string[,,] layout = areaGenerator.GetLayout();
        objectPositionGenerator = new ObjectPositionGenerator(areaGenerator.GetAccessableTiles(), areaInformation.GetObjectOffset());
        
        //Update stored data
        CustomAreaMapData areaMapData = new CustomAreaMapData(layout, style);
        areaData.SetAreaMapData(areaMapData);

        //Setup area
        areaBuilder.SetupAreaLayout(layout, areaInformation);
        areaBuilder.SetupPlaceholderObjects(areaMapData);
    }

    /// <summary>
    ///     This function creates minigame spots
    /// </summary>
    /// <param name="amount">The amount of minigame spots to create</param>
    public void GenerateMinigames(int amount)
    {
        //Generate Positions
        objectPositionGenerator.GenerateMinigamePositions(amount);
        List<Vector2> minigamePositions = objectPositionGenerator.GetMinigameSpotPositions();

        //Generate MinigameSpotData objects
        List<MinigameSpotData> minigameSpots = objectGenerator.GenerateMinigameSpots(minigamePositions);
        areaData.GetAreaMapData().SetMinigameSpots(minigameSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
    }

    /// <summary>
    ///     This function creates npc spots
    /// </summary>
    /// <param name="amount">The amount of npc spots to create</param>
    public void GenerateNPCs(int amount)
    {
        //Generate Positions
        objectPositionGenerator.GenerateNpcPositions(amount);
        List<Vector2> npcPositions = objectPositionGenerator.GetNpcSpotPositions();

        //Generate NpcSpotData objects
        List<NpcSpotData> npcSpots = objectGenerator.GenerateNpcSpots(npcPositions);
        areaData.GetAreaMapData().SetNpcSpots(npcSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
    }

    /// <summary>
    ///     This function creates book spots
    /// </summary>
    /// <param name="amount">The amount of book spots to create</param>
    public void GenerateBooks(int amount)
    {
        //Generate Positions
        objectPositionGenerator.GenerateBookPositions(amount);
        List<Vector2> bookPositions = objectPositionGenerator.GetBookSpotPositions();

        //Generate BookSpotData objects
        List<BookSpotData> bookSpots = objectGenerator.GenerateBookSpots(bookPositions);
        areaData.GetAreaMapData().SetBookSpots(bookSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
    }

    /// <summary>
    ///     This function creates teleporter spots
    /// </summary>
    /// <param name="amount">The amount of teleporter spots to create</param>
    public void GenerateTeleporters(int amount)
    {
        //Generate Positions
        objectPositionGenerator.GenerateTeleporterPositions(amount);
        List<Vector2> teleporterPositions = objectPositionGenerator.GetTeleporterSpotPositions();

        //Generate TeleporterSpotData objects
        List<TeleporterSpotData> teleporterSpots = objectGenerator.GenerateTeleporterSpots(teleporterPositions);
        areaData.GetAreaMapData().SetTeleporterSpots(teleporterSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
    }

    /// <summary>
    ///     This function creates dungeon spots
    /// </summary>
    /// <param name="amount">The amount of dungeon spots to create</param>
    public void GenerateDungeons(int amount)
    {
        //Generate Positions
        objectPositionGenerator.GenerateDungeonPositions(amount);
        List<Vector2> dungeonPositions = objectPositionGenerator.GetDungeonSpotPositions();

        //Generate SceneTransitionSpotData objects
        List<SceneTransitionSpotData> dungeonSpots = objectGenerator.GenerateDungeonSpots(dungeonPositions);
        areaData.GetAreaMapData().SetSceneTransitionSpots(dungeonSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
    }
    #endregion

    #endregion
}
