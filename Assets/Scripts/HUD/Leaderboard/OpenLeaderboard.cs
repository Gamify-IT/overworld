using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OpenLeaderboard : MonoBehaviour
{

    public static bool menuOpen = true;
    public GameObject rewardsPanel;
    public GameObject shopPanel;

    private AudioSource audioSource;
    public AudioClip clickSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
    }
    public void openMenue()
    {
        menuOpen = true;
        rewardsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void openLeaderboard()
    {
        menuOpen = true;
        SceneManager.LoadScene("Rewards", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    public void openShop()
    {
        menuOpen = true;
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void closeMenue()
    {
        audioSource.Play();
        Invoke("RewardsPanelSetActive", 0.15f);
        menuOpen = false;
        Time.timeScale = 1f; 
    }

    private void RewardsPanelSetActive()
    {
        rewardsPanel.SetActive(false); 
    }

    public void closeShop()
    {
        audioSource.Play();
        Invoke("ShopPanelSetActive", 0.15f);
        menuOpen = false;
        Time.timeScale = 1f;
    }

    private void ShopPanelSetActive()
    {
        shopPanel.SetActive(false);
    }

}
