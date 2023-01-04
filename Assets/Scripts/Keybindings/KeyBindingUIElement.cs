using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections;
using System;

public class KeyBindingUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text binding;

    private Keybinding keybinding;

    public void Setup(Keybinding keybinding)
    {
        this.keybinding = keybinding;
        title.text = keybinding.GetBinding().ToString();
        binding.text = keybinding.GetKey().ToString();
    }

    public void ChangeKeyButtonPressed()
    {
        binding.text = "___";
        StartCoroutine(UpdateKeyBinding());        
    }

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
        Debug.Log("Stop animation");
        if (pressedKey != keybinding.GetKey())
        {
            Keybinding newKeybinding = new Keybinding(keybinding.GetBinding(), pressedKey);
            Setup(newKeybinding);
            GameManager.Instance.ChangeKeybind(newKeybinding);
        }
        else
        {
            binding.text = keybinding.GetKey().ToString();
        }        
    }
}
