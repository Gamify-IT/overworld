using UnityEngine;

/// <summary>
///     This class contains the data of a subspace and is used in the space partitioning process of <c>SpacePartitioner</c> classes
/// </summary>
public class Subspace
{
    public Vector2Int offset;
    public Vector2Int size;

    public Subspace(Vector2Int offset, Vector2Int size)
    {
        this.offset = offset;
        this.size = size;
    }
}
