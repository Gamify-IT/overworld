using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FacingDirection
{
    north,
    east,
    south,
    west
}

/// <summary>
///     This class is used to manage the dungeon transition.
/// </summary>
public class LoadSubScene : MonoBehaviour
{
    private int worldIndex;
    private int dungeonIndex;
    public string sceneToLoad;
    public int worldIndexToLoad;
    public int dungeonIndexToLoad;
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float loadingTime;
    public Vector2 playerPosition;
    public FacingDirection facingDirection;

    /// <summary>
    ///     This function is called when the player enters a dungeon entrance.
    ///     It loads the dungeon scene and sets the player's position.
    ///     Also it fades in the scene.
    /// </summary>
    /// <param name="playerCollision">2D Collider of current player</param>
    private void OnTriggerEnter2D(Collider2D playerCollision)
    {
        if (playerCollision.CompareTag("Player"))
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

        GameManager.Instance.SetData(worldIndexToLoad, dungeonIndexToLoad);

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

        if (facingDirection == FacingDirection.north)
        {
            PlayerAnimation.Instance.playerAnimator.Play("Idle_Up");
        }

        if (facingDirection == FacingDirection.east)
        {
            PlayerAnimation.Instance.playerAnimator.Play("Idle_Right");
        }

        if (facingDirection == FacingDirection.south)
        {
            PlayerAnimation.Instance.playerAnimator.Play("Idle_Down");
        }

        if (facingDirection == FacingDirection.west)
        {
            PlayerAnimation.Instance.playerAnimator.Play("Idle_Left");
        }

        DestroyImmediate(fadeOutPanelCopy, true);
        Destroy(panel, 1);
    }

    #region GetterAndSetter
    public void SetWorldIndex(int worldIndex)
    {
        this.worldIndex = worldIndex;
    }

    public int GetWorldIndex()
    {
        return worldIndex;
    }

    public void SetDungeonIndex(int dungeonIndex)
    {
        this.dungeonIndex = dungeonIndex;
    }

    public int GetDungeonIndex()
    {
        return dungeonIndex;
    }

    #endregion
}