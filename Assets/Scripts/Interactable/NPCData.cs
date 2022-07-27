using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCData
{
    #region Attributes
    private string[] dialogue;
    private bool hasBeenTalkedTo;
    #endregion

    public NPCData(string[] dialogue, bool hasBeenTalkedTo)
    {
        this.dialogue = dialogue;
        this.hasBeenTalkedTo = hasBeenTalkedTo;
    }

    #region GetterAndSetter
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
