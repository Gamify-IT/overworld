using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaLocationDTO
{
    public int worldIndex;
    public int dungeonIndex;

    public AreaLocationDTO(int worldIndex, int dungeonIndex)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
    }

    public AreaLocationDTO() { }

    public static AreaLocationDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AreaLocationDTO>(jsonString);
    }
}
