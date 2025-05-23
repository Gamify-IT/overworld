using System.Text;

/// <summary>
///     Network message for character outfit data of a player.
/// </summary>
public class CharacterMessage : NetworkMessage
{
    protected override MessageType Type => MessageType.Character;
    private readonly string head;
    private readonly string body;

    public CharacterMessage(ushort clientId, string head, string body) : base(clientId)
    {
        this.head = head;
        this.body = body;
    }

    public override byte[] Serialize()
    {
        int index = 0;

        // compute array size
        int headLength = Encoding.UTF8.GetByteCount(head);
        int bodyLength = Encoding.UTF8.GetByteCount(body);
        byte[] data = new byte[5 + headLength + bodyLength]; // 1 + 2 + 1 + headLength + 1 + bodyLength

        // message type (1 Byte)
        data[index] = (byte)Type;
        index++;

        // clientId (2 Byte)
        data[index] = (byte)(clientId & 0xFF);
        data[index + 1] = (byte)((clientId >> 8) & 0xFF);
        index += 2;

        // head (1 Byte length + string)
        data[index] = (byte)headLength;
        index++;
        Encoding.UTF8.GetBytes(head).CopyTo(data, index);
        index += headLength;

        // body (1 Byte length + string)
        data[index] = (byte)bodyLength;
        index++;
        Encoding.UTF8.GetBytes(body).CopyTo(data, index);

        return data;
    }

    /// <summary>
    ///     Deserializes the received data and converts it to a character message object.
    /// </summary>
    /// <param name="data">received network data</param>
    /// <returns>character message</returns>
    public static new CharacterMessage Deserialize(ref byte[] data)
    {
        // skip message type (1 Byte)
        int index = 1;

        // clientId (2 Byte)
        ushort clientId = (ushort)(data[index] | (data[index + 1] << 8));
        index += 2;

        // head (1 Byte length + string)
        int headLength = data[index];
        index++;
        string head = Encoding.UTF8.GetString(data, index, headLength);
        index += headLength;

        // body (1 Byte length + string)
        int bodyLength = data[index];
        index++;
        string body = Encoding.UTF8.GetString(data, index, bodyLength);

        return new CharacterMessage(clientId, head, body);
    }

    public string GetHead() {  return head; }
    public string GetBody() { return body; }
}