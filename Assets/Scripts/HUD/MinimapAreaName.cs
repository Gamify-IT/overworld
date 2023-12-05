using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class sets up the name of the area on the minimap
/// </summary>
public class MinimapAreaName : MonoBehaviour
{
    public string areaName;
    public bool dungeonArea;

    /// <summary>
    /// This function sets the name of the area on the minimap
    /// </summary>
    /// <param name="other">The object entering the trigger</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(dungeonArea)
            {
                areaName = "Dungeon " + LoadSubScene.areaExchange.GetWorldIndex() + "-" + LoadSubScene.areaExchange.GetDungeonIndex();
            }
            ZoomScript.areaName = areaName;
        }
    }
}
