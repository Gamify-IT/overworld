using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsGenerator : LayoutGenerator
{
    private RoomGenerator roomGenerator;

    public IslandsGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections,
        RoomGenerator roomGenerator)
        : base(seed, size, accessability, worldConnections) 
    {
        this.roomGenerator = roomGenerator;
    }

    public IslandsGenerator(
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections,
        RoomGenerator roomGenerator)
        : base(size, accessability, worldConnections)
    {
        this.roomGenerator = roomGenerator;
    }

    public IslandsGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        RoomGenerator roomGenerator)
        : base(seed, size, accessability)
    {
        this.roomGenerator = roomGenerator;
    }

    public IslandsGenerator(
        Vector2Int size,
        int accessability,
        RoomGenerator roomGenerator)
        : base(size, accessability)
    {
        this.roomGenerator = roomGenerator;
    }

    public override void GenerateLayout()
    {
        Debug.Log("Island layout generator with rooms: " + roomGenerator.ToString());
        //Islands implementation
    }
}
