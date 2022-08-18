using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerTaskStatisticDTO
{
    public string id;
    public int highscore;
    public bool completed;
    public MinigameTaskDTO minigameTask;

    public PlayerTaskStatisticDTO(string id, int highscore, bool completed, MinigameTaskDTO minigameTask)
    {
        this.id = id;
        this.highscore = highscore;
        this.completed = completed;
        this.minigameTask = minigameTask;
    }

    public PlayerTaskStatisticDTO() { }

    public static PlayerTaskStatisticDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerTaskStatisticDTO>(jsonString);
    }
}
