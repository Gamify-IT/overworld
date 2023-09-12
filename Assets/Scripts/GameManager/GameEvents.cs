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

    public event Action<AreaInformation> onDungeonLoad;

    public void KeybindingChange(Binding binding)
    {
        if (onKeybindingChange != null)
        {
            onKeybindingChange(binding);
        }
    }

    public void SetupDungeon(AreaInformation dungeon)
    {
        if(onDungeonLoad != null)
        {
            onDungeonLoad(dungeon);
        }
        else
        {
            Debug.Log("Event null");
        }
    }
}