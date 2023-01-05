using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KeyBindingUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text binding;

    private Keybinding keybinding;

    /// <summary>
    ///     This function sets up the UI Element
    /// </summary>
    /// <param name="keybinding">The keybinding data</param>
    public void Setup(Keybinding keybinding)
    {
        this.keybinding = keybinding;
        title.text = keybinding.GetBinding().ToString();
        binding.text = keybinding.GetKey().ToString();
        title.color = Color.black;
        binding.color = Color.black;

        ValidateKeybinding(new Binding());
    }

    private void Start()
    {
        GameEvents.current.onKeybindingChange += ValidateKeybinding;
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= ValidateKeybinding;
    }

    /// <summary>
    ///     This function validate the keybinding. If invalid it will color the two bindings red.
    /// </summary>
    /// <param name="binding">Dummy and never needed</param>
    private void ValidateKeybinding(Binding binding) //binding is just there that the event system works
    {
        if (GameManager.Instance.CountSameKeyCodesInKeybindings(keybinding.GetKey()) > 1)
        {
            title.color = Color.red;
            this.binding.color = Color.red;
            Debug.Log("Invalid button: " + keybinding.GetKey());
        }
        else
        {
            title.color = Color.black;
            this.binding.color = Color.black;
            Debug.Log("Valid button: " + keybinding.GetKey());
        }
    }

    /// <summary>
    ///     This function is called by the Change Key Button and starts the keychanging process
    /// </summary>
    public void ChangeKeyButtonPressed()
    {
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
            Keybinding newKeybinding = new Keybinding(keybinding.GetBinding(), pressedKey);
            Setup(newKeybinding);

            GameManager.Instance.ChangeKeybind(newKeybinding);
        }
        else
        {
            Keybinding newKeybinding = new Keybinding(keybinding.GetBinding(), pressedKey);
            Setup(newKeybinding);
            Debug.Log("Same button " + pressedKey);
        }
    }
}