using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
///     This class is responsible for the Navigationmap logic.
/// </summary>
public class NavigationMap : MonoBehaviour
{
    public GameObject MapPanel;
    private bool playerIsClose;

    //KeyCodes
    private KeyCode interact;

    /// <summary>
    ///     This function is called when the object becomes enabled and active.
    ///     It is used to initialize the map.
    /// </summary>
    private void Awake()
    {
        MapPanel.SetActive(false);
        interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
    }


    /// <summary>
    ///     If the player is in range of the map the playerIsClose check will be set to true.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    /// <summary>
    ///     If the player leaves the range of the map while the map is open, the map will be closed and playerIsClose check will be set to false.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            MapPanel.SetActive(false);
        }
    }

    /// <summary>
    ///     This method opens the NavigationMap if the player is close to the map and presses "E".
    ///     If the player wants exit the map they can press "E" again.
    /// </summary>
    private void Update()
    {

        if (Input.GetKeyDown(interact) && playerIsClose && !SceneManager.GetSceneByBuildIndex(12).isLoaded &&
            !PauseMenu.menuOpen)
        {
            MapPanel.SetActive(!MapPanel.activeInHierarchy);
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.INTERACT)
        {
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        }
    }
}
