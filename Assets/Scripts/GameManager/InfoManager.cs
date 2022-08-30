using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class InfoManager : MonoBehaviour
{
    #region Singleton
    public static InfoManager instance { get; private set; }

    /// <summary>
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

    private void OnDestroy()
    {
        instance = null;
    }

    #endregion

    #region Attributes
    public TextMeshProUGUI infoText;
    #endregion

    #region Functionality
    public void displayInfo(string info)
    {
        infoText.text = info;
    }

    public void closeButtonPressed()
    {
        closeInfoPanel();
    }

    private async UniTask closeInfoPanel()
    {
        await SceneManager.UnloadSceneAsync("InfoScreen");
    }
    #endregion
}
