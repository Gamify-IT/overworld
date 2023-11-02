using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
