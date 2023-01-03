using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
