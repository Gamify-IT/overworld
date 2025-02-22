using System;

/// <summary>
///     Abstract class for a network message used in the communication protocol with the multiplayer server.
/// </summary>
public abstract class NetworkMessage
{
    protected abstract MessageType Type { get; }
    protected readonly byte playerId;

    public NetworkMessage(byte playerId)
    {
        this.playerId = playerId;
    }

    /// <summary>
    ///     Serializes a message to a byte array.
    /// </summary>
    /// <returns>byte array of the message</returns>
    public abstract byte[] Serialize();

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
            MessageType.Connect => ConnectionMessage.Deserialize(ref data),
            MessageType.Disconnect => DisconnectionMessage.Deserialize(ref data),
            MessageType.Acknowledge => AcknowledgeMessage.Deserialize(ref data),
            MessageType.Position => PositionMessage.Deserialize(ref data),
            MessageType.Character => CharacterMessage.Deserialize(ref data),
            MessageType.AreaInformation => AreaMessage.Deserialize(ref data),
            _ => throw new Exception("Unknown message type!"),
        };
    }

    public byte GetPlayerId() { return playerId; }
    public MessageType GetMessageType() {  return Type; }  
}