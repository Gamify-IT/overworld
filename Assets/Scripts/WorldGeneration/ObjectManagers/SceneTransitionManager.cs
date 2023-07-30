using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the creation and setup of scene transition spot at world loading
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject sceneTransitionSpotPrefab;

    /// <summary>
    ///     This function sets up scene transition objects for the data given
    /// </summary>
    /// <param name="sceneTransitionSpots">The data needed for the scene transitions</param>
    public void Setup(List<SceneTransitionSpotData> sceneTransitionSpots)
    {
        ClearSceneTransitionSpots();
        foreach (SceneTransitionSpotData sceneTransitionSpotData in sceneTransitionSpots)
        {
            CreateSceneTransitionSpot(sceneTransitionSpotData);
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
            sceneTransition.worldIndex = data.GetAreaToLoad().GetWorldIndex();
            if (data.GetAreaToLoad().IsDungeon())
            {
                sceneTransition.dungeonIndex = data.GetAreaToLoad().GetDungeonIndex();
            }
            else
            {
                sceneTransition.dungeonIndex = 0;
            }
            sceneTransition.sceneToLoad = data.GetSceneToLoad();
            sceneTransition.playerPosition = data.GetPlayerPosition();
            sceneTransition.facingDirection = data.GetFacingDirection();
        }
        else
        {
            Debug.LogError("Error creating scene transition - script not found");
        }
    }
}
