using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorManager : MonoBehaviour
{
    #region Attributes
    //UI
    [SerializeField] private GameObject generatorUIPrefab;
    [SerializeField] private CameraMovement cameraMovement;

    //Parent Objects
    [SerializeField] private AreaPainter areaPainter;
    [SerializeField] private MinigamesManager minigamesManager;
    [SerializeField] private NpcManager npcManager;
    [SerializeField] private BookManager bookManager;
    [SerializeField] private TeleporterManager teleporterManager;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;
    [SerializeField] private BarrierManager barrierManager;
    [SerializeField] private MinimapIconManager minimapIconsManager;
    [SerializeField] private MaskManager maskManager;

    //World map data
    private Dictionary<int, WorldMapData> worldMaps;
    private AreaInformation currentArea;
    #endregion

    /// <summary>
    ///     This function sets up everything for the given area
    /// </summary>
    /// <param name="areaToLoad">The area to set up</param>
    public void Setup(string areaToLoad)
    {
        currentArea = GetAreInformation(areaToLoad);
        worldMaps = LoadAreaData();

        if(currentArea.IsDungeon())
        {
            //TODO: SetupDungeonMap
        }
        else
        {
            SetupWorldMap();
            CreateMask();
        }
        SetupCamera();
        SetupUI();
    }

    /// <summary>
    ///     This function converts a given string to an <c>AreaInformation</c>
    /// </summary>
    /// <param name="area">The string to be converted</param>
    /// <returns>The converted <c>AreaInformation</c>, if valid, the default world 1 otherwise</returns>
    private AreaInformation GetAreInformation(string area)
    {
        Optional<AreaInformation> areaInformation;
        if(area.Contains("-"))
        {
            areaInformation = ConvertDungeon(area);
        }
        else
        {
            areaInformation = ConvertWorld(area);
        }

        if(areaInformation.IsPresent())
        {
            return areaInformation.Value();
        }
        else
        {
            Debug.LogError("Inavlid area specified, loading world 1 instead");
            return new AreaInformation(1, new Optional<int>());
        }        
    }

    /// <summary>
    ///     This function tries to convert a string into a dungeon identifier
    /// </summary>
    /// <param name="area">the string to be converted</param>
    /// <returns>The converted dungeon, if valid, an empty <c>Optional</c> otherwise</returns>
    private Optional<AreaInformation> ConvertDungeon(string area)
    {
        Optional<AreaInformation> optionalAreaInformation = new Optional<AreaInformation>();
        string[] parts = area.Split("-");
        if(parts.Length == 2)
        {
            bool success = int.TryParse(parts[0], out int worldIndex);
            if(success && IsValidWorldIndex(worldIndex))
            {
                AreaInformation areaInformation = new AreaInformation(worldIndex, new Optional<int>());
                success = int.TryParse(parts[1], out int dungeonIndex);
                if (success && IsValidDungeonIndex(worldIndex, dungeonIndex))
                {
                    areaInformation.SetDungeonIndex(dungeonIndex);
                    optionalAreaInformation.SetValue(areaInformation);
                }
            }            
        }
        return optionalAreaInformation;
    }

    /// <summary>
    ///     This function tries to convert a string into a world identifier
    /// </summary>
    /// <param name="area">the string to be converted</param>
    /// <returns>The converted world, if valid, an empty <c>Optional</c> otherwise</returns>
    private Optional<AreaInformation> ConvertWorld(string area)
    {
        Optional<AreaInformation> optionalAreaInformation = new Optional<AreaInformation>();
        bool success = int.TryParse(area, out int worldIndex);
        if (success && IsValidWorldIndex(worldIndex))
        {
            AreaInformation areaInformation = new AreaInformation(worldIndex, new Optional<int>());
            optionalAreaInformation.SetValue(areaInformation);
        }
        return optionalAreaInformation;
    }

    /// <summary>
    ///     This function checks, whether a given world index is valid
    /// </summary>
    /// <param name="worldIndex">the world index to be checked</param>
    /// <returns>true, if given index is valid, false otherwise</returns>
    private bool IsValidWorldIndex(int worldIndex)
    {
        return (worldIndex > 0 && worldIndex <= GameSettings.GetMaxWorlds());
    }

    /// <summary>
    ///     This function checks, whether a given dungeon index is valid
    /// </summary>
    /// <param name="worldIndex">the world index to be checked</param>
    /// <param name="dungeonIndex">the world index to be checked</param>
    /// <returns>true, if given index is valid, false otherwise</returns>
    private bool IsValidDungeonIndex(int worldIndex, int dungeonIndex)
    {
        //TODO: world has enough dungeons
        return dungeonIndex > 0;
    }

    /// <summary>
    ///     This function retrieves all area data from the backend
    /// </summary>
    private Dictionary<int, WorldMapData> LoadAreaData()
    {
        Dictionary<int, WorldMapData> worldMaps = new Dictionary<int, WorldMapData>();

        WorldMapDTO[] worldMapDTOs = GetAllWorldMaps();
        for(int i = 0; i < worldMapDTOs.Length; i++)
        {
            WorldMapData worldMapData = WorldMapData.ConvertDtoToData(worldMapDTOs[i]);
            worldMaps.Add(worldMapData.GetArea().GetWorldIndex(), worldMapData);
        }

        return worldMaps;
    }

    /// <summary>
    ///     This function retrieves all world maps from the backend
    /// </summary>
    /// <returns>An array containing all world maps</returns>
    private WorldMapDTO[] GetAllWorldMaps()
    {
        //TODO: load data from backend

        //Workaround: use local json files
        WorldMapDTO[] worldMapDTOs = new WorldMapDTO[4];
        for(int i = 0; i < 4; i++)
        {
            string path = "Areas/World" + (i+1);
            TextAsset targetFile = Resources.Load<TextAsset>(path);
            string json = targetFile.text;
            WorldMapDTO worldMapDTO = WorldMapDTO.CreateFromJSON(json);
            worldMapDTOs[i] = worldMapDTO;
        }
        return worldMapDTOs;
    }

    /// <summary>
    ///     This function sets up the world map
    /// </summary>
    private void SetupWorldMap()
    {
        foreach(KeyValuePair<int, WorldMapData> entry in worldMaps)
        {
            areaPainter.Paint(entry.Value.GetTiles(), entry.Value.GetOffset());
            minigamesManager.Setup(currentArea, entry.Value.GetMinigameSpots());
            npcManager.Setup(currentArea, entry.Value.GetNpcSpots());
            bookManager.Setup(currentArea, entry.Value.GetBookSpots());
            barrierManager.Setup(entry.Value.GetBarrierSpots());
            teleporterManager.Setup(currentArea, entry.Value.GetTeleporterSpots());
            sceneTransitionManager.Setup(currentArea, entry.Value.GetSceneTransitionSpots());
        }
    }
    
    /// <summary>
    ///     This function mask all worlds except the active one
    /// </summary>
    private void CreateMask()
    {
        List<WorldMapData> worlds = new List<WorldMapData>();

        foreach (KeyValuePair<int, WorldMapData> entry in worldMaps)
        {
            worlds.Add(entry.Value);
        }

        maskManager.Setup(worlds);
        maskManager.DeactivateMask(currentArea);
    }

    /// <summary>
    ///     This function moves to camera to the middle of the current world
    /// </summary>
    private void SetupCamera()
    {
        WorldMapData worldMapData = worldMaps[currentArea.GetWorldIndex()];
        Vector2Int size = new Vector2Int(worldMapData.GetTiles().GetLength(0), worldMapData.GetTiles().GetLength(1));
        Vector2Int offset = worldMapData.GetOffset();
        Vector3 position = new Vector3((size.x / 2) + offset.x, (size.y / 2) + offset.y, cameraMovement.transform.position.z);
        cameraMovement.transform.position = position;
    }

    /// <summary>
    ///     This function sets up the generator UI panel
    /// </summary>
    private void SetupUI()
    {
        GameObject uiObject = (GameObject)Instantiate(generatorUIPrefab) as GameObject;
        GeneratorUI generatorUI = uiObject.GetComponent<GeneratorUI>();
        if(generatorUI != null)
        {
            generatorUI.Setup(this, worldMaps[currentArea.GetWorldIndex()]);
        }
    }

    #region Area Settings
    /// <summary>
    ///     This function creates a layout for the given parameters
    /// </summary>
    /// <param name="size">The size of the area map</param>
    /// <param name="offset">The offset of the area map</param>
    /// <param name="style">The style of the area map</param>
    /// <param name="accessability">The amount of accessabile space</param>
    /// <param name="worldConnections">A list of connection points to other worlds, if the area is a world, empty optional otherwise</param>
    public void CreateLayout(Vector2Int size, WorldMapData worldMapData, float accessability)
    {
        Vector2Int offset = worldMapData.GetOffset();
        WorldStyle style = worldMapData.GetWorldStyle();
        List<WorldConnection> worldConnections = worldMapData.GetWorldConnections();
        AreaGenerator areaGenerator = new AreaGenerator(size, style, accessability, worldConnections);
        areaGenerator.GenerateLayout();
        string[,,] layout = areaGenerator.GetLayout();
        areaPainter.Paint(layout, offset);
        worldMapData.SetTiles(layout);

        ClearContent();
        worldMaps[currentArea.GetWorldIndex()] = worldMapData;
        worldMaps[currentArea.GetWorldIndex()].SetTiles(layout);
        worldMaps[currentArea.GetWorldIndex()].SetWorldStyle(style);
        worldMaps[currentArea.GetWorldIndex()].SetOffset(offset);
        worldMaps[currentArea.GetWorldIndex()].SetWorldConnections(worldConnections);      
    }

    /// <summary>
    ///     This function resets the area to the default, manually created one
    /// </summary>
    public void ResetToCustom()
    {
        string path;
        if (currentArea.IsDungeon())
        {
            path = "Areas/DefaultDungeon";
        }
        else
        {
            path = "Areas/DefaultWorld" + currentArea.GetWorldIndex();
        }

        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        WorldMapDTO worldMapDTO = WorldMapDTO.CreateFromJSON(json);
        WorldMapData worldMapData = WorldMapData.ConvertDtoToData(worldMapDTO);
        worldMaps[currentArea.GetWorldIndex()] = worldMapData;

        SetupWorldMap();
    }
    #endregion

    #region Content
    /// <summary>
    ///     This function generates minigame spots
    /// </summary>
    /// <param name="amount">The amount of minigame spots to be generated</param>
    /// <param name="area">The area the minigame spots are part of</param>
    /// <param name="offset">The offset of the area</param>
    public void GenerateMinigames(int amount, AreaInformation area, Vector2Int offset)
    {
        List<MinigameSpotData> minigameSpots = new List<MinigameSpotData>();
        for(int i=0; i<amount; i++)
        {
            Vector2 position = new Vector2(offset.x + 5 * i, offset.y);
            MinigameSpotData data = new MinigameSpotData(area, i+1, position);
            minigameSpots.Add(data);
        }
        minigamesManager.Setup(area, minigameSpots);
        worldMaps[currentArea.GetWorldIndex()].SetMinigameSpots(minigameSpots);
    }

    /// <summary>
    ///     This function generates npc spots
    /// </summary>
    /// <param name="amount">The amount of npc spots to be generated</param>
    /// <param name="area">The area the npc spots are part of</param>
    /// <param name="offset">The offset of the area</param>
    public void GenerateNPCs(int amount, AreaInformation area, Vector2Int offset)
    {
        List<NpcSpotData> npcSpots = new List<NpcSpotData>();
        for (int i = 0; i < amount; i++)
        {
            Vector2 position = new Vector2(offset.x + 5 * i, offset.y + 10);
            NpcSpotData data = new NpcSpotData(area, i + 1, position, "", "NPCHeads_0", "npc_0");
            npcSpots.Add(data);
        }
        npcManager.Setup(currentArea, npcSpots);
        worldMaps[currentArea.GetWorldIndex()].SetNpcSpots(npcSpots);
    }

    /// <summary>
    ///     This function generates book spots
    /// </summary>
    /// <param name="amount">The amount of book spots to be generated</param>
    /// <param name="area">The area the book spots are part of</param>
    /// <param name="offset">The offset of the area</param>
    public void GenerateBooks(int amount, AreaInformation area, Vector2Int offset)
    {
        List<BookSpotData> bookSpots = new List<BookSpotData>();
        for (int i = 0; i < amount; i++)
        {
            Vector2 position = new Vector2(offset.x + 5 * i, offset.y + 20);
            BookSpotData data = new BookSpotData(area, i + 1, position, "");
            bookSpots.Add(data);
        }
        bookManager.Setup(currentArea, bookSpots);
        worldMaps[currentArea.GetWorldIndex()].SetBookSpots(bookSpots);
    }

    /// <summary>
    ///     This function generates teleporter spots
    /// </summary>
    /// <param name="amount">The amount of teleporter spots to be generated</param>
    /// <param name="area">The area the teleporter spots are part of</param>
    /// <param name="offset">The offset of the area</param>
    public void GenerateTeleporter(int amount, AreaInformation area, Vector2Int offset)
    {
        List<TeleporterSpotData> teleporterSpots = new List<TeleporterSpotData>();
        for (int i = 0; i < amount; i++)
        {
            Vector2 position = new Vector2(offset.x + 5 * i, offset.y + 30);
            TeleporterSpotData data = new TeleporterSpotData(area, i + 1, position, "");
            teleporterSpots.Add(data);
        }
        teleporterManager.Setup(currentArea, teleporterSpots);
        worldMaps[currentArea.GetWorldIndex()].SetTeleporterSpots(teleporterSpots);
    }

    /// <summary>
    ///     This function generates dungeon spots
    /// </summary>
    /// <param name="amount">The amount of dungeon spots to be generated</param>
    /// <param name="area">The area the dungeon spots are part of</param>
    /// <param name="offset">The offset of the area</param>
    public void GenerateDungeons(int amount, AreaInformation area, Vector2Int offset)
    {
        List<SceneTransitionSpotData> dungeonSpots = new List<SceneTransitionSpotData>();
        for (int i = 0; i < amount; i++)
        {
            Vector2 position = new Vector2(offset.x + 5 * i, offset.y + 40);
            SceneTransitionSpotData data = new SceneTransitionSpotData(area, position, new Vector2(1,1), "", new AreaInformation(1, new Optional<int>()), new Vector2(1,1), FacingDirection.south);
            dungeonSpots.Add(data);
        }
        sceneTransitionManager.Setup(currentArea, dungeonSpots);
        worldMaps[currentArea.GetWorldIndex()].SetSceneTransitionSpots(dungeonSpots);
    }
    #endregion

    /// <summary>
    ///     This function saves the created world to the backend
    /// </summary>
    public async void SaveArea()
    {
        //TODO: send world map data to backend

        //Workaround: use local json files
        WorldMapDTO worldMapDTO = WorldMapDTO.ConvertDataToDto(worldMaps[currentArea.GetWorldIndex()]);
        string json = JsonUtility.ToJson(worldMapDTO, true);
        string path;
        if(currentArea.IsDungeon())
        {
            path = "Assets/Resources/Areas/Dungeon" + currentArea.GetDungeonIndex() + ".json";
        }
        else
        {
            path = "Assets/Resources/Areas/World" + currentArea.GetWorldIndex() + ".json";
        }
        WriteToJsonFile(json, path);
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
        UnityEditor.AssetDatabase.Refresh();
    }

    /// <summary>
    ///     This function removes all created objects in the world
    /// </summary>
    private void ClearContent()
    {
        minigamesManager.Setup(currentArea, new List<MinigameSpotData>());
        worldMaps[currentArea.GetWorldIndex()].SetMinigameSpots(new List<MinigameSpotData>());

        npcManager.Setup(currentArea, new List<NpcSpotData>());
        worldMaps[currentArea.GetWorldIndex()].SetNpcSpots(new List<NpcSpotData>());

        bookManager.Setup(currentArea, new List<BookSpotData>());
        worldMaps[currentArea.GetWorldIndex()].SetBookSpots(new List<BookSpotData>());

        barrierManager.Setup(new List<BarrierSpotData>());
        worldMaps[currentArea.GetWorldIndex()].SetBarrierSpots(new List<BarrierSpotData>());

        teleporterManager.Setup(currentArea, new List<TeleporterSpotData>());
        worldMaps[currentArea.GetWorldIndex()].SetTeleporterSpots(new List<TeleporterSpotData>());

        sceneTransitionManager.Setup(currentArea, new List<SceneTransitionSpotData>());
        worldMaps[currentArea.GetWorldIndex()].SetSceneTransitionSpots(new List<SceneTransitionSpotData>());
    }

    /// <summary>
    ///     This function returns the current <c>WorldMapData</c> object
    /// </summary>
    /// <returns></returns>
    public WorldMapData GetWorldMapData()
    {
        return worldMaps[currentArea.GetWorldIndex()];
    }

    /// <summary>
    ///     This function enables camera movement
    /// </summary>
    public void ActivateCameraMovement()
    {
        cameraMovement.Activate();
    }

    /// <summary>
    ///     This function disables camera movement
    /// </summary>
    public void DeactivateCameraMovement()
    {
        cameraMovement.Deactivate();
    }
}
