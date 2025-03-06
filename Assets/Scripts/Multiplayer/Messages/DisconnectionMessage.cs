using System.IO;

/// <summary>
///     Message type for disconnecting from the multiplayer.
/// </summary>
public class DisconnectionMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Disconnection;
    private static readonly byte size = 3; // 1 + 2 = 3 Byte

    public DisconnectionMessage(ushort clientId) : base(clientId) { }

    public override byte[] Serialize()
    {
        int index = 0;
        byte [] data = new byte[size];

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // clientId (2 Byte)
        data[index] = (byte)(clientId & 0xFF);
        data[index + 1] = (byte)((clientId >> 8) & 0xFF);

        return data;
    }

    /// <summary>
    ///     Deserializes the received data and converts it to a disconnection message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>disconnect message</returns>
    public static new DisconnectionMessage Deserialize(ref byte[] data)
    {
        if (data.Length != size)
        {
            throw new InvalidDataException("Wrong message size");
        }

        // skip message type (1 Byte)
        int index = 1;

        // clientId (2 Byte)
        ushort clientId = (ushort)(data[index] | (data[index + 1] << 8));

        return new DisconnectionMessage(clientId);
    }
}