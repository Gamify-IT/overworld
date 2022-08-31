using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// The <c>InfoManager</c> 
/// </summary>
public class InfoManager : MonoBehaviour
{
    #region Singleton
    public static InfoManager instance { get; private set; }

    /// <summary>
    /// The Awake function is called when the object is initialized and sets up the starting values and state of the object.
    /// This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.infoText.text = "";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// The OnDestroy function is called when the object is deleted.
    /// This function clears the singleton instance.
    /// </summary>
    private void OnDestroy()
    {
        instance = null;
    }

    #endregion

    #region Attributes
    public TextMeshProUGUI infoText;
    #endregion

    #region Functionality
    /// <summary>
    /// This function displays the given text on the panel.
    /// </summary>
    /// <param name="info">The text to display</param>
    public void displayInfo(string info)
    {
        infoText.text = info;
    }

    /// <summary>
    /// This function is to be called by the close button of the panel.
    /// </summary>
    public void closeButtonPressed()
    {
        closeInfoPanel();
    }

    /// <summary>
    /// This function closes the info panel.
    /// </summary>
    /// <returns></returns>
    private async UniTask closeInfoPanel()
    {
        await SceneManager.UnloadSceneAsync("InfoScreen");
    }
    #endregion
}
