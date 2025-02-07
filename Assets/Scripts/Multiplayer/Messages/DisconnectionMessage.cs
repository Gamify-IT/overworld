using System;
using UnityEngine;

/// <summary>
///     Message type for disconnecting from the multiplayer.
/// </summary>
public class DisconnectionMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Disconnect;

    public DisconnectionMessage(string playerId) : base(playerId) { }

    public override byte[] Serialize()
    {
        int index = 0;
        byte [] data = new byte[1 + 16];

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // playerId (GUID, 16 Byte)
        Buffer.BlockCopy(Guid.Parse(playerId).ToByteArray(), 0, data, index, 16);

        return data;
    }

    /// <summary>
    ///     Deserializes the recived data and converts it to a disconnection message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>disconnect message</returns>
    public static new DisconnectionMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // playerId (GUID, 16 Byte)
        string playerId = DeserializePlayerId(data, ref index);

        return new DisconnectionMessage(playerId);
    }
}