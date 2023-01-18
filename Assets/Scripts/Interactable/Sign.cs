using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class is responsible for the sign logic.
/// </summary>
public class Sign : MonoBehaviour
{
    public GameObject SignPanel;
    public TMP_Text signText;
    private bool playerIsClose;
    public string text;

    //KeyCodes
    private KeyCode interact;

    /// <summary>
    ///     This function is called when the object becomes enabled and active.
    ///     It is used to initialize the sign.
    /// </summary>
    private void Awake()
    {
        SignPanel.SetActive(false);
        signText.text = text;
        interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
    }


    /// <summary>
    ///     If the player is in range of the sign the playerIsClose check will be set to true.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    /// <summary>
    ///     If the player leaves the range of the sign while the sign is open, the sign will be closed and playerIsClose check
    ///     will be set to false.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            SignPanel.SetActive(false);
        }
    }

    /// <summary>
    ///     This method opens the sign if the player is close to the sign and presses "E".
    ///     If the player wants exit the sign they can press "E" again.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(interact) && playerIsClose && !SceneManager.GetSceneByBuildIndex(12).isLoaded &&
            !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
        {
            signText.text = text;
            SignPanel.SetActive(!SignPanel.activeInHierarchy);
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    /// <summary>
    ///     This function updates the keybindings
    /// </summary>
    /// <param name="binding">The binding that changed</param>
    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.INTERACT)
        {
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        }
    }
}