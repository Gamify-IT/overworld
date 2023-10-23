using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkardsWalkGenerator : LayoutGenerator
{
    public DrunkardsWalkGenerator(
        Vector2Int size,
        float accessability,
        List<WorldConnection> worldConnections)
        : base(size, accessability, worldConnections) { }

    public DrunkardsWalkGenerator(
        Vector2Int size,
        float accessability)
        : base(size, accessability) { }

    public override void GenerateLayout()
    {
        Debug.Log("Drunkard's walk layout generator");
        //DW implementation
    }
}
