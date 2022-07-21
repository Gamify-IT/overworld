using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconsUnloading : MonoBehaviour
{
    public string nameOfCurrentScene;
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
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