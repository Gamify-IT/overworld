using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
///     Manages the beahvior of the multiplayer HUD element in the bottom right corner.
/// </summary>
public class MultiplayerHUD : MonoBehaviour
{
    [Header("HUD panel")]
    [SerializeField] private GameObject background;

    [Header("Connection State")]
    [SerializeField] private GameObject connected;
    [SerializeField] private TextMeshProUGUI playerNumber;
    [SerializeField] private GameObject disconnected;
    [SerializeField] private GameObject inactive;

    #region singelton
    public static MultiplayerHUD Instance { get; private set; }
    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("online");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        playerNumber.text = "";
    }

    private void Update()
    {
        if (MultiplayerManager.Instance.IsConnected())
        {
            playerNumber.text = MultiplayerManager.Instance.GetNumberOfConnectedPlayers().ToString();
        }
    }

    /// <summary>
    ///     Sets the multiplayer HUD to the given state.
    /// </summary>
    /// <param name="type">state of the multiplayer connection</param>
    public void SetState(FeedbackType type)
    {
        ActivatePanel(type);
        switch (type)
        {
            case FeedbackType.Disconnected:
                StartCoroutine(CloseHUD());
                return;
            default:
                return;
        }
    }

    /// <summary>
    ///     Activates the correct panel for the current state in the multiplayer HUD and deactivates all others.
    /// </summary>
    /// <param name="state">state of the multiplayer connection</param>
    private void ActivatePanel(FeedbackType state)
    {
        background.SetActive(true);
        foreach (Transform item in background.transform)
        {
            if (item.gameObject.name == state.ToString())
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    ///     Closes the multiplayer HUD after 5 seconds.
    /// </summary>
    private IEnumerator CloseHUD()
    {
        // wait 5 seconds and close HUD
        yield return new WaitForSecondsRealtime(5.0f);
        background.SetActive(false );
    }
}
