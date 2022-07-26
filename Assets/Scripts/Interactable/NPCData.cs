using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCData
{
    #region Attributes
    private string[] dialogue;
    #endregion

    public NPCData(string[] dialogue)
    {
        this.dialogue = dialogue;
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
    #endregion
}
