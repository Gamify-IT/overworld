using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

/// <summary>
///     This script manages the volume controller button
/// </summary>
public class VolumeControllerButton : MonoBehaviour
{
    [SerializeField] private Sprite mutedImage;
    [SerializeField] private Sprite quietImage;
    [SerializeField] private Sprite midImage;
    [SerializeField] private Sprite highImage;
    [SerializeField] private AudioClip clickSound;

    private Button button;
    private Image buttonImage;
    private AudioSource audioSource;
    private static int volumeLevel;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
        
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(ChangeVolume);

        if(GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            volumeLevel = 1;  
        }
        UpdateButtonImage();
    }

    /// <summary>
    ///     This function changes the volume level to the next one, updates the menu and saves the new volume level
    /// </summary>
    private void ChangeVolume()
    {
        audioSource.Play();
        volumeLevel = (volumeLevel + 1) % 4;

        UpdateButtonImage();
        GameManager.Instance.UpdateVolume(volumeLevel);

        KeyCode volumeLevelKey = DataManager.Instance.ConvertIntToKeyCode(volumeLevel);
        Keybinding volumeLevelBinding = new Keybinding(Binding.VOLUME_LEVEL, volumeLevelKey);
        GameManager.Instance.ChangeKeybind(volumeLevelBinding);
    }

    /// <summary>
    ///     This function changes the volume controller button's image to the next one
    /// </summary>
    private void UpdateButtonImage()
    {
        switch (volumeLevel)
        {
            case 0:
                buttonImage.sprite = mutedImage;
                break;
            case 1:
                buttonImage.sprite = quietImage;
                break;
            case 2:
                buttonImage.sprite = midImage;
                break;
            case 3:
                buttonImage.sprite = highImage;
                break;
        }
    }

    /// <summary>
    ///     Updates the current volume level to a new value if this value is between 0 and 3
    /// </summary>
    /// <param name="volumeLevel">new volume level between 0 and 3</param>
    public static void SetVolumeLevel(int volumeLevel)
    {
        if (volumeLevel >= 0 && volumeLevel <= 3)
        {
            VolumeControllerButton.volumeLevel = volumeLevel;
        }
    }

}
