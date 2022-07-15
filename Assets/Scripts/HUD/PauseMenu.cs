using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool menuOpen = false;

    // Update is called once per frame
    void Update()
    {
        //esc handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //close menu when its opened
            if (menuOpen)
            {
                Resume();
            }
            //open menu when its not opened
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        SceneManager.UnloadScene("Menu");
        menuOpen = false;
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        menuOpen = true;
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }
}
