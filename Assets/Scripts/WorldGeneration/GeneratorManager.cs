using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorManager : MonoBehaviour
{
    #region Attributes
    //Camera
    [SerializeField] private Camera cameraObject;

    //UI
    [SerializeField] private GameObject generatorUIPrefab;

    //Parent Objects
    [SerializeField] private AreaPainter areaPainter;
    [SerializeField] private MinigamesManager minigamesManager;
    [SerializeField] private NpcManager npcManager;
    [SerializeField] private BookManager bookManager;
    [SerializeField] private TeleporterManager teleporterManager;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;
    [SerializeField] private BarrierManager barrierManager;
    [SerializeField] private MinimapIconManager minimapIconsManager;

    //World map data
    private WorldMapData worldMapData;
    private AreaInformation currentArea;
    #endregion

    /// <summary>
    ///     This function sets up everything for the given area
    /// </summary>
    /// <param name="area">The area to set up</param>
    public void Setup(AreaInformation area)
    {
        currentArea = area;
        worldMapData = LoadAreaData();
        SetupArea();
        SetupUI();
    }

    /// <summary>
    ///     This function loads the world map data for the edited data
    /// </summary>
    /// <returns>The <c>WorldMapData</c> of the given world</returns>
    private WorldMapData LoadAreaData()
    {
        //TODO: load world data from backend

        //Workaround: use local json files
        string path;
        if (currentArea.IsDungeon())
        {
            path = "Areas/Dungeon" + currentArea.GetDungeonIndex();
        }
        else
        {
            path = "Areas/World" + currentArea.GetWorldIndex();
        }
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        WorldMapDTO worldMapDTO = WorldMapDTO.CreateFromJSON(json);
        WorldMapData worldMapData = WorldMapData.ConvertDtoToData(worldMapDTO);
        return worldMapData;
    }

    /// <summary>
    ///     This function sets up the area for the data in <c>worldMapData</c>
    /// </summary>
    private void SetupArea()
    {
        areaPainter.Paint(worldMapData.GetTiles(), worldMapData.GetOffset());
        minigamesManager.Setup(worldMapData.GetMinigameSpots());
        npcManager.Setup(worldMapData.GetNpcSpots());
        bookManager.Setup(worldMapData.GetBookSpots());
        barrierManager.Setup(worldMapData.GetBarrierSpots());
        teleporterManager.Setup(worldMapData.GetTeleporterSpots());
        sceneTransitionManager.Setup(worldMapData.GetSceneTransitionSpots());
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
            generatorUI.Setup(this, worldMapData);
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
    public void CreateLayout(Vector2Int size, Vector2Int offset, WorldStyle style, float accessability, Optional<List<WorldConnection>> worldConnections)
    {
        AreaGenerator areaGenerator = new AreaGenerator(size, style, accessability, worldConnections);
        areaGenerator.GenerateLayout();
        string[,,] layout = areaGenerator.GetLayout();
        areaPainter.Paint(layout, offset);

        ClearContent();
        worldMapData.SetTiles(layout);
        worldMapData.SetWorldStyle(style);
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
        this.worldMapData = worldMapData;

        SetupArea();
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
        minigamesManager.Setup(minigameSpots);
        worldMapData.SetMinigameSpots(minigameSpots);
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
        npcManager.Setup(npcSpots);
        worldMapData.SetNpcSpots(npcSpots);
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
        bookManager.Setup(bookSpots);
        worldMapData.SetBookSpots(bookSpots);
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
        teleporterManager.Setup(teleporterSpots);
        worldMapData.SetTeleporterSpots(teleporterSpots);
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
        sceneTransitionManager.Setup(dungeonSpots);
        worldMapData.SetSceneTransitionSpots(dungeonSpots);
    }
    #endregion

    /// <summary>
    ///     This function saves the created world to the backend
    /// </summary>
    public async void SaveArea()
    {
        //TODO: send world map data to backend

        //Workaround: use local json files
        WorldMapDTO worldMapDTO = WorldMapDTO.ConvertDataToDto(worldMapData);
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
        minigamesManager.Setup(new List<MinigameSpotData>());
        worldMapData.SetMinigameSpots(new List<MinigameSpotData>());

        npcManager.Setup(new List<NpcSpotData>());
        worldMapData.SetNpcSpots(new List<NpcSpotData>());

        bookManager.Setup(new List<BookSpotData>());
        worldMapData.SetBookSpots(new List<BookSpotData>());

        barrierManager.Setup(new List<BarrierSpotData>());
        worldMapData.SetBarrierSpots(new List<BarrierSpotData>());

        teleporterManager.Setup(new List<TeleporterSpotData>());
        worldMapData.SetTeleporterSpots(new List<TeleporterSpotData>());

        sceneTransitionManager.Setup(new List<SceneTransitionSpotData>());
        worldMapData.SetSceneTransitionSpots(new List<SceneTransitionSpotData>());
    }

    /// <summary>
    ///     This function returns the current <c>WorldMapData</c> object
    /// </summary>
    /// <returns></returns>
    public WorldMapData GetWorldMapData()
    {
        return worldMapData;
    }
}
