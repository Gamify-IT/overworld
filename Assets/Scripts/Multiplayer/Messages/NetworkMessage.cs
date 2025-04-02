using System;

/// <summary>
///     Abstract class for a network message used in the communication protocol with the multiplayer server.
/// </summary>
public abstract class NetworkMessage
{
    protected abstract MessageType Type { get; }
    protected readonly ushort clientId;

    public NetworkMessage(ushort clientId)
    {
        this.clientId = clientId;
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
        try
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Network message null or empty");
            }

            MessageType type = (MessageType)data[0];

            return type switch
            {
                MessageType.Connection => ConnectionMessage.Deserialize(ref data),
                MessageType.Disconnection => DisconnectionMessage.Deserialize(ref data),
                MessageType.Acknowledge => AcknowledgeMessage.Deserialize(ref data),
                MessageType.Position => PositionMessage.Deserialize(ref data),
                MessageType.Character => CharacterMessage.Deserialize(ref data),
                MessageType.Area => AreaMessage.Deserialize(ref data),
                MessageType.Timeout => TimeoutMessage.Deserialize(ref data),
                _ => throw new Exception("Unknown message type!"),
            };
        }
        catch (Exception e)
        {
            throw new Exception($"Error on deserializing network message: {e.Message}");
        }
    }

    public ushort GetClientId() { return clientId; }
    public MessageType GetMessageType() {  return Type; }  
}