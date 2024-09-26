using System;

/// <summary>
/// IntTuple is a helper class for storing the unique identifier of objects in the game (e.g. book, NPC, etc.).
///
/// The unique identifier consists of 3 int values, namely:
///   -worldId - indicates the number of the world where the object is located,
///   -dungeonId - the number of the dungeon where the object is located,
///   -numberId - the serial number of the object.
/// </summary>
[Serializable]
public class IntTuple
{
    public int worldId;
    public int dungeonId;
    public int numberId;

    public IntTuple(int worldId, int dungeonId, int numberId)
    {
        this.worldId = worldId;
        this.dungeonId = dungeonId;
        this.numberId = numberId;
    }
}
