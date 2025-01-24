using UnityEngine;

public class RemotePlayerData
{
    private string playerId;
    private Vector2 position;
    private Vector2 movement;
    private float currentSpeed;
    private int characterIndex;
    private GameObject playerPrefab;

    public RemotePlayerData(string playerId, Vector2 position)
    {
        this.playerId = playerId;  
        this.position = position;   
    }

    #region getter 
    public string GetId() { return playerId; }
    public Vector2 GetPosition() { return position; }
    public Vector2 GetMovement() { return movement; }
    public float GetCurrentSpeed() { return currentSpeed; }
    #endregion
}