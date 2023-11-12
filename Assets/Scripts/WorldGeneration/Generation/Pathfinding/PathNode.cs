using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector2Int position;
    public bool accessable;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode parent;

    public PathNode(Vector2Int position, bool accessable)
    {
        this.position = position;
        this.accessable = accessable;

        ResetValues();

        parent = null;
    }

    public void ResetValues()
    {
        parent = null;
        gCost = int.MaxValue;
        hCost = 0;
        CalculateFCost();
    }

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }
}
