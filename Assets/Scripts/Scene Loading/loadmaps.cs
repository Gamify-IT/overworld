using System.Collections;
using System.Collections.Generic;
//using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class loadmaps : MonoBehaviour
{
    public string sceneOrigin;
    public string sceneDestination;
    public int sceneDestinationIndex;

    public void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.tag == "Player")
        {
            setupWorld();
        }
    }

    //setup world entered
    //  - if already active, do nothing
    //  - else: load scene and set data
    private async UniTask setupWorld()
    {
        if (SceneManager.GetSceneByName(sceneDestination).isLoaded)
        {
            return;
        }
        await loadWorld();
        GameManagerV2.instance.setData(sceneDestinationIndex, 0);
    }

    //load needed world and unload not needed worlds, if present
    private async UniTask loadWorld()
    {
        await SceneManager.LoadSceneAsync(sceneDestination, LoadSceneMode.Additive);

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") && !tempSceneName.Equals(sceneOrigin) && !tempSceneName.Equals(sceneDestination))
            {
                await SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }
    }
}
