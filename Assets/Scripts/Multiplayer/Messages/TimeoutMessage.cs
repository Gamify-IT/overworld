using System.IO;

/// <summary>
///     Message type for signaling the clients inactivity.
/// </summary>
public class TimeoutMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Timeout;
    private static readonly byte size = 3; // 1 + 2 = 3 Byte

    public TimeoutMessage(ushort playerId) : base(playerId) { }

    public override byte[] Serialize()
    {
        int index = 0;
        byte[] data = new byte[size];

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // clientId (2 Byte)
        data[index] = (byte)(clientId & 0xFF);
        data[index + 1] = (byte)((clientId >> 8) & 0xFF);

        return data;
    }

    /// <summary>
    ///     Deserializes the received data and converts it to a timeout message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>ping pong message</returns>
    public static new TimeoutMessage Deserialize(ref byte[] data)
    {
        if (data.Length != size)
        {
            throw new InvalidDataException("Wrong message size");
        }

        // skip message type (1 Byte)
        int index = 1;

        // clientId (2 Byte)
        ushort clientId = (ushort)(data[index] | (data[index + 1] << 8));

        return new TimeoutMessage(clientId);
    }
}