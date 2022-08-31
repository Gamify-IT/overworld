using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool menuOpen;
    public static bool subMenuOpen;

    public static string buttonName;

    // Update is called once per frame
    private void Update()
    {
        //esc handling
        if (Input.GetKeyDown(KeyCode.Escape) && !Animation.Instance.IsBusy())
        {
            PauseOrResume();
        }
    }

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

    //handles everyting when pause menu is closed
    public void Resume()
    {
        SceneManager.UnloadScene("Menu");
        menuOpen = false;
        Time.timeScale = 1f;
    }

    //handles everyting when pause menu is opened
    public void Pause()
    {
        menuOpen = true;
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        Time.timeScale = 0f;
    }

    //This sets a string to the name of the button clicked. If we name the Menu Buttons consistently, this saves labor for future menu buttons. So if the scene for a submenu is named "Something" the Button has to be named "Something Button"
    public void SubMenuSelection()
    {
        //get the name of the pressed button
        buttonName = EventSystem.current.currentSelectedGameObject.name;
        //remove appendix " Button" = length 7
        buttonName = buttonName.Remove(buttonName.Length - 7);
    }

    //handles everyting when submenu button is clicked
    public void OpenSubMenu()
    {
        subMenuOpen = true;
        menuOpen = false;
        SceneManager.UnloadScene("Menu");
        //load the scene which was set with SubMenuSelection()
        SceneManager.LoadScene(buttonName, LoadSceneMode.Additive);
    }

    public void CloseSubMenu()
    {
        menuOpen = true;
        subMenuOpen = false;
        //unload the scene which was set with SubMenuSelection()
        SceneManager.UnloadScene(buttonName);
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }
}