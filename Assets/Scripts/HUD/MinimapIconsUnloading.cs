using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages which minimap icons should be shown.
/// </summary>
public class MinimapIconsUnloading : MonoBehaviour
{
    public string nameOfCurrentScene;

    /// <summary>
    /// This function is called when the players enters an area.
    /// This function removes all minigame icons which should not be shown.
    /// </summary>
    /// <param name="player">The player object</param>
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            GameObject[] minimapIcons = GameObject.FindGameObjectsWithTag("MinimapIcon");

            foreach (GameObject minimapIcon in minimapIcons)
            {
                if (!minimapIcon.name.Contains(nameOfCurrentScene) && !nameOfCurrentScene.Equals(""))
                {
                    minimapIcon.GetComponent<SpriteRenderer>().enabled = true;;
                }
            }
            
            if (nameOfCurrentScene.Equals(""))
            {
                this.transform.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    /// <summary>
    /// This function is called when the players leaves an area.
    /// This function removes all minigame icons which should not be shown.
    /// </summary>
    /// <param name="player">The player object</param>
    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            GameObject[] minimapIcons = GameObject.FindGameObjectsWithTag("MinimapIcon");

            foreach (GameObject minimapIcon in minimapIcons)
            {
                if (minimapIcon.name.Contains(nameOfCurrentScene) && !nameOfCurrentScene.Equals(""))
                {
                    minimapIcon.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            
            if (nameOfCurrentScene.Equals(""))
            {
                this.transform.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}