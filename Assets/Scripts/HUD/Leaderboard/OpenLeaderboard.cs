using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OpenLeaderboard : MonoBehaviour
{

    public static bool menuOpen = true;
    public GameObject rewardsPanel;
    public GameObject shopPanel;

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
        menuOpen = false;
        rewardsPanel.SetActive(false); 
        Time.timeScale = 1f; 
    }

    public void closeShop()
    {
        menuOpen = false;
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

}
