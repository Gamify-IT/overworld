using System;
using UnityEngine;

/// <summary>
///     This class offers event trigger for multiplayer communication.
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

    public event EventHandler<EventArgs> OnDataChanged;

    public void TriggerDataChanged<T>(T message)
    {
        OnDataChanged?.Invoke(this, new EventArgsWrapper<T>(message));
    }
}

/// <summary>
///     Wrapper class for the message type the event trigger is used for.
/// </summary>
/// <typeparam name="T">message type of the event trigger</typeparam>
public class EventArgsWrapper<T> : EventArgs
{
    private readonly T message;

    public EventArgsWrapper(T message)
    {
        this.message = message;
    }

    public T GetMessage() {  return message; }
}