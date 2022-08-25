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
    public TextMeshProUGUI loadingText;

    string sceneToLoad;
    int worldIndex;
    int dungeonIndex;
    Vector2 playerPosition;
    #endregion

    #region Functionality
    public async UniTask loadData(string sceneToLoad, int worldIndex, int dungeonIndex, Vector2 playerPosition)
    {
        this.sceneToLoad = sceneToLoad;
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
        this.playerPosition = playerPosition;

        slider.value = 0;
        progressText.text = "0%";
        loadingText.text = "LOADING DATA...";

        if(GameManagerV2.instance == null)
        {
            Debug.Log("Game Manager not online yet.");
            return;
        }

        Debug.Log("Start fetching data");

        await GameManagerV2.instance.fetchData();

        Debug.Log("Finish fetching data");

        Debug.Log("Validate data");
        if(GameManagerV2.instance.loadingError)
        {
            SceneManager.LoadSceneAsync("OfflineMode", LoadSceneMode.Additive);
        }
        else
        {
            await loadScene();
        }
    }

    public async UniTask loadScene()
    {
        slider.value = 0.5f;
        progressText.text = "50%";
        loadingText.text = "LOADING WORLD...";

        Debug.Log("Start loading scene");

        await SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        Debug.Log("Finish loading scene");

        slider.value = 0.75f;
        progressText.text = "75%";
        loadingText.text = "PROCESSING DATA...";

        Debug.Log("Setting data");

        GameManagerV2.instance.setData(worldIndex, dungeonIndex);

        slider.value = 0.85f;
        progressText.text = "85%";
        loadingText.text = "PREPARING GAME START...";

        Debug.Log("Start unloading other scenes");

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") && !tempSceneName.Equals(sceneToLoad) && !tempSceneName.Equals("LoadingScreen") && !tempSceneName.Equals("OfflineMode"))
            {
                await SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }

        Debug.Log("Finish unloading other scenes");

        slider.value = 0.95f;
        progressText.text = "95%";
        loadingText.text = "SETTING UP PLAYER...";

        Debug.Log("Set player position");

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;

        slider.value = 1;
        progressText.text = "100%";
        loadingText.text = "DONE...";

        Debug.Log("Loading manager done");

        Debug.Log("Start unloading loading Manager");

        await SceneManager.UnloadSceneAsync("LoadingScreen");

        Debug.Log("Finish unloading loading Manager");
    }
    #endregion
}
