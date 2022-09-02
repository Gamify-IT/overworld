using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     The <c>InfoManager</c>
/// </summary>
public class InfoManager : MonoBehaviour
{
    #region Attributes

    public TextMeshProUGUI infoText;
    public TextMeshProUGUI headerText;

    #endregion

    #region Singleton

    public static InfoManager Instance { get; private set; }

    /// <summary>
    ///     The Awake function is called when the object is initialized and sets up the starting values and state of the
    ///     object.
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.headerText.text = "";
            Instance.infoText.text = "";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    ///     The OnDestroy function is called when the object is deleted.
    ///     This function clears the singleton instance.
    /// </summary>
    private void OnDestroy()
    {
        Instance = null;
    }

    #endregion

    #region Functionality

    /// <summary>
    ///     This function displays the given text on the panel.
    /// </summary>
    /// <param name="info">The text to display</param>
    public void DisplayInfo(string header, string info)
    {
        headerText.text = header;
        infoText.text = info;
    }

    /// <summary>
    ///     This function is to be called by the close button of the panel.
    /// </summary>
    public void CloseButtonPressed()
    {
        CloseInfoPanel();
    }

    /// <summary>
    ///     This function closes the info panel.
    /// </summary>
    /// <returns></returns>
    private async UniTask CloseInfoPanel()
    {
        await SceneManager.UnloadSceneAsync("InfoScreen");
    }

    #endregion
}