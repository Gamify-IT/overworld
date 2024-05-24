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

    public AudioClip backgroundSound;
    private AudioSource audioSource;


    private void Start()
    {
        //get AudioSource component
        audioSource = GetComponent<AudioSource>();
        //add AudioSource component if necessary
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        //set audio clip
        audioSource.clip = backgroundSound;
        //set AudioSource to loop
        audioSource.loop = true;
        //AudioSource does not start playing automatically when the GameObject awakens
        audioSource.playOnAwake = false;
    }


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
            PlayBackgroundSound();
        }
    }

    /// <summary>
    /// This function plays the background music.
    /// </summary>
    private void PlayBackgroundSound()
    {
        if (backgroundSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
