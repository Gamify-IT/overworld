using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class is used to handle the different panels regarding the leaderboard
/// </summary>
public class OpenLeaderboard : MonoBehaviour
{

    public static bool menuOpen = true;
    public GameObject rewardsPanel;

    private AudioSource audioSource;
    public AudioClip clickSound;

    /// <summary>
    /// Initializes the AudioSource component and sets the click sound.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
    }


    /// <summary>
    /// Opens the rewards menu and pauses the game.
    /// </summary>
    public void OpenMenue()
    {
        menuOpen = true;
        rewardsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Opens the leaderboard scene and pauses the game.
    /// </summary>
    public void OpenBoard()
    {
        menuOpen = true;
        SceneManager.LoadScene("Rewards", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Opens the shop scene and pauses the game.
    /// </summary>
    public void OpenShop()
    {
        menuOpen = true;
        SceneManager.LoadScene("Shop", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Closes the rewards menu, plays a click sound, and resumes the game.
    /// </summary>
    public void CloseMenue()
    {
        audioSource.Play();
        Invoke("RewardsPanelSetActive", 0.15f);
        menuOpen = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Deactivates the rewards panel.
    /// </summary>
    private void RewardsPanelSetActive()
    {
        rewardsPanel.SetActive(false);
    }

}
