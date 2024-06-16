using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OpenLeaderboard : MonoBehaviour
{

    public static bool menuOpen = true;

    public void Pause()
    {
        menuOpen = true;
        SceneManager.LoadScene("Rewards", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }
}
