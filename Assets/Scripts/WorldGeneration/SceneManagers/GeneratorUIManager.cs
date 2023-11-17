using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
///     This class sets up and manages the <c>Generator UI</c> and forwards all changes to the <c>AreaManager</c>
/// </summary>
public class GeneratorUIManager : MonoBehaviour
{
    [SerializeField] private GameObject generatorUIPrefab;
    [SerializeField] private AreaManager areaManager;

    private CameraMovement cameraController;
    private AreaInformationData areaInformation;

    /// <summary>
    ///     This function instantiates the <c>GeneratorUI</c> and sets all needed values
    /// </summary>
    /// <param name="areaInformation">The information relevant for the UI</param>
    public void SetupUI(AreaData areaData, AreaInformationData areaInformation, CameraMovement cameraController)
    {
        //store infos
        this.areaInformation = areaInformation;
        this.cameraController = cameraController;

        //create and set up UI
        GameObject uiObject = Instantiate(generatorUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        GeneratorUI ui = uiObject.GetComponent<GeneratorUI>();
        ui.Setup(this, areaData, areaInformation);

        SetupCamera();
    }

    /// <summary>
    ///     This function sets up the camera to be in the center of the area
    /// </summary>
    private void SetupCamera()
    {
        Vector2Int size = areaInformation.GetSize();
        Vector2Int offset = areaInformation.GetObjectOffset();
        Vector3 position = new Vector3((size.x / 2) + offset.x, (size.y / 2) + offset.y, cameraController.transform.position.z);
        cameraController.transform.position = position;
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

    /// <summary>
    ///     This function calls the <c>AreaManager</c> to reset the area to the default, manually created one
    /// </summary>
    public async UniTask<bool> ResetToDefault()
    {
        bool success = await areaManager.ResetArea();
        return success;
    }

    /// <summary>
    ///     This function forwards a layout generation request to the <c>AreaManager</c>
    /// </summary>
    /// <param name="size">The size of the layout</param>
    /// <param name="style">The style of the new layout<param>
    /// <param name="accessability">How much area is accessable</param>
    /// <param name="seed"/>The seed to be used</param>
    public void GenerateLayout(Vector2Int size, WorldStyle style, LayoutGeneratorType layoutGeneratorType, int accessability, string seed)
    {
        areaManager.GenerateLayout(size, style, layoutGeneratorType, accessability, seed);
    }

    /// <summary>
    ///     This function resets all objects in the area
    /// </summary>
    public void ResetObjects()
    {
        areaManager.ResetObjects();
    }

    /// <summary>
    ///     This function forwards a minigame generation request to the <c>AreaManager</c>
    /// </summary>
    /// <param name="amount">The amount of minigames to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateMinigames(int amount)
    {
        return areaManager.GenerateMinigames(amount);
    }

    /// <summary>
    ///     This function forwards a npc generation request to the <c>AreaManager</c>
    /// </summary>
    /// <param name="amount">The amount of npcs to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateNpcs(int amount)
    {
        return areaManager.GenerateNPCs(amount);
    }

    /// <summary>
    ///     This function forwards a book generation request to the <c>AreaManager</c>
    /// </summary>
    /// <param name="amount">The amount of books to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateBooks(int amount)
    {
        return areaManager.GenerateBooks(amount);
    }

    /// <summary>
    ///     This function forwards a teleporter generation request to the <c>AreaManager</c>
    /// </summary>
    /// <param name="amount">The amount of teleporters to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateTeleporters(int amount)
    {
        return areaManager.GenerateTeleporters(amount);
    }

    /// <summary>
    ///     This function forwards a dungeons generation request to the <c>AreaManager</c>
    /// </summary>
    /// <param name="amount">The amount of dungeons to generate</param>
    /// <returns>True, if all spots could be created, false otherwise</returns>
    public bool GenerateDungeons(int amount)
    {
        return areaManager.GenerateDungeons(amount);
    }

    /// <summary>
    ///     This function checks, whether the current area can be saved
    /// </summary>
    /// <returns>True, if the area can be saved, false otherwise</returns>
    public bool IsAreaSaveable()
    {
        return areaManager.IsAreaSaveable();
    }

    /// <summary>
    ///     This function forwards a save request to the <c>AreaManager</c>
    /// </summary>
    public async UniTask<bool> SaveArea()
    {
        bool success = await areaManager.SaveArea();
        return success;
    }
}
