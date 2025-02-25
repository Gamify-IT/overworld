/// <summary>
///     Possible message types for the multiplayer communication protocol.
/// </summary>
public enum MessageType : byte
{
    Connection = 0,
    Disconnection = 1,
    Acknowledge = 2,
    Position = 3,
    Character = 4,
    Area = 5,
    PingPong = 6
}