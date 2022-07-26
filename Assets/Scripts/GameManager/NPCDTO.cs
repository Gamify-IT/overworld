using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDTO
{
    private string id;
    private int index;
    private string text;

    public NPCDTO(string id, int index, string text)
    {
        this.id = id;
        this.index = index;
        this.text = text;
    }

    #region GetterAndSetter
    public string getId()
    {
        return id;
    }

    public void setId(string id)
    {
        this.id = id;
    }

    public int getIndex()
    {
        return index;
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    public string getText()
    {
        return text;
    }

    public void setText(string text)
    {
        this.text = text;
    }
    #endregion
}
