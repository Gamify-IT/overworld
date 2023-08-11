using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to serialize position data.
/// </summary>
[Serializable]
public class Position
{

    public static Position CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Position>(jsonString);
    }

    public float x;
    public float y;

    public Position(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Position()
    {
    }
}
