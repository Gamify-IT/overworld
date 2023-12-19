using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the inspector ui and forwards actions to the <c>AreaBuilder</c>
/// </summary>
public class InspectorUIManager : MonoBehaviour
{
    #region Attributes
    [SerializeField] private GameObject inspectorUIPrefab;
    [SerializeField] private AreaBuilder areaBuilder;

    private CameraMovement cameraController;
    private CustomAreaMapData areaMapData;
    private CustomAreaMapData displayedMapData;
    #endregion

    /// <summary>
    ///     This function sets up the initial values of the inspector ui manager
    /// </summary>
    /// <param name="areaMapData">The area map data of the current area</param>
    /// <param name="areaInformation">The information required to display the area data correctly</param>
    /// <param name="cameraController">The camera</param>
    /// <param name="areaIdentifier">The index of the area</param>
    public void Setup(CustomAreaMapData areaMapData, AreaInformationData areaInformation, CameraMovement cameraController, AreaInformation areaIdentifier)
    {
        this.areaMapData = areaMapData;
        this.cameraController = cameraController;
        SetupDisplayAreaMap();

        //setup objects
        UpdateObjects();

        //create and set up UI
        GameObject uiObject = Instantiate(inspectorUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        InspectorUI ui = uiObject.GetComponent<InspectorUI>();
        ui.Setup(this, areaIdentifier);

        SetupCamera(areaInformation);
    }

    /// <summary>
    ///     This function initializes the displayed area map data
    /// </summary>
    private void SetupDisplayAreaMap()
    {
        displayedMapData = new CustomAreaMapData();
        displayedMapData.SetMinigameSpots(areaMapData.GetMinigameSpots());
        displayedMapData.SetNpcSpots(areaMapData.GetNpcSpots());
        displayedMapData.SetBookSpots(areaMapData.GetBookSpots());
        displayedMapData.SetTeleporterSpots(areaMapData.GetTeleporterSpots());
        displayedMapData.SetSceneTransitionSpots(areaMapData.GetSceneTransitionSpots());
        displayedMapData.SetBarrierSpots(areaMapData.GetBarrierSpots());
    }

    /// <summary>
    ///     This function sets up the camera to be in the center of the area
    /// </summary>
    private void SetupCamera(AreaInformationData areaInformation)
    {
        Vector2Int size = areaInformation.GetSize();
        Vector2Int offset = areaInformation.GetObjectOffset();
        Vector3 position = new Vector3((size.x / 2) + offset.x, (size.y / 2) + offset.y, cameraController.transform.position.z);
        cameraController.transform.position = position;
    }

    /// <summary>
    ///     This function updates the displayed objects
    /// </summary>
    private void UpdateObjects()
    {
        areaBuilder.SetupPlaceholderObjects(displayedMapData);
    }

    /// <summary>
    ///     This function enables or disables the minigame placeholder icons
    /// </summary>
    /// <param name="active">The state of the icons</param>
    public void SetMinigames(bool active)
    {
        if(active)
        {
            displayedMapData.SetMinigameSpots(areaMapData.GetMinigameSpots());
        }
        else
        {
            displayedMapData.SetMinigameSpots(new List<MinigameSpotData>());
        }

        UpdateObjects();
    }

    /// <summary>
    ///     This function enables or disables the npc placeholder icons
    /// </summary>
    /// <param name="active">The state of the icons</param>
    public void SetNpcs(bool active)
    {
        if (active)
        {
            displayedMapData.SetNpcSpots(areaMapData.GetNpcSpots());
        }
        else
        {
            displayedMapData.SetNpcSpots(new List<NpcSpotData>());
        }

        UpdateObjects();
    }

    /// <summary>
    ///     This function enables or disables the book placeholder icons
    /// </summary>
    /// <param name="active">The state of the icons</param>
    public void SetBooks(bool active)
    {
        if (active)
        {
            displayedMapData.SetBookSpots(areaMapData.GetBookSpots());
        }
        else
        {
            displayedMapData.SetBookSpots(new List<BookSpotData>());
        }

        UpdateObjects();
    }

    /// <summary>
    ///     This function enables or disables the teleporter placeholder icons
    /// </summary>
    /// <param name="active">The state of the icons</param>
    public void SetTeleporters(bool active)
    {
        if (active)
        {
            displayedMapData.SetTeleporterSpots(areaMapData.GetTeleporterSpots());
        }
        else
        {
            displayedMapData.SetTeleporterSpots(new List<TeleporterSpotData>());
        }

        UpdateObjects();
    }

    /// <summary>
    ///     This function enables or disables the dungeon placeholder icons
    /// </summary>
    /// <param name="active">The state of the icons</param>
    public void SetDungeons(bool active)
    {
        if (active)
        {
            displayedMapData.SetSceneTransitionSpots(areaMapData.GetSceneTransitionSpots());
        }
        else
        {
            displayedMapData.SetSceneTransitionSpots(new List<SceneTransitionSpotData>());
        }

        UpdateObjects();
    }

    /// <summary>
    ///     This function enables or disables the barrier placeholder icons
    /// </summary>
    /// <param name="active">The state of the icons</param>
    public void SetBarriers(bool active)
    {
        if (active)
        {
            displayedMapData.SetBarrierSpots(areaMapData.GetBarrierSpots());
        }
        else
        {
            displayedMapData.SetBarrierSpots(new List<BarrierSpotData>());
        }

        UpdateObjects();
    }

    #region CameraControl
    /// <summary>
    ///     This function enables camera movement
    /// </summary>
    public void ActivateCameraMovement()
    {
        cameraController.Activate();
    }

    /// <summary>
    ///     This function disables camera movement
    /// </summary>
    public void DeactivateCameraMovement()
    {
        cameraController.Deactivate();
    }
    #endregion
}
