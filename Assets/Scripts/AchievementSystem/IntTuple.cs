using System;

/// <summary>
///     The <c>IntTuple</c> is needed to store tuples consisting of worldId, dungeonId and object number 
///     so that there are no incompatibility conflicts between frontend and backend
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
