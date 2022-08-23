using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class LoadingManager : MonoBehaviour
{
    #region Singleton
    public static LoadingManager instance { get; private set; }

    /// <summary>
    /// This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI progressText;
    #endregion

    #region Functionality
    public async UniTask loadScene(string sceneToLoad, int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        slider.value = 0;

        if(GameManagerV2.instance == null)
        {
            Debug.Log("Game Manager not online yet.");
            return;
        }

        await GameManagerV2.instance.fetchData();

        slider.value = 0.5f;

        AsyncOperation asyncOperationLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!asyncOperationLoad.isDone){}

        slider.value = 0.75f;

        GameManagerV2.instance.setData(worldIndex, dungeonIndex);

        slider.value = 0.85f;

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") && !tempSceneName.Equals(sceneToLoad))
            {
                await SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }

        slider.value = 0.95f;

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;

        slider.value = 1;

        await SceneManager.UnloadSceneAsync("LoadingScreen");
    }
    #endregion
}
