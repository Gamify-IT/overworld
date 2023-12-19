using UnityEngine;

/// <summary>
///     This class represents a node in the graph of the a* pathfinder
/// </summary>
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

    /// <summary>
    ///     This function resets all stored values to the inital states
    /// </summary>
    public void ResetValues()
    {
        parent = null;
        gCost = int.MaxValue;
        hCost = 0;
        CalculateFCost();
    }

    /// <summary>
    ///     This function calculates the expected cost based on the cost to get to this node and the heuristic for the rest of the way
    /// </summary>
    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }
}
