using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private static readonly int moveStraightCost = 10;
    private static readonly int moveDiagonalCost = 14;

    private PathNode[,] grid;
    private Vector2Int size;

    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinder(bool[,] accessableTiles)
    {
        size = new Vector2Int(accessableTiles.GetLength(0), accessableTiles.GetLength(1));
        grid = new PathNode[size.x, size.y];

        InitGrid(accessableTiles);
    }

    /// <summary>
    ///     This function initialized the grid cells with the correct path nodes
    /// </summary>
    /// <param name="accessableTiles">Accessability of each cell</param>
    private void InitGrid(bool[,] accessableTiles)
    {
        for(int x=0; x < size.x; x++)
        {
            for(int y=0; y < size.y; y++)
            {
                grid[x, y] = new PathNode(new Vector2Int(x, y), accessableTiles[x, y]);
            }
        }
    }

    /// <summary>
    ///     This funciton calculates the shortest path from the given start positon to the end position
    /// </summary>
    /// <param name="startPos">The start position</param>
    /// <param name="endPos">The end position</param>
    /// <returns>An optional containg the path, if found, an emtpy optional, if no path could be found</returns>
    public Optional<List<Vector2Int>> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        Optional<List<Vector2Int>> path = new Optional<List<Vector2Int>>();

        bool success = SearchPath(startPos, endPos);
        if (success)
        {
            PathNode endNode = grid[endPos.x, endPos.y];
            path.SetValue(ReconstructPath(endNode));
        }

        return path; 
    }

    /// <summary>
    ///     This function runs the a star search
    /// </summary>
    /// <param name="startPos">The position to start from</param>
    /// <param name="endPos">The position to search a path to</param>
    /// <returns>True, if a path was found, false otherwise</returns>
    private bool SearchPath(Vector2Int startPos, Vector2Int endPos)
    {
        PathNode startNode = grid[startPos.x, startPos.y];
        PathNode endNode = grid[endPos.x, endPos.y];

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        //clear values from earlier runs
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[x, y].ResetValues();
            }
        }

        //init start node
        startNode.gCost = 0;
        startNode.hCost = CalculateHeurisiticCost(startPos, endPos);
        startNode.CalculateFCost();

        //loop
        while(openList.Count > 0)
        {
            PathNode currentNode = GetNextNode();
            if(currentNode == endNode)
            {
                //path found
                return true;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighbors(currentNode))
            {
                if(closedList.Contains(neighborNode))
                {
                    continue;
                }
                if(!neighborNode.accessable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateHeurisiticCost(currentNode.position, neighborNode.position);
                if(tentativeGCost < neighborNode.gCost)
                {
                    //better path to neighbor found
                    neighborNode.parent = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateHeurisiticCost(neighborNode.position, endNode.position);
                    neighborNode.CalculateFCost();

                    if(!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //no path found
        return false;

    }

    /// <summary>
    ///     This function calculates the heuristic (optimal) cost between two positions
    /// </summary>
    /// <param name="from">The first position</param>
    /// <param name="to">The second positon</param>
    /// <returns>The heuristic cost between the two given positions</returns>
    private int CalculateHeurisiticCost(Vector2Int from, Vector2Int to)
    {
        int deltaX = Mathf.Abs(from.x - to.x);
        int deltaY = Mathf.Abs(from.y - to.y);
        int remaining = Mathf.Abs(deltaX - deltaY);
        return moveDiagonalCost * Mathf.Min(deltaX, deltaY) + moveStraightCost * remaining;
    }

    /// <summary>
    ///     This function returns the next node to explore
    /// </summary>
    /// <returns>The next node with the lowest f cost</returns>
    private PathNode GetNextNode()
    {
        PathNode nextNode = openList[0];
        for(int i = 1; i < openList.Count; i++)
        {
            if(openList[i].fCost < nextNode.fCost)
            {
                nextNode = openList[i];
            }
        }
        return nextNode;
    }

    /// <summary>
    ///     This function gets all neighbors of the given node
    /// </summary>
    /// <param name="currentNode">The node to get the neighbors from</param>
    /// <returns>A list containing all valid neighbors</returns>
    private List<PathNode> GetNeighbors(PathNode currentNode)
    {
        List<PathNode> neighbors = new List<PathNode>();

        int posX = currentNode.position.x;
        int posY = currentNode.position.y;

        for(int x = posX - 1; x <= posX + 1; x++)
        {
            for(int y = posY - 1; y <= posY + 1; y++)
            {
                if(x == posX && y == posY)
                {
                    //center
                    continue;
                }

                if(IsInRange(x, y))
                {
                    neighbors.Add(grid[x, y]);
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    ///     This function reconstructs the found path 
    /// </summary>
    /// <param name="endNode">The end node</param>
    /// <returns>A list containing all steps</returns>
    private List<Vector2Int> ReconstructPath(PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(endNode.position);
        PathNode currentNode = endNode;
        while(currentNode.parent != null)
        {
            currentNode = currentNode.parent;
            path.Add(currentNode.position);
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    ///     This function checks, whether a given position is inside of the layout size
    /// </summary>
    /// <param name="posX">The x coordinate</param>
    /// <param name="posY">The y coordinate</param>
    /// <returns>True, if in range, false otherwise</returns>
    private bool IsInRange(int posX, int posY)
    {
        return (posX >= 0 && posX < size.x && posY >= 0 && posY < size.y);
    }

    /// <summary>
    ///     This function returns the path cost for a given path
    /// </summary>
    /// <param name="path">The path the get the cost from</param>
    /// <returns>The cost of the path</returns>
    public int GetDistance(List<Vector2Int> path)
    {
        if(path.Count < 2)
        {
            return 0;
        }

        int distance = 0;

        Vector2Int currentPosition = path[0];
        Vector2Int nextPosition = path[1];

        int stepCost = GetStepCost(currentPosition, nextPosition);
        distance += stepCost;

        while (path.IndexOf(nextPosition) < path.Count - 1)
        {
            stepCost = GetStepCost(currentPosition, nextPosition);
            distance += stepCost;

            currentPosition = nextPosition;
            nextPosition = path[path.IndexOf(nextPosition) + 1];
        }

        return Mathf.RoundToInt(distance / 10);
    }

    /// <summary>
    ///     This function calculates the step cost between two given positions
    /// </summary>
    /// <param name="from">The first position</param>
    /// <param name="to">The second position</param>
    /// <returns>The cost to move from one position to the other</returns>
    private int GetStepCost(Vector2Int from, Vector2Int to)
    {
        if(from.x == to.x || from.y == to.y)
        {
            //direct neighbor -> straight travel cost
            return moveStraightCost;
        }
        else
        {
            //diagonal neighbor -> diagonal travel cost
            return moveDiagonalCost;
        }
    }

}
