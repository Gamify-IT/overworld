using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class creates the layout and all objects in the area
/// </summary>
public class AreaBuilder : MonoBehaviour
{
    //Parent Objects
    [SerializeField] private AreaPainter areaPainter;
    [SerializeField] private MinigamesManager minigamesManager;
    [SerializeField] private NpcManager npcManager;
    [SerializeField] private BookManager bookManager;
    [SerializeField] private TeleporterManager teleporterManager;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;
    [SerializeField] private BarrierManager barrierManager;
    [SerializeField] private MinimapIconManager minimapIconsManager;
    [SerializeField] private List<GameObject> additionalObjects;

    /// <summary>
    ///     This function creates the layout
    /// </summary>
    /// <param name="layout">The layout to set</param>
    /// <param name="areaInformation">The generic information about the area</param>
    public void SetupAreaLayout(TileSprite[,,] layout, AreaInformationData areaInformation)
    {
        Debug.Log("Setup layout");
        areaPainter.Paint(layout, areaInformation.GetGridOffset());
    }

    /// <summary>
    ///     This function creates all objects for play mode
    /// </summary>
    /// <param name="areaData">The data to set up</param>
    public void SetupAreaObjects(CustomAreaMapData areaData)
    {       
        minigamesManager.Setup(areaData.GetMinigameSpots());
        npcManager.Setup(areaData.GetNpcSpots());
        bookManager.Setup(areaData.GetBookSpots());
        teleporterManager.Setup(areaData.GetTeleporterSpots());
        sceneTransitionManager.Setup(areaData.GetSceneTransitionSpots());
        barrierManager.Setup(areaData.GetBarrierSpots());
    }

    /// <summary>
    ///     This function creates placeholder objects for the generator and inspector mode
    /// </summary>
    /// <param name="areaData">The data to set up</param>
    public void SetupPlaceholderObjects(CustomAreaMapData areaData)
    {
        minigamesManager.SetupPlaceholder(areaData.GetMinigameSpots());
        npcManager.SetupPlaceholder(areaData.GetNpcSpots());
        bookManager.SetupPlaceholder(areaData.GetBookSpots());
        teleporterManager.SetupPlaceholder(areaData.GetTeleporterSpots());
        sceneTransitionManager.SetupPlaceholder(areaData.GetSceneTransitionSpots());
        barrierManager.SetupPlaceholder(areaData.GetBarrierSpots());
    }

    /// <summary>
    ///     This function enables or disables the minigame icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the minigame icons should be shown</param>
    public void DisplayMinigames(bool active)
    {
        minigamesManager.gameObject.SetActive(active);
    }

    /// <summary>
    ///     This function enables or disables the npc icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the npc icons should be shown</param>
    public void DisplayNpcs(bool active)
    {
        npcManager.gameObject.SetActive(active);
    }

    /// <summary>
    ///     This function enables or disables the book icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the book icons should be shown</param>
    public void DisplayBooks(bool active)
    {
        bookManager.gameObject.SetActive(active);
    }

    /// <summary>
    ///     This function enables or disables the teleporter icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the teleporter icons should be shown</param>
    public void DisplayTeleporter(bool active)
    {
        teleporterManager.gameObject.SetActive(active);
    }

    /// <summary>
    ///     This function enables or disables the dungeon icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the dungeon icons should be shown</param>
    public void DisplayDungeons(bool active)
    {
        sceneTransitionManager.gameObject.SetActive(active);
    }

    /// <summary>
    ///     This function enables or disables the barrier icons, based on the given value
    /// </summary>
    /// <param name="active">Whether or not the barrier icons should be shown</param>
    public void DisplayBarriers(bool active)
    {
        barrierManager.gameObject.SetActive(active);
    }

    /// <summary>
    ///     This function removes the additional objects
    /// </summary>
    public void RemoveAdditionalObjects()
    {
        foreach(GameObject gameObject in additionalObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
