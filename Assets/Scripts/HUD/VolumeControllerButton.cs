using UnityEngine;
using UnityEngine.UI;

public class VolumeControllerButton : MonoBehaviour
{
    public Sprite mutedImage;
    public Sprite quietImage;
    public Sprite midImage;
    public Sprite highImage;

    private Button button;
    private Image buttonImage;
    public static int volumeLevel;
    public AudioClip clickSound;
    private AudioSource audioSource;

    /// <summary>
    /// This function initializes the audio sources and gets last volume level choice
    /// </summary>
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
        
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(ChangeVolume);

        PlayerstatisticDTO playerData = DataManager.Instance.GetPlayerData();
        volumeLevel = playerData.volumeLevel;

        UpdateButtonImage();
        UpdateVolume();
    }

    /// <summary>
    /// This function changes the volume level to the next one and save this level in PlayerPrefs
    /// </summary>
    private void ChangeVolume()
    {
        audioSource.Play();
        volumeLevel = (volumeLevel + 1) % 4;
        GameManager.Instance.SetVolumeLevel(volumeLevel);
        GameManager.Instance.SaveVolumeLevel();
        UpdateVolume();
        UpdateButtonImage();
    }

    /// <summary>
    /// This function updates the level volume and applies the changes to all audio in the game
    /// </summary>
    private void UpdateVolume()
    {
        float volume = 0f;
        switch (volumeLevel)
        {
            case 0:
                volume = 0f;
                break;
            case 1:
                volume = 0.5f;
                break;
            case 2:
                volume = 1f;
                break;
            case 3:
                volume = 2f;
                break;
        }
        AudioListener.volume = volume;
    }

    /// <summary>
    /// This function changes the volume controller button to the next one
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
}
