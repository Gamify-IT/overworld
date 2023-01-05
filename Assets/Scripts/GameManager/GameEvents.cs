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

    public void KeybindingChange(Binding binding)
    {
        if (onKeybindingChange != null)
        {
            onKeybindingChange(binding);
        }
    }

    public event Action onKeybindingReset;

    public void KeybindingReset()
    {
        if (onKeybindingReset != null)
        {
            onKeybindingReset();
        }
    }
}