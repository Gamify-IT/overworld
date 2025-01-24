using UnityEngine;

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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public delegate void DataChangedEventHandler(object sender, object data);
    public static event DataChangedEventHandler OnPositionChanged;

    public void TriggerPositionChanged(Vector2 newPosition)
    {
        OnPositionChanged?.Invoke(this, newPosition);
    }
}