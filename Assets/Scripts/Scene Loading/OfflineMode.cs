using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
///     This class provides the 'startOfflineMode()' function which loads the offline mode screen.
/// </summary>
public class OfflineMode : MonoBehaviour
{
    #region Singleton

    public static OfflineMode Instance { get; private set; }

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

    /// <summary>
    ///     This function deletes the singleton instance, so it can be reinitialized.
    /// </summary>
    private void OnDestroy()
    {
        Instance = null;
    }

    #endregion

    [SerializeField] private TextMeshProUGUI infoText;

    /// <summary>
    ///     This function loads the offline mode screen.
    /// </summary>
    public async void StartOfflineMode()
    {
        GameManager.Instance.GetDummyData();

        DataManager.Instance.GetDummyAreaData();
        LoadingManager.Instance.LoadScene();
        SceneManager.UnloadSceneAsync("OfflineMode");

        
    }

    /// <summary>
    ///     This function displays the given text as the error message
    /// </summary>
    /// <param name="text">The text to be displayed</param>
    public void DisplayInfo(string text)
    {
        infoText.text = text;
    }
}