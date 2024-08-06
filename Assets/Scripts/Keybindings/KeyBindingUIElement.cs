using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KeyBindingUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text binding;

    private KeyBindingManager keyBindingManager;

    private Keybinding keybinding;

    public AudioClip clickSound;
    private AudioSource audioSource;

    /// <summary>
    ///     This function sets up the UI Element
    /// </summary>
    /// <param name="keybinding">The keybinding data</param>
    public void Setup(Keybinding keybinding, KeyBindingManager keyBindingManager)
    {
        this.keyBindingManager = keyBindingManager;
        this.keybinding = keybinding;
        UpdateUI();
        title.color = Color.black;
        binding.color = Color.black;
    }

    /// <summary>
    ///     This function marks this keybinding as invalid
    /// </summary>
    public void MarkInvalid()
    {
        title.color = Color.red;
        binding.color = Color.red;
    }

    /// <summary>
    ///     This function marks this keybinding as valid
    /// </summary>
    public void MarkValid()
    {
        title.color = Color.black;
        binding.color = Color.black;
    }

    /// <summary>
    ///     This function is called when the Change Key Button is pressed
    ///     It starts the keychanging process
    /// </summary>
    public void ChangeKeyButtonPressed()
    {
        audioSource=GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip=clickSound;
        audioSource.playOnAwake=false;
        PlayClickSound();

        title.color = Color.black;
        binding.color = Color.black;
        binding.text = "___";
        StartCoroutine(UpdateKeyBinding());
    }

    /// <summary>
    ///     This corountine waits for an user input and updates the stored key and UI accordingly
    ///     (Same Key: no update is done)
    ///     (Other valid key: updates UI and stored key)
    ///     (Other invalid key: updates UI to red)
    /// </summary>
    private IEnumerator UpdateKeyBinding()
    {
        Array keyCodes = Enum.GetValues(typeof(KeyCode));
        KeyCode pressedKey = KeyCode.None;
        bool userInput = false;
        while (!userInput)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in keyCodes)
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        pressedKey = keyCode;
                        userInput = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        if (pressedKey != keybinding.GetKey())
        {
            keybinding.SetKey(pressedKey);
            UpdateUI();

            keyBindingManager.ChangeKeybinding(keybinding);
        }
    }

    /// <summary>
    ///     This function updates the UI
    /// </summary>
    private void UpdateUI()
    {
        title.text = keybinding.GetBinding().ToString().Replace("_", " ");
        binding.text = keybinding.GetKey().ToString();
    }

    /// <summary>
    /// This function plays the click sound
    /// </summary>
    private void PlayClickSound()
    {
        if(clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    /// <summary>
    /// This function calls in the Key Binding Manager.
    /// This function sets the click sound to the object.
    /// </summary>
    public void SetClickSound(AudioClip clip)
    {
        clickSound = clip;            
    }
}