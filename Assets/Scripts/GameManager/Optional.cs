using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Optional<T>
{
    private bool enabled;
    private T value;

    public Optional(T initialValue)
    {
        enabled = true;
        value = initialValue;
    }

    public bool Enabled()
    {
        return enabled;
    }

    public T Value()
    {
        return value;
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    public void SetValue(T value)
    {
        this.value = value;
    }
}
