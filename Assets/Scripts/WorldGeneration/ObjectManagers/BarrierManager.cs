using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the creation and setup of barrier spot at world loading
/// </summary>
public class BarrierManager : MonoBehaviour
{
    [SerializeField] private GameObject barrierSpotPrefab;

    /// <summary>
    ///     This function sets up barrier objects for the data given
    /// </summary>
    /// <param name="barrierSpots">The data needed for the barriers</param>
    public void Setup(List<BarrierSpotData> barrierSpots)
    {
        ClearBarrierSpots();
        foreach (BarrierSpotData barrierSpotData in barrierSpots)
        {
            CreateBarrierSpot(barrierSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing barrier objects
    /// </summary>
    private void ClearBarrierSpots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates an barrier spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the barrier spot</param>
    private void CreateBarrierSpot(BarrierSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject barrierSpot = Instantiate(barrierSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;
        Barrier barrier = barrierSpot.GetComponent<Barrier>();
        if (barrier != null)
        {
            barrier.SetWorldOriginIndex(data.GetArea().GetWorldIndex());
            barrier.SetDestionationAreaIndex(data.GetDestinationAreaIndex());
            barrier.SetBarrierType(data.GetBarrierType());
        }
        else
        {
            Debug.LogError("Error creating barrier - script not found");
        }
    }
}