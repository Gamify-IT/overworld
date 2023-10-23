using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsGenerator : LayoutGenerator
{
    private RoomGenerator roomGenerator;

    public IslandsGenerator(
        Vector2Int size,
        float accessability,
        List<WorldConnection> worldConnections,
        RoomGenerator roomGenerator)
        : base(size, accessability, worldConnections) 
    {
        this.roomGenerator = roomGenerator;
    }

    public IslandsGenerator(
        Vector2Int size,
        float accessability,
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
