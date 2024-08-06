using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class handles the updates when the player enters a new world
/// </summary>
public class UpdateArea : MonoBehaviour
{
    [SerializeField] private LoadMaps loadMapsTrigger;
    private int destinationWorldIndex;

    private void Start()
    {
        destinationWorldIndex = loadMapsTrigger.GetSceneDestinationIndex();
    }

    /// <summary>
    ///     Updates the current area data 
    /// </summary>
    /// <param name="playerCollision">player entering the new world</param>
    private void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.tag == "Player")
        {
            GameManager.Instance.SetPlayerPosition(destinationWorldIndex, 0);
            GameManager.Instance.SetData(destinationWorldIndex, 0);
            Debug.Log("Update new world: " + destinationWorldIndex);
        }
    }

}
