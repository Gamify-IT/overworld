using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keybinding
{
    private Binding binding;
    private KeyCode key;

    public Keybinding(Binding binding, KeyCode key)
    {
        this.binding = binding;
        this.key = key;
    }

    #region Getter And Setter
    public Binding GetBinding()
    {
        return binding;
    }

    public KeyCode GetKey()
    {
        return key;
    }

    public void SetKey(KeyCode keyCode)
    {
        key = keyCode;
    }

    /// <summary>
    ///     This function converts a <c>KeybindingDTO</c> to a <c>Keybinding</c>
    /// </summary>
    /// <param name="keybindingDTO">The <c>KeybindingDTO</c> to convert</param>
    /// <returns></returns>
    public static Keybinding ConvertDTO(KeybindingDTO keybindingDTO)
    {
        Binding binding = (Binding) System.Enum.Parse(typeof(Binding), keybindingDTO.binding);
        KeyCode key;

        if (binding != Binding.VOLUME_LEVEL)
        {
            key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keybindingDTO.key);
        }
        else
        {
            string receivedKey = keybindingDTO.key;
            key = receivedKey != "" ? DataManager.Instance.ConvertIntToKeyCode(int.Parse(keybindingDTO.key)) : DataManager.Instance.ConvertIntToKeyCode(1);
        }   
        
        Debug.Log("Converted key for " + binding.ToString() + ": " + key.ToString());
        Keybinding keybinding = new Keybinding(binding, key);
        return keybinding;
    }

    #endregion
}
