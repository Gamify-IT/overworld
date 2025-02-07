/// <summary>
///     Possible message types for the multiplayer communication protocol.
/// </summary>
public enum MessageType : byte
{
    Connect = 0,
    Disconnect = 1,
    Acknowledge = 2,
    Position = 3,
    Character = 4,
    AreaInformation = 5
}