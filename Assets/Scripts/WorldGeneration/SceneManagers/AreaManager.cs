using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;

/// <summary>
///     This class manages the generation and setup of the area
/// </summary>
public class AreaManager : MonoBehaviour
{
    [SerializeField] private int worldIndex;
    [SerializeField] private int dungeonIndex;

    [SerializeField] private AreaBuilder areaBuilder;
    [SerializeField] private GeneratorUIManager generatorUI;
    [SerializeField] private InspectorUIManager inspectorUI;

    private string courseID;
    private bool demoMode;
    private AreaGeneratorManager areaGeneratorManager;
    private AreaData areaData;
    private AreaInformation areaIdentifier;
    private AreaInformationData areaInformation;
    private ObjectPositionGenerator objectPositionGenerator;
    private ObjectGenerator objectGenerator;
    private List<BarrierSpotData> worldConnectionBarriers;

    /// <summary>
    ///     This function sets up the area, if the game is in PLAY mode
    /// </summary>
    private void Start()
    {
        //if not in play mode: do nothing
        if (GameSettings.GetGamemode() != Gamemode.PLAY)
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
            List<SceneTransitionSpotData> dungeonSpots = areaData.GetAreaMapData().GetSceneTransitionSpots();
            areaData.GetAreaMapData().GetLayout().AddDungeonSpots(dungeonSpots, areaInformation.GetObjectOffset());
            areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetLayout().GetTileSprites(), areaInformation);
            areaBuilder.RemoveAdditionalObjects();

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

        GameManager.Instance.SetData(worldIndex, dungeonIndex);
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
    public void SetupInspector(string courseID, AreaData areaData, AreaInformation areaIdentifier, CameraMovement cameraController, bool demoMode)
    {
        this.courseID = courseID;
        this.demoMode = demoMode;
        this.areaData = areaData;
        this.areaIdentifier = areaIdentifier;
        areaInformation = GetAreaInformation(areaIdentifier);

        CustomAreaMapData areaMapData;
        if (areaData.IsGeneratedArea())
        {
            List<SceneTransitionSpotData> dungeonSpots = areaData.GetAreaMapData().GetSceneTransitionSpots();
            this.areaData.GetAreaMapData().GetLayout().AddDungeonSpots(dungeonSpots, areaInformation.GetObjectOffset());
            areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetLayout().GetTileSprites(), areaInformation);
            areaBuilder.RemoveAdditionalObjects();
            areaMapData = areaData.GetAreaMapData();
        }
        else
        {
            areaMapData = GetDefaultAreaMapData();
        }

