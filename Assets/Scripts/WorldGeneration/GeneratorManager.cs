using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorManager : MonoBehaviour
{
    #region Attributes
    //Camera
    [SerializeField] private Camera Camera;

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
    #endregion

    public void Setup()
    {
        SetupUI();
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
            Vector2Int size = new Vector2Int(200, 118);
            Vector2Int offset = new Vector2Int(-69, -20);
            WorldStyle style = WorldStyle.CAVE;
            float accessability = 0.75f;
            List<WorldConnection> worldConnections = new List<WorldConnection>();
            generatorUI.Setup(this, size, offset, style, accessability, worldConnections);
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
        areaPainter.Paint(layout, size, offset);
    }

    /// <summary>
    ///     This function resets the area to the default, manually created one
    /// </summary>
    public void ResetToCustom()
    {
        string[,,] layout = new string[200,118,5];
        for(int i=0; i<200; i++)
        {
            for(int j=0; j<118; j++)
            {
                layout[i, j, 0] = "Overworld-Savanna_0";
                for (int k=1; k<5; k++)
                {
                    layout[i, j, k] = "none";
                }
            }
        }

        Vector2Int size = new Vector2Int(200, 118);
        Vector2Int offset = new Vector2Int(-69, -20);

        areaPainter.Paint(layout, size, offset);
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
            Vector2 position = new Vector2(offset.x + 2*i, offset.y);
            MinigameSpotData data = new MinigameSpotData(area, i+1, position);
            minigameSpots.Add(data);
        }
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
        npcManager.Setup(npcSpots);
    }
    #endregion
}
