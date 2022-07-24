using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/*
 * This script manages the pause menu controlled by the 'ESC' button.
 * The pause menu can either be open or close. 
 * If the menu is open, there is the possibility for a submenu to be active as well. 
 */
public class PauseMenu : MonoBehaviour
{
    public static bool menuOpen = false;
    public static bool subMenuOpen = false;
    public static string buttonName;
    
    /*
     * The Update function is called every frame. It updates all values according to the changes happened since the last frame. 
     * It toggles the state of the pause menu if an input happened.
     */
    void Update()
    {
        //esc handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseOrResume();
        }
    }

    /*
     * This function toggles the state of the pause menu. 
     * If it was active it gets set as inactive and the other way round. 
     * If a submenu is open it gets closed instead of the entire menu. 
     */
    public void PauseOrResume()
    {
        //open menu when its not opened
        if (!menuOpen && !subMenuOpen)
        {
            Pause();
        }
        //close menu when its opened
        else if (menuOpen && !subMenuOpen)
        {
            Resume();
        }
        //add option to go back out of a submenu into the normal pause menu again (instead of clicking the back button)
        else
        {
            CloseSubMenu();
        }
    }

    /*
     * This function closes the pause menu.
     */
    public void Resume()
    {
        SceneManager.UnloadScene("Menu");
        menuOpen = false;
        Time.timeScale = 1f;
    }
    
    /*
     * This function openes the pause menu.
     */
    public void Pause()
    {
        menuOpen = true;
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    /*
     * This function gets called by a button to select a submenu. It stores the name of button to open the corresponding sub menu. 
     */
    public void SubMenuSelection()
    {
        //get the name of the pressed button
        buttonName = EventSystem.current.currentSelectedGameObject.name;
        //remove appendix " Button" = length 7
        buttonName = buttonName.Remove(buttonName.Length - 7);
    }
    
    /*
     * This function opens a sub menu.
     */
    public void OpenSubMenu()
    {
        subMenuOpen = true;
        menuOpen = false;
        SceneManager.UnloadScene("Menu");
        //load the scene which was set with SubMenuSelection()
        SceneManager.LoadScene(buttonName, LoadSceneMode.Additive);
    }

    /*
     * This function closes the opened sub menu.
     */
    public void CloseSubMenu()
    {
        menuOpen = true;
        subMenuOpen = false;
        //unload the scene which was set with SubMenuSelection()
        SceneManager.UnloadScene(buttonName);
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }
}
