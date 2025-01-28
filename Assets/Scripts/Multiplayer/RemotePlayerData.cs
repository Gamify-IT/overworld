using UnityEngine;

/// <summary>
///     Represents the data of a remote player, i.e. a player connected via multiplayer.
/// </summary>
public class RemotePlayerData
{
    private readonly string playerId;
    private Vector2 position;
    private Vector2 movement;
    private int characterIndex;
    private GameObject playerPrefab;

    public RemotePlayerData(string playerId, Vector2 position, Vector2 movement)
    {
        this.playerId = playerId;  
        this.position = position;  
        this.movement = movement;
    }

    #region getter 
    public string GetId() { return playerId; }
    public Vector2 GetPosition() { return position; }
    public Vector2 GetMovement() { return movement; }
    #endregion
}