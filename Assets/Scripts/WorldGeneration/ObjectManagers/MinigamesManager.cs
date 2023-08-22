using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the creation and setup of minigames spot at world loading
/// </summary>
public class MinigamesManager : MonoBehaviour
{
    [SerializeField] private GameObject minigameSpotPrefab;

    /// <summary>
    ///     This function sets up minigame objects for the data given
    /// </summary>
    /// <param name="minigameSpots">The data needed for the minigames</param>
    public void Setup(AreaInformation area, List<MinigameSpotData> minigameSpots)
    {
        ClearMinigameSpots(area);
        foreach(MinigameSpotData minigameSpotData in minigameSpots)
        {
            CreateMinigameSpot(minigameSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing minigame objects of the given area
    /// </summary>
    private void ClearMinigameSpots(AreaInformation area)
    {
        foreach(Transform child in transform)
        {
            Minigame minigame = child.GetComponent<Minigame>();
            if(minigame != null)
            {
                int worldIndex = area.GetWorldIndex();
                int dungeonIndex = 0;
                if (area.IsDungeon())
                {
                    dungeonIndex = area.GetDungeonIndex();
                }
                if( !(minigame.GetWorldIndex() == worldIndex && minigame.GetDungeonIndex() == dungeonIndex))
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                Destroy(child.gameObject);
            }
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
            minigame.SetWorldIndex(data.GetArea().GetWorldIndex());
            if(data.GetArea().IsDungeon())
            {
                minigame.SetDungeonIndex(data.GetArea().GetDungeonIndex());                
            }
            else
            {
                minigame.SetDungeonIndex(0);
            }
            minigame.SetIndex(data.GetIndex());
        }
        else
        {
            Debug.LogError("Error creating minigame");
        }        
    }
}
