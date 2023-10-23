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
    public async void StartOfflineMode()
    {
        GameManager.Instance.GetDummyData();

        //ersetzen durch get dummy data
        await DataManager.Instance.FetchAreaData();


        LoadingManager.Instance.LoadScene();
        SceneManager.UnloadSceneAsync("OfflineMode");
    }
}