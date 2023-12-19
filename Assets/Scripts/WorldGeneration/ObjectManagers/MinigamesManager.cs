using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This classes manages the creation of minigame objects and placeholder icons
/// </summary>
public class MinigamesManager : MonoBehaviour
{
    [SerializeField] private GameObject minigameSpotPrefab;
    [SerializeField] private GameObject placeholderPrefab;

    /// <summary>
    ///     This function sets up minigame objects for the data given
    /// </summary>
    /// <param name="minigameSpots">The data needed for the minigames</param>
    public void Setup(List<MinigameSpotData> minigameSpots)
    {
        ClearMinigameSpots();
        foreach(MinigameSpotData minigameSpotData in minigameSpots)
        {
            CreateMinigameSpot(minigameSpotData);
        }
    }

    /// <summary>
    ///     This function sets up placeholder minigame objects for the data given
    /// </summary>
    /// <param name="minigameSpots">The data needed for the minigames</param>
    public void SetupPlaceholder(List<MinigameSpotData> minigameSpots)
    {
        ClearMinigameSpots();
        foreach (MinigameSpotData minigameSpotData in minigameSpots)
        {
            CreatePlaceholderMinigameSpot(minigameSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing minigame objects of the given area
    /// </summary>
    private void ClearMinigameSpots()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates an minigame spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the minigame spot</param>
    private void CreateMinigameSpot(MinigameSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject minigameSpot = Instantiate(minigameSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;
        Minigame minigame = minigameSpot.GetComponent<Minigame>();
        if(minigame != null)
        {
            minigame.Initialize(data.GetArea(), data.GetIndex());
        }
        else
        {
            Debug.LogError("Error creating minigame");
        }        
    }

    /// <summary>
    ///     This function creates a placeholder minigame spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the minigame spot</param>
    private void CreatePlaceholderMinigameSpot(MinigameSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject placeholderSpot = Instantiate(placeholderPrefab, position, Quaternion.identity, this.transform) as GameObject;
        PlaceholderObject placeholder = placeholderSpot.GetComponent<PlaceholderObject>();
        if (placeholder != null)
        {
            placeholder.Setup(PlaceholderType.MINIGAME, data.GetIndex());
        }
        else
        {
            Debug.LogError("Error creating placeholder minigame spot");
        }
    }
}
