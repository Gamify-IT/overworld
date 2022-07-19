using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapAreaName : MonoBehaviour
{
    public string areaName;
    
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            MinimapScript.areaName = areaName;
        }
    }
}
