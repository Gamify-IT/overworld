using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerstatisticDTO
{
    public string id;
    public AreaLocationDTO[] unlockedAreas;
    public AreaLocationDTO[] unlockedDungeons;
    public AreaLocationDTO currentArea;
    public string userId;
    public string username;
    public int knowledge;

    public PlayerstatisticDTO(string id, AreaLocationDTO[] unlockedAreas, AreaLocationDTO[] unlockedDungeons, AreaLocationDTO currentArea, string userId, string username, int knowledge)
    {
        this.id = id;
        this.unlockedAreas = unlockedAreas;
        this.unlockedDungeons = unlockedDungeons;
        this.currentArea = currentArea;
        this.userId = userId;
        this.username = username;
        this.knowledge = knowledge;
    }

    public PlayerstatisticDTO() { }

    public static PlayerstatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerstatisticDTO>(jsonString);
    }
}
