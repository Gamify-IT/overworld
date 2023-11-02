using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines an Space Partitioner. It is used to inherit for specific partitioner implementation.
/// </summary>
public abstract class SpacePartitioner
{
    protected SpacePartitioner() { }

    /// <summary>
    ///     This function partitions the given space 
    /// </summary>
    /// <param name="spaceToPartition">The space to be partitioned</param>
    /// <returns>A list containing all created subspaces</returns>
    public abstract List<Subspace> Partition(Subspace spaceToPartition);
}
