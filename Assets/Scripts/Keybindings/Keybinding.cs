using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keybinding
{
    private Binding binding;
    private KeyCode key;
    private int volumeLevel;

    public Keybinding(Binding binding, KeyCode key, int volumeLevel)
    {
        this.binding = binding;
        this.key = key;
        this.volumeLevel = volumeLevel;
    }

    #region Getter And Setter

    public int GetVolumeLevel()
    {
        return volumeLevel;
    }

    public void SetVolumeLevel(int volumeLevel)
    {
        this.volumeLevel = volumeLevel;
    }

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
        KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keybindingDTO.key);
        int volumeLevel = keybindingDTO.volumeLevel;
        Keybinding keybinding = new Keybinding(binding, key, volumeLevel);
        return keybinding;
    }

    #endregion
}
