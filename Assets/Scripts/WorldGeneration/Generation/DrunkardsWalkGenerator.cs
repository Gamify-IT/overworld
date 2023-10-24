using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkardsWalkGenerator : LayoutGenerator
{
    public DrunkardsWalkGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(seed, size, accessability, worldConnections) { }

    public DrunkardsWalkGenerator(
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections)
        : base(size, accessability, worldConnections) { }

    public DrunkardsWalkGenerator(
        string seed,
        Vector2Int size,
        int accessability)
        : base(seed, size, accessability) { }

    public DrunkardsWalkGenerator(
        Vector2Int size,
        int accessability)
        : base(size, accessability) { }

    public override void GenerateLayout()
    {
        Debug.Log("Drunkard's walk layout generator");
        //DW implementation
    }
}
