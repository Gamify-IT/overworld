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
    private int volumeLevel;
    public AudioClip clickSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
        
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(ChangeVolume);
        volumeLevel = PlayerPrefs.GetInt("VolumeLevel", 3);
        UpdateButtonImage();
        UpdateVolume();
    }

    private void ChangeVolume()
    {
        audioSource.Play();
        volumeLevel = (volumeLevel + 1) % 4;
        PlayerPrefs.SetInt("VolumeLevel", volumeLevel);
        UpdateVolume();
        UpdateButtonImage();
    }

    private void UpdateVolume()
    {
        float volume = 0f;
        switch (volumeLevel)
        {
            case 0:
                volume = 0f;
                break;
            case 1:
                volume = 0.33f;
                break;
            case 2:
                volume = 0.66f;
                break;
            case 3:
                volume = 1f;
                break;
        }

        AudioListener.volume = volume;
    }

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
