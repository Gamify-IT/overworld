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
    public string teleporterID;
    public bool completed;
    public string userId;
    #endregion

    #region Constructor
    public TeleporterUnlockedEvent(string teleporterID, bool unlocked, string userId)
    {
        this.teleporterID = teleporterID;
        this.completed = unlocked;
        this.userId = userId;
    }
    #endregion
}
