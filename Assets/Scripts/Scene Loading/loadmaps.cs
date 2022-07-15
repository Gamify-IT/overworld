using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadmaps : MonoBehaviour
{
    public string sceneLoadOne;
    public string sceneLoadTwo;
    public string sceneUnloadOne;
    public string sceneUnloadTwo;
    public bool loadingFinished;

    public void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.tag == "Player" && loadingFinished)
        {
            StartCoroutine(LoadUnloadWorlds(sceneLoadOne, sceneLoadTwo, sceneUnloadOne, sceneUnloadTwo));
        }
    }

    public IEnumerator LoadUnloadWorlds(string loadWorldOne, string loadWorldTwo, string unloadWorldOne, string unloadWorldTwo)
    {
        Debug.Log("start loading");
        loadingFinished = false;
        AsyncOperation asyncOperationLoadOne = null;
        AsyncOperation asyncOperationLoadTwo = null;
        AsyncOperation asyncOperationUnloadOne = null;
        AsyncOperation asyncOperationUnloadTwo = null;

        if (!SceneManager.GetSceneByName(loadWorldOne).isLoaded)
        {
            asyncOperationLoadOne = SceneManager.LoadSceneAsync(loadWorldOne, LoadSceneMode.Additive);
        }
        if (!SceneManager.GetSceneByName(loadWorldTwo).isLoaded)
        {
            asyncOperationLoadTwo = SceneManager.LoadSceneAsync(loadWorldTwo, LoadSceneMode.Additive);
        }
        if (SceneManager.GetSceneByName(unloadWorldOne).isLoaded)
        {
            asyncOperationUnloadOne = SceneManager.UnloadSceneAsync(unloadWorldOne);
        }
        if (SceneManager.GetSceneByName(unloadWorldTwo).isLoaded)
        {
            asyncOperationUnloadTwo = SceneManager.UnloadSceneAsync(unloadWorldTwo);
        }

        while (
            (asyncOperationLoadOne != null && !asyncOperationLoadOne.isDone) && 
            (asyncOperationLoadTwo != null && !asyncOperationLoadTwo.isDone) && 
            (asyncOperationUnloadOne != null && !asyncOperationUnloadOne.isDone) && 
            (asyncOperationUnloadTwo != null && !asyncOperationUnloadTwo.isDone))
        {
            Debug.Log("loading");
            yield return null;
        }
        Debug.Log("finished loading");
        loadingFinished = true;
    }
}
