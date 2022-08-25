using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineMode : MonoBehaviour
{
    public void startOfflineMode()
    {
        LoadingManager.instance.loadScene();
        SceneManager.UnloadSceneAsync("OfflineMode");
    }
}