        inspectorUI.Setup(areaMapData, areaInformation, cameraController, areaData.GetArea());
    }

    /// <summary>
    ///     This function sets up the area with the layout and objects and the generator UI
    /// </summary>
    /// <param name="areaData">The data of the area</param>
    /// <param name="areaIdentifier">The area identifier</param>
    /// <param name="cameraController">The camera</param>
    public void SetupGenerator(string courseID, AreaGeneratorManager areaGeneratorManager, AreaData areaData, AreaInformation areaIdentifier, CameraMovement cameraController, bool setupUI, bool demoMode)
    {
        //store infos
        this.courseID = courseID;
        this.demoMode = demoMode;
        this.areaGeneratorManager = areaGeneratorManager;
        worldIndex = areaData.GetArea().GetWorldIndex();
        dungeonIndex = 0;
        if(areaData.GetArea().IsDungeon())
        {
            dungeonIndex = areaData.GetArea().GetDungeonIndex();
        }

        this.areaData = areaData;
        this.areaIdentifier = areaIdentifier;
        areaInformation = GetAreaInformation(areaIdentifier);
        objectGenerator = new ObjectGenerator(areaIdentifier, areaInformation.GetObjectOffset());
        worldConnectionBarriers = new List<BarrierSpotData>();
        if(areaData.IsGeneratedArea())
        {
            List<SceneTransitionSpotData> dungeonSpots = areaData.GetAreaMapData().GetSceneTransitionSpots();
            this.areaData.GetAreaMapData().GetLayout().AddDungeonSpots(dungeonSpots, areaInformation.GetObjectOffset());

            objectPositionGenerator = new ObjectPositionGenerator(areaData.GetAreaMapData().GetLayout().GetCellTypes(),  
                areaInformation.GetWorldConnections(), 
                areaData.GetAreaMapData().GetLayout().GetStyle());

            objectPositionGenerator.SetMinigameSpots(areaData.GetAreaMapData().GetMinigameSpots(), areaInformation.GetObjectOffset());
            objectPositionGenerator.SetNpcSpots(areaData.GetAreaMapData().GetNpcSpots(), areaInformation.GetObjectOffset());
            objectPositionGenerator.SetBookSpots(areaData.GetAreaMapData().GetBookSpots(), areaInformation.GetObjectOffset());
            objectPositionGenerator.SetTeleporterSpots(areaData.GetAreaMapData().GetTeleporterSpots(), areaInformation.GetObjectOffset());
            objectPositionGenerator.SetDungeonSpots(areaData.GetAreaMapData().GetSceneTransitionSpots(), areaInformation.GetObjectOffset());
        }

        //setup area
        SetupArea();

        //setup ui
        generatorUI.SetupUI(areaData, areaInformation, cameraController, setupUI);
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
            areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetLayout().GetTileSprites(), areaInformation);
            areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
            areaBuilder.RemoveAdditionalObjects();
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
            string teleporterName = "Dungeon " + areaIdentifier.GetWorldIndex() + "-" + areaIdentifier.GetDungeonIndex() + " " + teleporterSpot.GetName();
            teleporterSpot.SetName(teleporterName);
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
    public async UniTask<bool> ResetArea()
    {
        AreaData backup = areaData;
        areaData = new AreaData(areaIdentifier, new Optional<CustomAreaMapData>());
        bool success = await SaveArea();

        if(success)
        {
            //reload scene
            areaGeneratorManager.ReloadArea(areaData);
        }
        else
        {
            areaData = backup;
        }

        return success;
    }

    /// <summary>
    ///     This function checks, whether the current area can be saved (at least one minigame spot needed, for dungeons exactly one dungeon spot)
    /// </summary>
    /// <returns>True, if the area can be saved, false otherwise</returns>
    public bool IsAreaSaveable()
    {
        if(!areaData.IsGeneratedArea())
        {
            //no generated area
            return false;
        }

        if (areaIdentifier.IsDungeon() && areaData.GetAreaMapData().GetSceneTransitionSpots().Count == 0)
        {
            //dungeon withouth exit cannot be saved
            return false;
        }

        if (areaData.GetAreaMapData().GetMinigameSpots().Count == 0)
        {
            //area without minigames cannot be saved
            return false;
        }

        return true;
    }

    /// <summary>
    ///     This function saves the current area data to the backend
    /// </summary>
    public async UniTask<bool> SaveArea()
    {
        if(demoMode)
        {
            return true;
        }

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
        return true;
#endif
        //send area map data to backend
        path = GameSettings.GetOverworldBackendPath() + "/courses/" + courseID + "/area/" + areaIdentifier.GetWorldIndex();
        if(areaIdentifier.IsDungeon())
        {
            path += "/dungeon/" + areaIdentifier.GetDungeonIndex();
        }

        bool success = await RestRequest.PutRequest(path, json);
        return success;
    }

    /// <summary>
    ///     This function writes the given json in a file at the given path
    /// </summary>
    /// <param name="json">The json to write</param>
    /// <param name="path">The path of the file to write to</param>
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
    /// <param name="layoutGeneratorType">The type of layout generator to be used</param>
    /// <param name="accessability">How much area is accessable</param>
    /// <param name="seed">The seed to be used, if wanted</param>
    public void GenerateLayout(Vector2Int size, WorldStyle style, LayoutGeneratorType layoutGeneratorType, int accessability, string seed)
    {
        //Setup area generator and object and generate layout
        List<WorldConnection> worldConnections = areaInformation.GetWorldConnections();

        //setup layout generator
        LayoutGenerator layoutGenerator = new CellularAutomataGenerator(seed, size, accessability, worldConnections);

        switch (layoutGeneratorType)
        {
            case LayoutGeneratorType.CELLULAR_AUTOMATA:
                layoutGenerator = new CellularAutomataGenerator(seed, size, accessability, worldConnections);
                break;

            case LayoutGeneratorType.DRUNKARDS_WALK:
                layoutGenerator = new DrunkardsWalkGenerator(seed, size, accessability, worldConnections);
                break;

            case LayoutGeneratorType.ISLAND_CELLULAR_AUTOMATA:
                layoutGenerator = new IslandsGenerator(seed, size, accessability, worldConnections, RoomGenerator.CELLULAR_AUTOMATA);
                break;

            case LayoutGeneratorType.ISLAND_DRUNKARDS_WALK:
                layoutGenerator = new IslandsGenerator(seed, size, accessability, worldConnections, RoomGenerator.DRUNKARDS_WALK);
                break;
        }

        //generate layout
        layoutGenerator.GenerateLayout();
        CellType[,] baseLayout = layoutGenerator.GetLayout();
        
        //polish layout
        LayoutPolisher polisher = new LayoutPolisher(style, baseLayout);
        CellType[,] polishedLayout = polisher.Polish();

        //convert layout
        LayoutConverter converter = new SavannaConverter(polishedLayout);

        switch (style)
        {
            case WorldStyle.SAVANNA:
                converter = new SavannaConverter(polishedLayout);
                break;

            case WorldStyle.CAVE:
                converter = new CaveConverter(polishedLayout);
                break;

            case WorldStyle.BEACH:
                converter = new BeachConverter(polishedLayout);
                break;

            case WorldStyle.FOREST:
                converter = new ForestConverter(polishedLayout);
                break;
        }

        converter.Convert();
        TileSprite[,,] tileLayout = converter.GetTileSprites();

        //create world objects
        EnvironmentObjectGenerator environmentObjectGenerator = new EnvironmentObjectGenerator(polishedLayout, tileLayout, style, seed);
        environmentObjectGenerator.AddObjects();
        polishedLayout = environmentObjectGenerator.GetCellTypes();
        tileLayout = environmentObjectGenerator.GetTileSprites();

        //setup object position generator
        objectPositionGenerator = new ObjectPositionGenerator(polishedLayout, worldConnections, style);

        //Update stored data
        Layout layout = new Layout(areaIdentifier, tileLayout, polishedLayout, layoutGeneratorType, seed, accessability, style);
        CustomAreaMapData areaMapData = new CustomAreaMapData(layout);
        areaData.SetAreaMapData(areaMapData);

        //Setup area
        areaBuilder.SetupAreaLayout(tileLayout, areaInformation);
        areaBuilder.SetupPlaceholderObjects(areaMapData);
        areaBuilder.RemoveAdditionalObjects();        
    }

    /// <summary>
    ///     This function adds the world connections barriers, unless the area is a dungeon or the world connections are already set
    /// </summary>
    public void AddWorldConnectionBarriers()
    {
        if(!areaData.IsGeneratedArea())
        {
            return;
        }

        if (areaData.GetAreaMapData().GetBarrierSpots().Count == 0)
        {
            worldConnectionBarriers = objectGenerator.GenerateWorldBarrierSpots(objectPositionGenerator.GetWorldBarrierSpots(areaIdentifier));
            areaData.GetAreaMapData().SetBarrierSpots(worldConnectionBarriers);
            areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
        }
    }

    /// <summary>
    ///     This function removes all generated objects
    /// </summary>
    public void ResetObjects()
    {
        objectPositionGenerator.ResetObjects();

        //Remove object spots
        areaData.GetAreaMapData().SetMinigameSpots(new List<MinigameSpotData>());
        areaData.GetAreaMapData().SetNpcSpots(new List<NpcSpotData>());
        areaData.GetAreaMapData().SetBookSpots(new List<BookSpotData>());
        areaData.GetAreaMapData().SetTeleporterSpots(new List<TeleporterSpotData>());

        //remove dungeon entrance / exit tiles from layout
        areaData.GetAreaMapData().GetLayout().RemoveDungeonSpots(areaData.GetAreaMapData().GetSceneTransitionSpots(), areaInformation.GetObjectOffset());
        areaData.GetAreaMapData().SetSceneTransitionSpots(new List<SceneTransitionSpotData>());

        areaData.GetAreaMapData().SetBarrierSpots(new List<BarrierSpotData>());

        //Remove Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());
        areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetLayout().GetTileSprites(), areaInformation);
    }

    /// <summary>
    ///     This function creates minigame spots
    /// </summary>
    /// <param name="amount">The amount of minigame spots to create</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateMinigames(int amount)
    {
        //Generate Positions
        bool success = objectPositionGenerator.GenerateMinigamePositions(amount);
        List<Vector2Int> minigamePositions = objectPositionGenerator.GetMinigameSpotPositions();

        //Generate MinigameSpotData objects
        List<MinigameSpotData> minigameSpots = objectGenerator.GenerateMinigameSpots(minigamePositions);
        areaData.GetAreaMapData().SetMinigameSpots(minigameSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());

        return success;
    }

    /// <summary>
    ///     This function creates npc spots
    /// </summary>
    /// <param name="amount">The amount of npc spots to create</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateNPCs(int amount)
    {
        //Generate Positions
        bool success = objectPositionGenerator.GenerateNpcPositions(amount);
        List<Vector2Int> npcPositions = objectPositionGenerator.GetNpcSpotPositions();

        //Generate NpcSpotData objects
        List<NpcSpotData> npcSpots = objectGenerator.GenerateNpcSpots(npcPositions);
        areaData.GetAreaMapData().SetNpcSpots(npcSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());

        return success;
    }

    /// <summary>
    ///     This function creates book spots
    /// </summary>
    /// <param name="amount">The amount of book spots to create</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateBooks(int amount)
    {
        //Generate Positions
        bool success = objectPositionGenerator.GenerateBookPositions(amount);
        List<Vector2Int> bookPositions = objectPositionGenerator.GetBookSpotPositions();

        //Generate BookSpotData objects
        List<BookSpotData> bookSpots = objectGenerator.GenerateBookSpots(bookPositions);
        areaData.GetAreaMapData().SetBookSpots(bookSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());

        return success;
    }

    /// <summary>
    ///     This function creates teleporter spots
    /// </summary>
    /// <param name="amount">The amount of teleporter spots to create</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateTeleporters(int amount)
    {
        //Generate Positions
        bool success = objectPositionGenerator.GenerateTeleporterPositions(amount);
        List<Vector2Int> teleporterPositions = objectPositionGenerator.GetTeleporterSpotPositions();

        //Generate TeleporterSpotData objects
        List<TeleporterSpotData> teleporterSpots = objectGenerator.GenerateTeleporterSpots(teleporterPositions);
        areaData.GetAreaMapData().SetTeleporterSpots(teleporterSpots);

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());

        return success;
    }

    /// <summary>
    ///     This function creates dungeon spots
    /// </summary>
    /// <param name="amount">The amount of dungeon spots to create</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateDungeons(int amount)
    {
        //Remove previous positions
        areaData.GetAreaMapData().GetLayout().RemoveDungeonSpots(areaData.GetAreaMapData().GetSceneTransitionSpots(), areaInformation.GetObjectOffset());
        areaData.GetAreaMapData().SetBarrierSpots(new List<BarrierSpotData>());

        //Generate new positions
        bool success = objectPositionGenerator.GenerateDungeonPositions(amount);
        List<DungeonSpotPosition> dungeonPositions = objectPositionGenerator.GetDungeonSpotPositions();

        //Generate SceneTransitionSpotData objects, adapt layout
        List<SceneTransitionSpotData> dungeonSpots = objectGenerator.GenerateDungeonSpots(dungeonPositions);
        areaData.GetAreaMapData().SetSceneTransitionSpots(dungeonSpots);
        areaData.GetAreaMapData().GetLayout().AddDungeonSpots(dungeonSpots, areaInformation.GetObjectOffset());
        areaBuilder.SetupAreaLayout(areaData.GetAreaMapData().GetLayout().GetTileSprites(), areaInformation);

        //Create barriers for the dungeon spots + add world barriers, if area is a world (no barriers in dungeons)
        if(!areaIdentifier.IsDungeon())
        {
            List<BarrierSpotData> dungeonBarrierSpots = objectGenerator.GenerateDungeonBarrierSpots(dungeonPositions);
            List<BarrierSpotData> worldBarrierSpots = worldConnectionBarriers;
            List<BarrierSpotData> barrierSpots = new List<BarrierSpotData>();
            barrierSpots.AddRange(dungeonBarrierSpots);
            barrierSpots.AddRange(worldBarrierSpots);
            areaData.GetAreaMapData().SetBarrierSpots(barrierSpots);
        }        

        //Create Placeholders
        areaBuilder.SetupPlaceholderObjects(areaData.GetAreaMapData());

        return success;
    }
    #endregion

    #endregion

    #region Object toggle

    /// <summary>
    ///     This function enables or disables the minigame icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the minigame icons should be shown</param>
    public void DisplayMinigames(bool active)
    {
        areaBuilder.DisplayMinigames(active);
    }

    /// <summary>
    ///     This function enables or disables the npc icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the npc icons should be shown</param>
    public void DisplayNpcs(bool active)
    {
        areaBuilder.DisplayNpcs(active);
    }

    /// <summary>
    ///     This function enables or disables the book icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the book icons should be shown</param>
    public void DisplayBooks(bool active)
    {
        areaBuilder.DisplayBooks(active);
    }

    /// <summary>
    ///     This function enables or disables the teleporter icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the teleporter icons should be shown</param>
    public void DisplayTeleporter(bool active)
    {
        areaBuilder.DisplayTeleporter(active);
    }

    /// <summary>
    ///     This function enables or disables the dungeon icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the dungeon icons should be shown</param>
    public void DisplayDungeons(bool active)
    {
        areaBuilder.DisplayDungeons(active);
        areaBuilder.DisplayBarriers(active);
    }

    #endregion
}
