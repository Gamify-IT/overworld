using System.Collections.Generic;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    //Object references
    [SerializeField] private GameObject keybindingPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject confirmationCanvas;

    //Keybinding
    private List<Keybinding> currentKeybindings;
    private List<Keybinding> newKeybindings;
    private bool validBindings;

    public AudioClip clickSound;
    private AudioSource audioSource;

    //Keybinding Objects
    private Dictionary<Binding, KeyBindingUIElement> keybindingObjects;

    //KeyCodes
    private KeyCode cancel;

    private void Start()
    {
        cancel = GameManager.Instance.GetKeyCode(Binding.CANCEL);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;

        keybindingObjects = new Dictionary<Binding, KeyBindingUIElement>();
        currentKeybindings = GameManager.Instance.GetKeybindings();
        newKeybindings = currentKeybindings;
        confirmationCanvas.SetActive(false);
        validBindings = true;

        DisplayKeybinding();
        InitializeAudio();
    }

    /// <summary>
    ///     This function adds new audio sources and sets click sound as a clip
    /// </summary>
    private void InitializeAudio()
    {
        audioSource=GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip=clickSound;
        audioSource.playOnAwake=false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(cancel))
        {
            if(confirmationCanvas.activeInHierarchy)
            {
                CancelButtonPressed();
            }
            else
            {
                BackButtonPressed();
            }            
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    /// <summary>
    ///     This function changes a given keybinding and checks, if the current bindings are valid or not and updates the UI accordingly
    /// </summary>
    /// <param name="keybinding">The <c>Keybinding</c> to change</param>
    public void ChangeKeybinding(Keybinding keybinding)
    {
        if (keybinding.GetBinding() != Binding.VOLUME_LEVEL)
        {
            Debug.Log("Change " + keybinding.GetBinding() + " to " + keybinding.GetKey());
            ChangeKeybindingInList(keybinding);
            ValidateKeybindings();
        }
    }

    /// <summary>
    ///     This function gets called when the back button is pressed
    ///     If the changes are valid, it saves them and closes the keybinding menu
    ///     Otherwise it opens the confirmation canvas
    /// </summary>
    public void BackButtonPressed()
    {
        PlayClickSound();
        if (validBindings)
        {
            SaveKeybindings();
            CloseKeybindingsMenu();
        }
        else
        {
            confirmationCanvas.SetActive(true);
        }
    }

    /// <summary>
    ///     This function gets called when the reset button is pressed
    ///     It resets the UI Elements to the current one stored
    /// </summary>
    public void ResetButtonPressed()
    {
        GameManager.Instance.ResetKeybindings();
        currentKeybindings = GameManager.Instance.GetKeybindings();
        newKeybindings = currentKeybindings;
        foreach (Keybinding keybinding in currentKeybindings)
        {
            Binding binding = keybinding.GetBinding();   
            
            if (binding != Binding.VOLUME_LEVEL)
            {
                keybindingObjects[binding].Setup(keybinding, this);
                GameEvents.current.KeybindingChange(binding);
            }
        }
    }

    /// <summary>
    ///     This function gets called when the confirm button is pressed
    ///     It closes the keybindings menu without saving the changes
    /// </summary>
    public void ConfirmButtonPressed()
    {
        CloseKeybindingsMenu();
    }

    /// <summary>
    ///     This function gets called when the cancel button is pressed
    ///     It closes the confirmation panel
    /// </summary>
    public void CancelButtonPressed()
    {
        confirmationCanvas.SetActive(false);
        PlayClickSound();
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
        }
    }

    /// <summary>
    ///     This function displays all keybindings
    /// </summary>
    private void DisplayKeybinding()
    {
        foreach (Keybinding keybinding in currentKeybindings)
        {
            if (keybinding.GetBinding() != Binding.VOLUME_LEVEL)
            {
                DisplayKeybinding(keybinding);
            }
        }
    }

    /// <summary>
    ///     This function creates a GameObject for the given <c>Keybinding</c>
    /// </summary>
    /// <param name="keybinding">The keybinding a GameObject should be created for</param>
    private void DisplayKeybinding(Keybinding keybinding)
    {
        GameObject keybindingObject = Instantiate(keybindingPrefab, content.transform, false);

        KeyBindingUIElement keyBindingUIElement = keybindingObject.GetComponent<KeyBindingUIElement>();
        if (keyBindingUIElement != null)
        {
            keyBindingUIElement.Setup(keybinding, this);
            keybindingObjects.Add(keybinding.GetBinding(), keyBindingUIElement);
            keyBindingUIElement.SetClickSound(clickSound);
        }
        else
        {
            Destroy(keybindingObject);
        }
    }


    /// <summary>
    ///     This function changes the given Keybinding in the <c>newKeybindings</c> List
    /// </summary>
    /// <param name="newKeybinding">The keybinding to be changed</param>
    private void ChangeKeybindingInList(Keybinding newKeybinding)
    {
        Binding binding = newKeybinding.GetBinding();
        KeyCode keyCode = newKeybinding.GetKey();
        foreach(Keybinding keybinding in newKeybindings)
        {
            if(keybinding.GetBinding().Equals(binding))
            {
                keybinding.SetKey(keyCode);
            }
        }
    }

    /// <summary>
    ///     This function checks, whether all Bindings are correct or not and marks invalid bindings red
    /// </summary>
    private void ValidateKeybindings()
    {
        validBindings = true;
        List<KeyCode> allKeyCodes = new List<KeyCode>();
        List<KeyCode> duplicateKeyCodes = new List<KeyCode>();
        foreach(Keybinding keybinding in newKeybindings)
        {
            KeyCode keyCode = keybinding.GetKey();
            if(allKeyCodes.Contains(keyCode))
            {
                validBindings = false;
                duplicateKeyCodes.Add(keyCode);
            }
            allKeyCodes.Add(keyCode);
        }
        foreach (Keybinding keybinding in newKeybindings)
        {
            Binding binding = keybinding.GetBinding();
            KeyCode keyCode = keybinding.GetKey();

            if (binding != Binding.VOLUME_LEVEL)
            {
                if (duplicateKeyCodes.Contains(keyCode))
                {
                    keybindingObjects[binding].MarkInvalid();
                }
                else
                {
                    keybindingObjects[binding].MarkValid();
                }
            }          
        }
        Debug.Log("Keybindings are valid: " + validBindings);
    }

    /// <summary>
    ///     This function saves the keybindings
    /// </summary>
    private void SaveKeybindings()
    {
        foreach(Keybinding keybinding in newKeybindings)
        {
            GameManager.Instance.ChangeKeybind(keybinding);
        }
    }

    /// <summary>
    ///     This function closes the keybindings menu
    /// </summary>
    private void CloseKeybindingsMenu()
    {
        PauseMenu pauseMenu = this.gameObject.GetComponent<PauseMenu>();
        if(pauseMenu != null)
        {
            pauseMenu.CloseSubMenu();
        }
    }

    /// <summary>
    /// This function plays the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if(clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}