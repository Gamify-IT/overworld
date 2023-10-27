using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleRoomConnection
{
    private Room startRoom;
    private Room destinationRoom;
    private Vector2Int startTile;
    private Vector2Int destinationTile;
    private float distance;

    public PossibleRoomConnection(Room startRoom, Room destinationRoom, Vector2Int startTile, Vector2Int destinationTile, float distance)
    {
        this.startRoom = startRoom;
        this.destinationRoom = destinationRoom;
        this.startTile = startTile;
        this.destinationTile = destinationTile;
        this.distance = distance;
    }

    public Room GetStartRoom()
    {
        return startRoom;
    }

    public Room GetDestinationRoom()
    {
        return destinationRoom;
    }

    public Vector2Int GetStartTile()
    {
        return startTile;
    }

    public Vector2Int GetDestinationTile()
    {
        return destinationTile;
    }

    public float GetDistance()
    {
        return distance;
    }
}
