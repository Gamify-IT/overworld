using System;

/// <summary>
///     Abstract class for a network message used in the communication protocol with the multiplayer server.
/// </summary>
public abstract class NetworkMessage
{
    protected abstract MessageType Type { get; }
    public string playerId;

    /// <summary>
    ///     Serializes a message to a byte array.
    /// </summary>
    /// <returns>byte array of the message</returns>
    public abstract byte[] Serialize();

    /// <summary>
    ///     Deserializes the player id of a message.
    /// </summary>
    /// <param name="data">received data</param>
    /// <param name="index">current pointer in the byte array</param>
    /// <returns>id of the player</returns>
    protected static string DeserializePlayerId(byte[] data, ref int index)
    {
        byte[] playerIdBytes = new byte[16];
        Buffer.BlockCopy(data, index, playerIdBytes, 0, 16);
        index += 16;
        return new Guid(playerIdBytes).ToString();
    }

    /// <summary>
    ///     Deserializes a received byte array to a message.
    /// </summary>
    /// <param name="data">received data</param>
    /// <returns>message depending on the received data</returns>
    /// <exception cref="Exception">if message type is unknown</exception>
    public static NetworkMessage Deserialize(ref byte[] data)
    {
        MessageType type = (MessageType)data[0];

        return type switch
        {
            MessageType.Position => PositionMessage.Deserialize(ref data),
            MessageType.Character => CharacterMessage.Deserialize(ref data),
            _ => throw new Exception("Unknown message type!"),
        };
    }

    public string GetPlayerId() { return playerId; }
}