using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    public void Awake()
    {
        current = this;
    }

    public event Action<Binding> onKeybindingChange;

    public void KeybingingChange(Binding binding)
    {
        if (onKeybindingChange != null)
        {
            onKeybindingChange(binding);
        }
    }
}