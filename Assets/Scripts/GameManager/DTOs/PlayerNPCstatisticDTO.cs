using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to retrieve data from Get Requests.
/// </summary>
[System.Serializable]
public class PlayerNPCStatisticDTO
{
    #region Attributes
    public string id;
    public bool completed;
    public NPCDTO npc;
    #endregion
}
