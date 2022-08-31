using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class is responsible for loading the next world when the hitbox is triggered.
/// </summary>
public class LoadMaps : MonoBehaviour
{
    public string sceneOrigin;
    public string sceneDestination;
    public int sceneDestinationIndex;

    /// <summary>
    ///     This function is called when the player enters hitbox on the way to the next world.
    ///     It calls 'SetupWorld()' to load the next scene and set the data for the next world.
    /// </summary>
    /// <param name="playerCollision">2D Collider of current player</param>
    public void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.tag == "Player")
        {
            SetupWorld();
        }
    }

    /// <summary>
    ///     This function setups the world data.
    ///     If the world isn't already loaded completely it calls 'LoadWorld()' to load the scene.
    /// </summary>
    private async UniTask SetupWorld()
    {
        if (SceneManager.GetSceneByName(sceneDestination).isLoaded)
        {
            return;
        }

        await LoadWorld();
        GameManager.Instance.SetData(sceneDestinationIndex, 0);
    }

    /// <summary>
    ///     This function loads the scene of the world the player entered and unload not needed worlds.
    /// </summary>
    private async UniTask LoadWorld()
    {
        await SceneManager.LoadSceneAsync(sceneDestination, LoadSceneMode.Additive);

        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            string tempSceneName = SceneManager.GetSceneAt(sceneIndex).name;
            if (!tempSceneName.Equals("Player") && !tempSceneName.Equals("Player HUD") &&
                !tempSceneName.Equals(sceneOrigin) && !tempSceneName.Equals(sceneDestination))
            {
                await SceneManager.UnloadSceneAsync(tempSceneName);
            }
        }
    }
}