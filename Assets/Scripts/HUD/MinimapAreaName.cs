using System.Collections;
using UnityEngine;

public class MinimapAreaName : MonoBehaviour
{
    public string areaName;
    public bool dungeonArea;

    private string currentAreaName;
    
    private PlayerStatisticData playerData;
    private int volumeLevel;
  
    public AudioClip backgroundMusicWorld1;
    public AudioClip backgroundMusicWorld2;
    public AudioClip backgroundMusicWorld3;
    public AudioClip backgroundMusicWorld4;
    public AudioClip backgroundMusicDungeon;
    public static AudioClip clipToPlay;
    
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    private bool isUsingSource1 = true;

    private static AudioClip currentClip;

    public float crossfadeDuration = 0.5f;

    /// <summary>
    /// This function initializes the audio sources and starts playing the background music
    /// </summary>
    private void Start()
    {
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();

        InitializeAudioSource(audioSource1);
        InitializeAudioSource(audioSource2);

        PlayBackgroundMusic();
        
        UpdateVolume();
    }

    /// <summary>
    /// This function updates the volume level and applies the changes to all audio in the game
    /// </summary>
    private void UpdateVolume()
    {
        playerData = DataManager.Instance.GetPlayerData();
        volumeLevel = playerData.GetVolumeLevel();
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
    /// This function configures the specified audio source with initial settings
    /// </summary>
    /// <param name="audioSource">The AudioSource component to initialize</param>
    private void InitializeAudioSource(AudioSource audioSource)
    {
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    /// <summary>
    /// This function updates the current area name and changes the background music if the area has changed
    /// </summary>
    private void Update()
    {
        string newAreaName = ZoomScript.areaName;
        if (newAreaName != currentAreaName)
        {
            currentAreaName = newAreaName;
            PlayBackgroundMusic();
        }
    }

    /// <summary>
    /// This function sets the name of the area on the minimap
    /// </summary>
    /// <param name="other">The object entering the trigger</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (dungeonArea)
            {
                areaName = "Dungeon " + LoadSubScene.areaExchange.GetWorldIndex() + "-" + LoadSubScene.areaExchange.GetDungeonIndex();
            }
            ZoomScript.areaName = areaName;
        }
    }

    /// <summary>
    /// This function starts a new audio clip for the received area
    /// </summary>
    private void PlayBackgroundMusic()
    {
        GetClipToPlay();
        if (clipToPlay != currentClip || clipToPlay == backgroundMusicDungeon)
        {
            StartCoroutine(CrossfadeMusic(clipToPlay));
            currentClip = clipToPlay;
        }
    }

    /// <summary>
    /// This function gives an audio clip based on the current area name
    /// </summary>
    private void GetClipToPlay()
    {
        if (currentAreaName.Contains("World 1"))
        {
            clipToPlay = backgroundMusicWorld1;
        }
        else if (currentAreaName.Contains("World 2"))
        {
            clipToPlay = backgroundMusicWorld2;
        }
        else if (currentAreaName.Contains("World 3"))
        {
            clipToPlay = backgroundMusicWorld3;
        }
        else if (currentAreaName.Contains("World 4"))
        {
            clipToPlay = backgroundMusicWorld4;
        }
        else
        {
            clipToPlay = backgroundMusicDungeon;
        }
    }

    /// <summary>
    /// This function makes smoothly crossfades between the currently playing audio clip and a new one
    /// </summary>
    /// <param name="newClip">The new audio clip to play.</param>
    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        AudioSource currentSource = isUsingSource1 ? audioSource1 : audioSource2;
        AudioSource newSource = isUsingSource1 ? audioSource2 : audioSource1;

        newSource.clip = newClip;
        newSource.Play();

        float elapsedTime = 0f;

        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / crossfadeDuration;

            currentSource.volume = Mathf.Lerp(1f, 0f, t);
            newSource.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }
        currentSource.volume = 0f;
        newSource.volume = 1f;
        isUsingSource1 = !isUsingSource1;
    }
}
