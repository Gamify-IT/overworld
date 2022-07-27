using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCTalkEvent
{
    public string npcId;
    public bool completed;
    public string userId;

    public NPCTalkEvent(string npcId, bool completed, string userId)
    {
        this.npcId = npcId;
        this.completed = completed;
        this.userId = userId;
    }
}
