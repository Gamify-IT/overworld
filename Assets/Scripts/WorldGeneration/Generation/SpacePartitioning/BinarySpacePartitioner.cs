using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines an implementation of a binary space partitioner, used by the <c>IslandsGenerator</c>
/// </summary>
public class BinarySpacePartitioner : SpacePartitioner
{
    private int minWidth;
    private int minHeight;
    private System.Random pseudoRandomNumberGenerator;

    public BinarySpacePartitioner(string seed, int minWidth, int minHeight) : base()
    {
        this.minWidth = minWidth;
        this.minHeight = minHeight;
        pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());
    }

    /// <summary>
    ///     This function partitions the given space
    /// </summary>
    /// <param name="spaceToPartition">The space to be partitioned</param>
    /// <returns>A list containing all created subspaces</returns>
    public override List<Subspace> Partition(Subspace spaceToPartition)
    {
        return BinarySplit(spaceToPartition);
    }

    /// <summary>
    ///     This function recursively splits the the given space in two subspaces, until no subspaces can be split anymore
    /// </summary>
    /// <param name="spaceToSplit">The space to split</param>
    /// <returns>A list containing all the created subspaces</returns>
    private List<Subspace> BinarySplit(Subspace spaceToSplit)
    {
        List<Subspace> spaces = new List<Subspace>();

        Queue<Subspace> spaceQueue = new Queue<Subspace>();
        spaceQueue.Enqueue(spaceToSplit);

        while(spaceQueue.Count > 0)
        {
            Subspace space = spaceQueue.Dequeue();
            if(space.size.x >= minWidth && space.size.y >= minHeight)
            {
                int split = pseudoRandomNumberGenerator.Next(0, 2);
                if(split == 0)
                {
                    //try vertical split first
                    if(space.size.x >= 2 * minWidth)
                    {
                        //split vertical
                        List<Subspace> subspaces = SplitVertically(space);
                        foreach(Subspace subspace in subspaces)
                        {
                            spaceQueue.Enqueue(subspace);
                        }
                    }
                    else if(space.size.y >= 2 * minHeight)
                    {
                        //split horizontally
                        List<Subspace> subspaces = SplitHorizontally(space);
                        foreach (Subspace subspace in subspaces)
                        {
                            spaceQueue.Enqueue(subspace);
                        }
                    }
                    else
                    {
                        //space cannot be split anymore
                        spaces.Add(space);
                    }
                }
                else
                {
                    //try horizontal split first
                    if (space.size.y >= 2 * minHeight)
                    {
                        //split horizontally
                        List<Subspace> subspaces = SplitHorizontally(space);
                        foreach (Subspace subspace in subspaces)
                        {
                            spaceQueue.Enqueue(subspace);
                        }
                    }
                    else if (space.size.x >= 2 * minWidth)
                    {
                        //split vertical
                        List<Subspace> subspaces = SplitVertically(space);
                        foreach (Subspace subspace in subspaces)
                        {
                            spaceQueue.Enqueue(subspace);
                        }
                    }
                    else
                    {
                        //space cannot be split anymore
                        spaces.Add(space);
                    }
                }
            }
        }

        return spaces;
    }

    /// <summary>
    ///    This function splits the given space horizontally
    /// </summary>
    /// <param name="space">The space to split</param>
    /// <returns>A list containing the two created subspaces</returns>
    private List<Subspace> SplitVertically(Subspace space)
    {
        int xSplit = pseudoRandomNumberGenerator.Next(1, space.size.x);

        Vector2Int subspace1Size = new Vector2Int(xSplit, space.size.y);
        Subspace subspace1 = new Subspace(space.offset, subspace1Size);

        Vector2Int subspace2Offset = new Vector2Int(space.offset.x + xSplit, space.offset.y);
        Vector2Int subspace2Size = new Vector2Int(space.size.x - xSplit, space.size.y);
        Subspace subspace2 = new Subspace(subspace2Offset, subspace2Size);

        return new List<Subspace>() { subspace1, subspace2 };
    }

    /// <summary>
    ///    This function splits the given space vertically
    /// </summary>
    /// <param name="space">The space to split</param>
    /// <returns>A list containing the two created subspaces</returns>
    private List<Subspace> SplitHorizontally(Subspace space)
    {
        int ySplit = pseudoRandomNumberGenerator.Next(1, space.size.y);

        Vector2Int subspace1Size = new Vector2Int(space.size.x, ySplit);
        Subspace subspace1 = new Subspace(space.offset, subspace1Size);

        Vector2Int subspace2Offset = new Vector2Int(space.offset.x, space.offset.y + ySplit);
        Vector2Int subspace2Size = new Vector2Int(space.size.x, space.size.y - ySplit);
        Subspace subspace2 = new Subspace(subspace2Offset, subspace2Size);

        return new List<Subspace>() { subspace1, subspace2 };
    }
}
