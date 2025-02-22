using System;
using UnityEngine;

/// <summary>
///     Message type for disconnecting from the multiplayer.
/// </summary>
public class DisconnectionMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Disconnect;

    public DisconnectionMessage(byte playerId) : base(playerId) { }

    public override byte[] Serialize()
    {
        int index = 0;
        byte [] data = new byte[1 + 1];

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // playerId (1 Byte)
        data[index] = playerId;

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

        // playerId (1 Byte)
        byte playerId = data[index];

        return new DisconnectionMessage(playerId);
    }
}