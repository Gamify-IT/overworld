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
    private GeneratorUI ui;
    private CameraMovement cameraMovement;    

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
    private AreaData areaData;
    private AreaInformation currentArea;
    private AreaInformationData areaInformation;
    #endregion

    /// <summary>
    ///     This function sets up everything for the given area
    /// </summary>
    /// <param name="areaToLoad">The area to set up</param>
    public void Setup(AreaData areaData, CameraMovement camera)
    {
        this.areaData = areaData;
        currentArea = areaData.GetArea();
        cameraMovement = camera;
        areaInformation = GetAreaInformation();

        if(areaData.IsGeneratedArea())
        {
            SetupGeneratedArea();
        }

        SetupCamera();
        SetupUI();
    }

    /// <summary>
    ///     This function reads the information needed for area generation
    /// </summary>
    /// <returns></returns>
    private AreaInformationData GetAreaInformation()
    {
        string path;
        if (currentArea.IsDungeon())
        {
            path = "AreaInfo/Dungeon" + currentArea.GetWorldIndex() + "-" + currentArea.GetWorldIndex();
        }
        else
        {
            path = "AreaInfo/World" + currentArea.GetWorldIndex();
        }
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AreaInformationDTO areaInformationDTO = AreaInformationDTO.CreateFromJSON(json);
        AreaInformationData areaInformationData = AreaInformationData.ConvertDtoToData(areaInformationDTO);
        return areaInformationData;
    }

    /// <summary>
    ///     This function recreated the generated area
    /// </summary>
    private void SetupGeneratedArea()
    {
        Vector2Int offset = areaInformation.GetOffset();
        CustomAreaMapData areaMap = areaData.GetAreaMapData();

        string[,,] layout = areaMap.GetTiles();
        areaPainter.Paint(layout, offset);

        minigamesManager.Setup(areaMap.GetMinigameSpots());
        npcManager.Setup(areaMap.GetNpcSpots());
        bookManager.Setup(areaMap.GetBookSpots());
        teleporterManager.Setup(areaMap.GetTeleporterSpots());
        sceneTransitionManager.Setup(areaMap.GetSceneTransitionSpots());
        barrierManager.Setup(areaMap.GetBarrierSpots());
    }

    /// <summary>
    ///     This function moves to camera to the middle of the current world
    /// </summary>
    private void SetupCamera()
    {
        Vector2Int size = areaInformation.GetSize();
        Vector2Int offset = areaInformation.GetOffset();
        Vector3 position = new Vector3((size.x / 2) + offset.x, (size.y / 2) + offset.y, cameraMovement.transform.position.z);
        cameraMovement.transform.position = position;
    }

    /// <summary>
    ///     This function sets up the generator UI panel
    /// </summary>
    private void SetupUI()
    {
        GameObject uiObject = Instantiate(generatorUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        ui = uiObject.GetComponent<GeneratorUI>();
        ui.Setup(this, areaData, areaInformation);
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
    public void CreateLayout(Vector2Int size, CustomAreaMapData areaMapData, float accessability)
    {        
        WorldStyle style = areaMapData.GetWorldStyle();
        List<WorldConnection> worldConnections = areaInformation.GetWorldConnections();
        AreaGenerator areaGenerator = new AreaGenerator(size, style, accessability, worldConnections);
        areaGenerator.GenerateLayout();
        string[,,] layout = areaGenerator.GetLayout();
        
        CustomAreaMapData areaMap = new CustomAreaMapData(layout, style);
        areaData.SetAreaMapData(areaMap);

        ClearContent();
        Vector2Int offset = areaInformation.GetOffset();
        areaPainter.Paint(layout, offset);
    }

    /// <summary>
    ///     This function resets the area to the default, manually created one
    /// </summary>
    public void ResetToCustom()
    {
        areaData.Reset();
        SaveArea();
        //TODO: trigger reload of scene
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
        CustomAreaMapData areaMapData = areaData.GetAreaMapData();
        areaMapData.SetMinigameSpots(minigameSpots);

        minigamesManager.Setup(minigameSpots);
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
        CustomAreaMapData areaMapData = areaData.GetAreaMapData();
        areaMapData.SetNpcSpots(npcSpots);

        npcManager.Setup(npcSpots);
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
        CustomAreaMapData areaMapData = areaData.GetAreaMapData();
        areaMapData.SetBookSpots(bookSpots);

        bookManager.Setup(bookSpots);
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
        CustomAreaMapData areaMapData = areaData.GetAreaMapData();
        areaMapData.SetTeleporterSpots(teleporterSpots);

        teleporterManager.Setup(teleporterSpots);
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
        CustomAreaMapData areaMapData = areaData.GetAreaMapData();
        areaMapData.SetSceneTransitionSpots(dungeonSpots);

        sceneTransitionManager.Setup(dungeonSpots);
    }
    #endregion

    /// <summary>
    ///     This function saves the created world to the backend
    /// </summary>
    public async void SaveArea()
    {
        //TODO: send world map data to backend

        //Workaround: use local json files
        AreaDTO areaDTO = AreaDTO.ConvertDataToDto(areaData);
        string json = JsonUtility.ToJson(areaDTO, true);
        string path;
        if(currentArea.IsDungeon())
        {
            path = "Assets/Resources/Areas/Dungeon" + currentArea.GetWorldIndex() + "-" + currentArea.GetDungeonIndex() + ".json";
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

        npcManager.Setup(new List<NpcSpotData>());

        bookManager.Setup(new List<BookSpotData>());

        barrierManager.Setup(new List<BarrierSpotData>());

        teleporterManager.Setup(new List<TeleporterSpotData>());

        sceneTransitionManager.Setup(new List<SceneTransitionSpotData>());
    }

    /// <summary>
    ///     This function returns the current <c>AreaData</c> object
    /// </summary>
    /// <returns></returns>
    public AreaData GetAreaData()
    {
        return areaData;
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
