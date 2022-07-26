using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSubScene : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float loadingTime;
    public Vector2 playerPosition;


    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag == "Player")
        {
            StartCoroutine(FadeCoroutine());
        }
    }

    /// <summary>
    /// This Coroutine starts a fadeout after walking into a Dungeon entrance.
    /// After the Loading of the new Scene is complete, the fadeIn starts and the player position is adjusted.
    /// Finally the fadeOutPanel is destroyed.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeCoroutine()
    {
        GameObject fadeOutPanelCopy = Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(loadingTime);
        AsyncOperation asyncOperationLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!asyncOperationLoad.isDone)
        {
            yield return null;
        }

        for(int sceneIndex = 0;sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player")  && !tempSceneName.Equals("Player HUD") && !tempSceneName.Equals(sceneToLoad))
            {
                SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
        GameObject panel = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
        DestroyImmediate(fadeOutPanelCopy, true);
        Destroy(panel, 1); 
    }
}
