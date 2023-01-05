using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines all needed data for NPC interactions.
/// </summary>
[System.Serializable]
public class TeleporterUnlockedEvent
{
    #region Attributes
    public AreaLocationDTO area;
    public int index;
    public string userId;
    #endregion

    #region Constructor
    public TeleporterUnlockedEvent(int worldID, int dungeonID, int index, string userId)
    {
        this.area = new AreaLocationDTO(worldID, dungeonID);
        this.index = index;
        this.userId = userId;
    }
    #endregion
}
