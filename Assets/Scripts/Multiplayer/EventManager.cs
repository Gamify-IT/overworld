using System;
using UnityEngine;

/// <summary>
///     This class manages movement events by the player, trigger multiplayer communication.
/// </summary>
public class EventManager : MonoBehaviour
{
    #region singelton
    public static EventManager Instance { get; private set; }
    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    //public delegate void DataChangedEventHandler(object sender, object data);
    public event EventHandler<PositionChangedEventArgs> OnPositionChanged;

    /// <summary>
    ///     Trigger that sends the players position and movement vector.
    /// </summary>
    /// <param name="newPosition">players new position</param>
    /// <param name="movement">players normalized movement vector</param>
    public void TriggerPositionChanged(Vector2 newPosition, Vector2 movement)
    {
        OnPositionChanged?.Invoke(this, new(newPosition, movement));
    }
}

#region custom event argument
/// <summary>
///     Custom event for player movement und position.
/// </summary>
public class PositionChangedEventArgs : EventArgs
{
    private Vector2 newPosition;
    private Vector2 movement;

    public PositionChangedEventArgs(Vector2 newPosition, Vector2 movement)
    {
        this.newPosition = newPosition;
        this.movement = movement;
    }

    public Vector2 GetPosition() { return newPosition; }
    public Vector2 GetMovement() { return movement; }
}
#endregion