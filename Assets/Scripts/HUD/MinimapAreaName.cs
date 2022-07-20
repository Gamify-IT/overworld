using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapAreaName : MonoBehaviour
{
    public string areaName;
    [SerializeField]  private int areaNumber;
    
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            MinimapScript.areaName = areaName;
            GameManager.instance.loadWorld(areaNumber);
        }
    }
}
