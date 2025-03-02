using System;
using UnityEngine;

/// <summary>
///     Network message type for position data, i.e. position and moevemtn vector of a player.
/// </summary>
public class PositionMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Position;
    private readonly Vector2 position;
    private readonly Vector2 movement;

    public PositionMessage(ushort clientId, Vector2 position, Vector2 movement) : base(clientId)
    {
        this.position = position;  
        this.movement = movement;
    }

    public override byte[] Serialize()
    {
        int index = 0;
        byte[] data = new byte[19]; // 1 + 2 + 8 + 8 = 19

        // message type (1 Byte)
        data[index] = (byte) Type;
        index++;

        // clientId (2 Byte)
        data[index] = (byte)(clientId & 0xFF);
        data[index + 1] = (byte)((clientId >> 8) & 0xFF);
        index += 2;

        // position (2 * 4 Bytes)
        Buffer.BlockCopy(BitConverter.GetBytes(position.x), 0, data, index, 4);
        index += 4;
        Buffer.BlockCopy(BitConverter.GetBytes(position.y), 0, data, index, 4);
        index += 4;

        // movement (2 * 4 Byte)
        Buffer.BlockCopy(BitConverter.GetBytes(movement.x), 0, data, index, 4);
        index += 4;
        Buffer.BlockCopy(BitConverter.GetBytes(movement.y), 0, data, index, 4);

        return data;
    }

    /// <summary>
    ///     Deserializes the recived data and converts it to a position message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>position message</returns>
    public static new PositionMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // clientId (2 Byte)
        ushort clientId = (ushort)(data[index] | (data[index + 1] << 8));
        index += 2;

        // position (2 * 4 Byte)
        float posX = BitConverter.ToSingle(data, index);
        index += 4;
        float posY = BitConverter.ToSingle(data, index);
        index += 4;

        // movement (2 * 4 Byte)
        float moveX = BitConverter.ToSingle(data, index);
        index += 4;
        float moveY = BitConverter.ToSingle(data, index);

        return new PositionMessage(clientId, new Vector2(posX, posY), new Vector2(moveX, moveY));
    }

    public Vector2 GetPosition() { return position; }
    public Vector2 GetMovement() { return movement; }
}