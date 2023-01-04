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

    #region Getter

    public Binding GetBinding()
    {
        return binding;
    }

    public KeyCode GetKey()
    {
        return key;
    }

    #endregion
}
