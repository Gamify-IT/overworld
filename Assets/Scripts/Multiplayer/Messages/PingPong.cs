/// <summary>
///     Message type for chec if connetion between client and server is still open.
/// </summary>
public class PingPongMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.PingPong;

    public PingPongMessage(byte playerId) : base(playerId) { }

    public override byte[] Serialize()
    {
        int index = 0;
        byte[] data = new byte[1 + 1];

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // playerId (1 Byte)
        data[index] = playerId;

        return data;
    }

    /// <summary>
    ///     Deserializes the received data and converts it to a ping pong message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>ping pong message</returns>
    public static new PingPongMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // playerId (1 Byte)
        byte playerId = data[index];

        return new PingPongMessage(playerId);
    }
}