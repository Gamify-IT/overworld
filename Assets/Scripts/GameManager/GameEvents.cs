using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    public void Awake()
    {
        current = this;
    }

    public event Action onKeybindingChange;

    public void KeybingingChange()
    {
        if (onKeybindingChange != null)
        {
            onKeybindingChange();
        }
    }
}