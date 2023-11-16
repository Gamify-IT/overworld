using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the creation and setup of scene transition spot at world loading
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject sceneTransitionSpotPrefab;
    [SerializeField] private GameObject placeholderPrefab;

    [Space(10)]

    [Header("Icons")]
    [SerializeField] private GameObject minimapIcons;

    /// <summary>
    ///     This function sets up scene transition objects for the data given
    /// </summary>
    /// <param name="sceneTransitionSpots">The data needed for the scene transitions</param>
    public void Setup(List<SceneTransitionSpotData> sceneTransitionSpots)
    {
        ClearSceneTransitionSpots();
        MinimapIconManager minimapIconManager = minimapIcons.GetComponent<MinimapIconManager>();
        if (minimapIconManager != null)
        {
            minimapIconManager.ClearMinimapIconsOfType(MinimapIconType.DUNGEON);
        }
        else
        {
            Debug.LogError("Error creating npc minimap icon - MinimapIconManager not found");
        }
        foreach (SceneTransitionSpotData sceneTransitionSpotData in sceneTransitionSpots)
        {
            CreateSceneTransitionSpot(sceneTransitionSpotData);
        }
    }

    /// <summary>
    ///     This function sets up placeholder scene transition objects for the data given
    /// </summary>
    /// <param name="sceneTransitionSpots">The data needed for the scene transitions</param>
    public void SetupPlaceholder(List<SceneTransitionSpotData> sceneTransitionSpots)
    {
        ClearSceneTransitionSpots();
        foreach (SceneTransitionSpotData sceneTransitionSpotData in sceneTransitionSpots)
        {
            CreatePlaceholderSceneTransitionSpot(sceneTransitionSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing scene transition objects
    /// </summary>
    private void ClearSceneTransitionSpots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates an scene transition spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the scene transition spot</param>
    private void CreateSceneTransitionSpot(SceneTransitionSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject sceneTransitionSpot = Instantiate(sceneTransitionSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;

        BoxCollider2D collider = sceneTransitionSpot.GetComponent<BoxCollider2D>();
        if(collider != null)
        {
            collider.size = data.GetSize();
        }

        LoadSubScene sceneTransition = sceneTransitionSpot.GetComponent<LoadSubScene>();
        if (sceneTransition != null)
        {
            sceneTransition.Initialize(data.GetArea(), data.GetAreaToLoad(), data.GetFacingDirection());
        }
        else
        {
            Debug.LogError("Error creating scene transition - script not found");
        }

        MinimapIconManager minimapIconManager = minimapIcons.GetComponent<MinimapIconManager>();
        if (minimapIconManager != null)
        {
            minimapIconManager.AddMinimapIcon(MinimapIconType.DUNGEON, position);
        }
        else
        {
            Debug.LogError("Error creating npc minimap icon - MinimapIconManager not found");
        }
    }

    /// <summary>
    ///     This function creates a placeholder scene transition spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the scene transition spot</param>
    private void CreatePlaceholderSceneTransitionSpot(SceneTransitionSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject placeholderSpot = Instantiate(placeholderPrefab, position, Quaternion.identity, this.transform) as GameObject;
        PlaceholderObject placeholder = placeholderSpot.GetComponent<PlaceholderObject>();
        if (placeholder != null)
        {
            placeholder.Setup(PlaceholderType.SCENE_TRANSITION, data.GetAreaToLoad().GetDungeonIndex());
        }
        else
        {
            Debug.LogError("Error creating placeholder scene transition spot");
        }
    }
}
