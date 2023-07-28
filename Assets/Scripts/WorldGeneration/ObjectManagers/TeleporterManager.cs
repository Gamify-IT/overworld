using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterManager : MonoBehaviour
{
    [SerializeField] private GameObject teleporterSpotPrefab;

    /// <summary>
    ///     This function sets up teleporter objects for the data given
    /// </summary>
    /// <param name="teleporterSpots">The data needed for the teleporter</param>
    public void Setup(List<TeleporterSpotData> teleporterSpots)
    {
        ClearTeleporterSpots();
        foreach (TeleporterSpotData teleporterSpotData in teleporterSpots)
        {
            CreateTeleporterSpot(teleporterSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing teleporter objects
    /// </summary>
    private void ClearTeleporterSpots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates a teleporter spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the teleporter spot</param>
    private void CreateTeleporterSpot(TeleporterSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject teleporterSpot = Instantiate(teleporterSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;

        Teleporter teleporter = teleporterSpot.GetComponent<Teleporter>();
        if (teleporter != null)
        {
            teleporter.SetWorldIndex(data.GetWorldIndex());
            if (data.IsDungeon())
            {
                teleporter.SetDungeonIndex(data.GetDungeonIndex());
            }
            else
            {
                teleporter.SetDungeonIndex(0);
            }
            teleporter.SetIndex(data.GetIndex());
            teleporter.SetName(data.GetName());
        }
        else
        {
            Debug.LogError("Error creating teleporter - Script not found");
        }
    }
}
