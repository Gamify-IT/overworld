using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDTO
{
    public string id;
    public AreaLocationDTO area;
    public int index;
    public List<string> text;

    public NPCDTO(string id, AreaLocationDTO area, int index, List<string> text)
    {
        this.id = id;
        this.area = area;
        this.index = index;
        this.text = text;
    }

    public NPCDTO() { }

    public static NPCDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NPCDTO>(jsonString);
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

    public List<string> getText()
    {
        return text;
    }

    public void setText(List<string> text)
    {
        this.text = text;
    }
    #endregion
}
