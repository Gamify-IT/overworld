using System.IO;

/// <summary>
///     Network message for the area information of the player, i.e. world and dungeon index.
/// </summary>
public class AreaMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Area;
    private static readonly byte size = 5; // 1 + 2 + 2 = 5 Byte
    private readonly byte worldIndex;
    private readonly byte dungeonIndex;

    public AreaMessage(ushort clientId, byte worldIndex, byte dungeonIndex) : base(clientId)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
    }

    public override byte[] Serialize()
    {
        int index = 0;
        byte [] data = new byte[size];

        // message type
        data[0] = (byte)Type;
        index++;

        // clientId (2 Byte)
        data[index] = (byte)(clientId & 0xFF);
        data[index + 1] = (byte)((clientId >> 8) & 0xFF);
        index += 2;

        // world and dungeon index (2 * 1 Byte)
        data[index] = worldIndex;
        index++;
        data[index] = dungeonIndex;

        return data;
    }

    /// <summary>
    ///     Deserializes the received data and converts it to a area message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>area information message</returns>
    public static new AreaMessage Deserialize(ref byte[] data)
    {
        if (data.Length != size)
        {
            throw new InvalidDataException("Wrong message size");
        }

        // skip message type (1 Byte)
        int index = 1;

        // clientId (2 Byte)
        ushort clientId = (ushort)(data[index] | (data[index + 1] << 8));
        index += 2;

        // world and dungeon index (2 * 1 Byte)
        byte worldIndex = data[index];
        index++;
        byte dungenIndex = data[index];

        return new AreaMessage(clientId, worldIndex, dungenIndex);
    }

    public byte GetWorldIndex() {  return worldIndex; }
    public byte GetDungeonIndex() { return dungeonIndex; }
}