using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System;

/// <summary>
///     This script manages the pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static bool menuOpen;
    public static bool subMenuOpen;
    public static string buttonName;

    public AudioClip clickSound;
    private AudioSource audioSource;

    //KeyCodes
    private KeyCode cancel;

    private int daysPlayed;
    private DateTime lastLoginDate;
    
    private void Start()
    {
        audioSource=GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip=clickSound;

        cancel = GameManager.Instance.GetKeyCode(Binding.CANCEL);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
        PlayClickSound();
    }

    /// <summary>
    ///     The <c>Update</c> function is called once every frame.
    ///     This function checks, if an input occured and if so, adapts the pause menu accordingly.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(cancel) && !PlayerAnimation.Instance.IsBusy())
        {
            PauseOrResume();
        }
        
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    /// <summary>
    ///     This function changes the state of the pause menu.
    ///     It opens or closes the menu or closes a submenu, based on the current state.
    /// </summary>
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
            if (!buttonName.Equals("Keybindings"))
            {
                CloseSubMenu();
            }
        }
    }

    /// <summary>
    ///     This function closes the pause menu. 
    /// </summary>
    public void Resume()
    {
        //create temporary AudioSource to play sound and then destroy it
        if (clickSound != null)
        {
            GameObject temporaryAudioSource = new GameObject("TemporaryAudio");
            AudioSource audioSource = temporaryAudioSource.AddComponent<AudioSource>();
            audioSource.clip = clickSound;
            audioSource.PlayOneShot(clickSound);
            Destroy(temporaryAudioSource, clickSound.length);
        }
        PlayClickSound();
        SceneManager.UnloadScene("Menu");
        menuOpen = false;
        GameManager.Instance.isPaused = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    ///     This function opens the pause menu.
    /// </summary>
    public void Pause()
    {
        menuOpen = true;
        GameManager.Instance.isPaused = true;
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        Time.timeScale = 0f;

        string lastLoginDateStr = PlayerPrefs.GetString("LastLoginDate", "");
        int daysCount = PlayerPrefs.GetInt("DaysPlayed", 0);

        DateTime today = DateTime.Today;
        if (!string.IsNullOrEmpty(lastLoginDateStr))
        {
            lastLoginDate = DateTime.Parse(lastLoginDateStr);

            if (lastLoginDate.Date < today)
            {
                daysPlayed = daysCount + 1;
                PlayerPrefs.SetInt("DaysPlayed", daysPlayed);
                PlayerPrefs.Save();

                GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GAMER, 1);
                GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.PROFESSIONAL_GAMER, 1);
            }
            else
            {
                daysPlayed = daysCount;
            }
        }
        else
        {
            lastLoginDate = DateTime.Now;
            daysPlayed = 1;
            PlayerPrefs.SetInt("DaysPlayed", daysPlayed);
            PlayerPrefs.Save();
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GAMER, 1);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.PROFESSIONAL_GAMER, 1);
        }

        PlayerPrefs.SetString("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
        Debug.Log("day: days played " + daysPlayed);
        Debug.Log("day: current date " + DateTime.Now);
        Debug.Log("day: last play date " + lastLoginDate);
    }

    /// <summary>
    ///     This function is called by a menu button.
    ///     This function stores which button was pressed.
    /// </summary>
    public void SubMenuSelection()
    {
        //get the name of the pressed button
        buttonName = EventSystem.current.currentSelectedGameObject.name;
        //remove appendix " Button" = length 7
        buttonName = buttonName.Remove(buttonName.Length - 7);

    }

    /// <summary>
    ///     This function is called by a menu button.
    ///     This function opens the submenu.
    /// </summary>
    public void OpenSubMenu()
    {
        subMenuOpen = true;
        menuOpen = false;
        SceneManager.UnloadScene("Menu");
        //load the scene which was set with SubMenuSelection()
        SceneManager.LoadScene(buttonName, LoadSceneMode.Additive);
    }

    /// <summary>
    ///     This function closes a submenu.
    /// </summary>
    public void CloseSubMenu()
    {
        menuOpen = true;
        subMenuOpen = false;
        SceneManager.UnloadScene(buttonName);
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

    /// <summary>
    ///     This function updates the keybindings
    /// </summary>
    /// <param name="binding">The binding that changed</param>
    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.CANCEL)
        {
            cancel = GameManager.Instance.GetKeyCode(Binding.CANCEL);
            PlayClickSound();
        }
    }


    /// <summary>
    /// This function is called by the menu and submenu buttons.
    /// This function plays the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}

