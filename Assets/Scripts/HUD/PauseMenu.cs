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
                SceneManager.UnloadScene("Menu");
                menuOpen = false;
            }
            //open menu when its not opened
            else
            {
                menuOpen = true;
                SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
            }
        }
    }
}
