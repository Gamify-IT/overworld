using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerTaskStatisticDTOArray
{
    public PlayerTaskStatisticDTO[] playerTaskStatisticDTOs;

    public PlayerTaskStatisticDTOArray(PlayerTaskStatisticDTO[] playerTaskStatisticDTOs)
    {
        this.playerTaskStatisticDTOs = playerTaskStatisticDTOs;
    }

    public PlayerTaskStatisticDTOArray() { }

    public static PlayerTaskStatisticDTOArray CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerTaskStatisticDTOArray>(jsonString);
    }
}
