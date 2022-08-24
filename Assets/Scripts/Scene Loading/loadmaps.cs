using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadmaps : MonoBehaviour
{
    public string sceneLoadOne;
    public string sceneLoadTwo;
    public bool loadingFinished;
    public int sceneToLoadIndex;

    public void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.tag == "Player" && loadingFinished)
        {
            StartCoroutine(LoadUnloadWorlds(sceneLoadOne, sceneLoadTwo));
        }
    }

    public IEnumerator LoadUnloadWorlds(string loadWorldOne, string loadWorldTwo)
    {
        loadingFinished = false;
        AsyncOperation asyncOperationLoadOne = null;
        AsyncOperation asyncOperationLoadTwo = null;

        if (!SceneManager.GetSceneByName(loadWorldOne).isLoaded)
        {
            asyncOperationLoadOne = SceneManager.LoadSceneAsync(loadWorldOne, LoadSceneMode.Additive);
        }
        if (!SceneManager.GetSceneByName(loadWorldTwo).isLoaded)
        {
            asyncOperationLoadTwo = SceneManager.LoadSceneAsync(loadWorldTwo, LoadSceneMode.Additive);
        }

        for(int sceneIndex = 0;sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player")  && !tempSceneName.Equals("Player HUD") && !tempSceneName.Equals(loadWorldOne) && !tempSceneName.Equals(loadWorldTwo))
            {
                SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }

        while (
            (asyncOperationLoadOne != null && !asyncOperationLoadOne.isDone) && 
            (asyncOperationLoadTwo != null && !asyncOperationLoadTwo.isDone))
        {
            yield return null;
        }
        loadingFinished = true;
        GameManagerV2.instance.setData(sceneToLoadIndex, 0);
    }
}
