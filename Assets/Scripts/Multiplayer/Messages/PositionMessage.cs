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

    public PositionMessage(string playerId, Vector2 position, Vector2 movement)
    {
        this.playerId = playerId;
        this.position = position;  
        this.movement = movement;
    }

    public override byte[] Serialize()
    {
        int index = 0;
        byte[] data = new byte[33];

        // message type (1 Byte)
        data[index] = (byte) Type;
        index++;

        // playerId (GUID, 16 Bytes)
        Buffer.BlockCopy(Guid.Parse(playerId).ToByteArray(), 0, data, index, 16);
        index += 16;

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

    public static new PositionMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // playerId (GUID, 16 Byte)
        string playerId = DeserializePlayerId(data, ref index);

        // position (2 * 4 Byte)
        float posX = BitConverter.ToSingle(data, index);
        index += 4;
        float posY = BitConverter.ToSingle(data, index);
        index += 4;

        // movement (2 * 4 Byte)
        float moveX = BitConverter.ToSingle(data, index);
        index += 4;
        float moveY = BitConverter.ToSingle(data, index);

        return new PositionMessage(playerId, new Vector2(posX, posY), new Vector2(moveX, moveY));
    }

    public Vector2 GetPosition() { return position; }
    public Vector2 GetMovement() { return movement; }
}