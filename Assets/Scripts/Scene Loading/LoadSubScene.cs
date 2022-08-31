using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class is used to manage the dungeon transition.
/// </summary>
public class LoadSubScene : MonoBehaviour
{
    public string sceneToLoad;
    public int worldIndex;
    public int dungeonIndex;
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float loadingTime;
    public Vector2 playerPosition;

    /// <summary>
    ///     This function is called when the player enters a dungeon entrance.
    ///     It loads the dungeon scene and sets the player's position.
    ///     Also it fades in the scene.
    /// </summary>
    /// <param name="playerCollision">2D Collider of current player</param>
    private void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.tag == "Player")
        {
            StartCoroutine(FadeCoroutine());
        }
    }

    /// <summary>
    ///     This Coroutine starts a fadeout after walking into a Dungeon entrance.
    ///     After the Loading of the new Scene is complete, the fadeIn starts and the player position is adjusted.
    ///     Finally the fadeOutPanel is destroyed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeCoroutine()
    {
        GameObject fadeOutPanelCopy = Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(loadingTime);
        AsyncOperation asyncOperationLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncOperationLoad.isDone)
        {
            yield return null;
        }

        GameManager.instance.setData(worldIndex, dungeonIndex);

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") &&
                !tempSceneName.Equals(sceneToLoad))
            {
                SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }

        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
        GameObject panel = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity);
        DestroyImmediate(fadeOutPanelCopy, true);
        Destroy(panel, 1);
    }
}