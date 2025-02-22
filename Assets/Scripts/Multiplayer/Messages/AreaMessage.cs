using System;

/// <summary>
///     Network message for the area information of the player, i.e. world and dungeon index.
/// </summary>
public class AreaMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.AreaInformation;
    private readonly byte worldIndex;
    private readonly byte dungeonIndex;

    public AreaMessage(byte playerId, byte worldIndex, byte dungeonIndex) : base(playerId)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
    }

    public override byte[] Serialize()
    {
        int index = 0;
        byte [] data = new byte[1 + 1 + 2];

        // message type
        data[0] = (byte)Type;
        index++;

        // playerId (1 Bytes)
        data[index] = playerId;
        index++;

        // world and dungeon index (2 * 1 Byte)
        data[index] = worldIndex;
        index++;
        data[index] = dungeonIndex;

        return data;
    }

    /// <summary>
    ///     Deserializes the recived data and converts it to a area message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>area information message</returns>
    public static new AreaMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // playerId (1 Byte)
        byte playerId = data[index];
        index++;

        // world and dungeon index (2 * 1 Byte)
        byte worldIndex = data[index];
        index++;
        byte dungenIndex = data[index];

        return new AreaMessage(playerId, worldIndex, dungenIndex);
    }

    public byte GetWorldIndex() {  return worldIndex; }
    public byte GetDungeonIndex() { return dungeonIndex; }
}