using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class provides the 'startOfflineMode()' function which loads the offline mode screen.
/// </summary>
public class OfflineMode : MonoBehaviour
{
    /// <summary>
    ///     This function loads the offline mode screen.
    /// </summary>
    public void StartOfflineMode()
    {
        LoadingManager.Instance.LoadScene();
        SceneManager.UnloadSceneAsync("OfflineMode");
    }
}