using System;
using System.Text;
using UnityEngine;

/// <summary>
///     Message type for the first message when connecting to the server.
///     Consists of the player's current position and character outfit.
/// </summary>
public class ConnectionMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Connect;
    private readonly Vector2 startPosition;
    private readonly byte worldIndex;
    private readonly byte dungeonIndex;
    private readonly string head;
    private readonly string body;

    public ConnectionMessage(byte playerId, Vector2 startPosition, byte worldIndex, byte dungeonIndex, string head, string body) : base(playerId)
    {
        this.startPosition = startPosition;
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.head = body;
        this.body = head;
    }

    public override byte[] Serialize()
    {
        int index = 0;

        // compute array size
        int headLength = Encoding.UTF8.GetByteCount(head);
        int bodyLength = Encoding.UTF8.GetByteCount(body);
        byte[] data = new byte[1 + 1 + 8 + 2 + 1 + headLength + 1 + bodyLength];

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // playerId (1 Byte)
        data[index] = playerId;
        index++;

        // start position (2 * 4 Bytes)
        Buffer.BlockCopy(BitConverter.GetBytes(startPosition.x), 0, data, index, 4);
        index += 4;
        Buffer.BlockCopy(BitConverter.GetBytes(startPosition.y), 0, data, index, 4);
        index += 4;

        // world and dungeon index (2 * 1 Byte)
        data[index] = worldIndex;
        index++;
        data[index] = dungeonIndex;
        index++;

        // head (1 Byte length + string length)
        data[index] = (byte)headLength;
        index++;
        Encoding.UTF8.GetBytes(head).CopyTo(data, index);
        index += headLength;

        // body (1 Byte length + string length)
        data[index] = (byte)bodyLength;
        index++;
        Encoding.UTF8.GetBytes(body).CopyTo(data, index);

        return data;
    }

    /// <summary>
    ///     Deserializes the recived data and converts it to a connection message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>message with required inital data</returns>
    public static new ConnectionMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // playerId (1 Byte)
        byte playerId = data[index];
        index++;

        // start position (2 * 4 Byte)
        float posX = BitConverter.ToSingle(data, index);
        index += 4;
        float posY = BitConverter.ToSingle(data, index);
        index += 4;

        // world and dungeon index (2 * 1 Byte)
        byte worldIndex = data[index];
        index++;
        byte dungenIndex = data[index];
        index++;

        // head (1 Byte length + string length)
        int headLength = data[index];
        index++;
        string head = Encoding.UTF8.GetString(data, index, headLength);
        index += headLength;

        // body (1 Byte length + string length)
        int bodyLength = data[index];
        index++;
        string body = Encoding.UTF8.GetString(data, index, bodyLength);

        return new ConnectionMessage(playerId, new Vector2(posX, posY), worldIndex, dungenIndex ,head, body);
    }

    public Vector2 GetStartPosition() { return startPosition; }
    public byte GetWorldIndex() { return worldIndex; }
    public byte GetDungeonIndex() { return dungeonIndex; }
    public string GetHead() { return head; }
    public string GetBody() { return body; }
}