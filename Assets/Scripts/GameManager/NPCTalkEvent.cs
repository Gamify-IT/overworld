using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines all needed data for NPC interactions.
/// </summary>
[System.Serializable]
public class NPCTalkEvent
{
    #region Attributes
    public string npcId;
    public bool completed;
    public string userId;
    #endregion

    #region Constructor
    public NPCTalkEvent(string npcId, bool completed, string userId)
    {
        this.npcId = npcId;
        this.completed = completed;
        this.userId = userId;
    }
    #endregion
}
