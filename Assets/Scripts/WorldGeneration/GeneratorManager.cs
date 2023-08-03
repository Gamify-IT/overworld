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
    [SerializeField] private MinigamesManager minigames;
    [SerializeField] private NpcManager NPCs;
    [SerializeField] private BookManager books;
    [SerializeField] private TeleporterManager teleporters;
    [SerializeField] private SceneTransitionManager sceneTransitions;
    [SerializeField] private BarrierManager barriers;
    [SerializeField] private MinimapIconManager minimapIcons;
    #endregion

    public void Setup()
    {
        SetupUI();
    }

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

    public void CreateLayout(Vector2Int size, Vector2Int offset, WorldStyle style, float accessability, List<WorldConnection> worldConnections)
    {
        AreaGenerator areaGenerator = new AreaGenerator(size, style, accessability, worldConnections);
        areaGenerator.GenerateLayout();
        string[,,] layout = areaGenerator.GetLayout();
        areaPainter.Paint(layout, size, offset);
    }

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
}
