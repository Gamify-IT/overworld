using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCData
{
    #region Attributes
    private string uuid;
    private string[] dialogue;
    private bool hasBeenTalkedTo;
    #endregion

    public NPCData(string uuid,  string[] dialogue, bool hasBeenTalkedTo)
    {
        this.uuid = uuid;
        this.dialogue = dialogue;
        this.hasBeenTalkedTo = hasBeenTalkedTo;
    }

    #region GetterAndSetter
    public string getUUID()
    {
        return uuid;
    }

    public void setUUID(string uuid)
    {
        this.uuid = uuid;
    }
    
    public string[] getDialogue()
    {
        return dialogue;
    }

    public void setDialogue(string[] dialogue)
    {
        this.dialogue = dialogue;
    }

    public bool getHasBeenTalkedTo()
    {
        return hasBeenTalkedTo;
    }

    public void setHasBeenTalkedTo(bool hasBeenTalkedTo)
    {
        this.hasBeenTalkedTo = hasBeenTalkedTo;
    }
    #endregion
}
