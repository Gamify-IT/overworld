using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FacingDirection
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

/// <summary>
///     This class is used to manage the dungeon transition.
/// </summary>
public class LoadSubScene : MonoBehaviour
{
    public static AreaInformation areaExchange = DataManager.Instance.GetPlayerData().GetLogoutScene() == "Dungeon" ? 
        new AreaInformation(DataManager.Instance.GetPlayerData().GetCurrentArea().worldIndex, new Optional<int>(DataManager.Instance.GetPlayerData().GetCurrentArea().dungeonIndex))
        : new AreaInformation(DataManager.Instance.GetPlayerData().GetCurrentArea().worldIndex, new Optional<int>());
    public static bool transitionBlocked = false;
    public static bool setupDone = false;

    [SerializeField] private int worldIndex;
    [SerializeField] private int dungeonIndex;
    public int worldIndexToLoad;
    public int dungeonIndexToLoad;
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float loadingTime;
    public FacingDirection facingDirection;

    private void Start()
    {
        if(GameSettings.GetGamemode() != Gamemode.PLAY)
        {
            return;
        }

        if(transitionBlocked)
        {
            return;
        }

        if(dungeonIndex == 0)
        {
            //Scene Transition in world
            if(areaExchange.IsDungeon())
            {
                //was previously in a dungeon
                if(areaExchange.GetWorldIndex() == worldIndexToLoad && areaExchange.GetDungeonIndex() == dungeonIndexToLoad)
                {
                    //was in the dungeon this scene transition points to
                    areaExchange = new AreaInformation(worldIndex, new Optional<int>());

                    if (facingDirection == FacingDirection.NORTH)
                    {
                        PlayerAnimation.Instance.playerAnimator.Play("Idle_Up");
                        Vector2 playerPosition = this.transform.position + new Vector3(0, 2, 0);
                        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                    }

                    if (facingDirection == FacingDirection.EAST)
                    {
                        PlayerAnimation.Instance.playerAnimator.Play("Idle_Right");
                        Vector2 playerPosition = this.transform.position + new Vector3(2, 0, 0);
                        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                    }

                    if (facingDirection == FacingDirection.SOUTH)
                    {
                        PlayerAnimation.Instance.playerAnimator.Play("Idle_Down");
                        Vector2 playerPosition = this.transform.position + new Vector3(0, -2, 0);
                        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                    }

                    if (facingDirection == FacingDirection.WEST)
                    {
                        PlayerAnimation.Instance.playerAnimator.Play("Idle_Left");
                        Vector2 playerPosition = this.transform.position + new Vector3(-2, 0, 0);
                        GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                    }
                }
            }
        }
        else
        {
            if (!GameManager.Instance.GetJustLoaded())
            {
                //Scene Transition in dungeon
                Debug.Log("Now in dungeon: " + areaExchange.GetWorldIndex() + "-" + areaExchange.GetDungeonIndex());
                worldIndex = areaExchange.GetWorldIndex();
                dungeonIndex = areaExchange.GetDungeonIndex();

                if (facingDirection == FacingDirection.NORTH)
                {
                    PlayerAnimation.Instance.playerAnimator.Play("Idle_Up");
                    Vector2 playerPosition = this.transform.position + new Vector3(0, 2, 0);
                    GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                }

                if (facingDirection == FacingDirection.EAST)
                {
                    PlayerAnimation.Instance.playerAnimator.Play("Idle_Right");
                    Vector2 playerPosition = this.transform.position + new Vector3(2, 0, 0);
                    GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                }

                if (facingDirection == FacingDirection.SOUTH)
                {
                    PlayerAnimation.Instance.playerAnimator.Play("Idle_Down");
                    Vector2 playerPosition = this.transform.position + new Vector3(0, -2, 0);
                    GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                }

                if (facingDirection == FacingDirection.WEST)
                {
                    PlayerAnimation.Instance.playerAnimator.Play("Idle_Left");
                    Vector2 playerPosition = this.transform.position + new Vector3(-2, 0, 0);
                    GameObject.FindGameObjectWithTag("Player").transform.position = playerPosition;
                }

                if (!setupDone)
                {
                    GameEvents.current.SetupDungeon(areaExchange);
                    setupDone = true;
                }
            }
        }
        GameManager.Instance.SetJustLoaded(false);
    }

    /// <summary>
    ///     This function initializes the <c>SceneTransition</c> object
    /// </summary>
    /// <param name="areaIdentifier">The area the <c>SceneTransition</c> is in</param>
    /// <param name="areaToLoad">The area the <c>SceneTransition</c> is getting the player to</param>
    /// <param name="facingDirection">The direction of the player is facing (in the current area)</param>
    public void Initialize(AreaInformation areaIdentifier, AreaInformation areaToLoad, FacingDirection facingDirection)
    {
        worldIndex = areaIdentifier.GetWorldIndex();
        dungeonIndex = 0;
        if (areaIdentifier.IsDungeon())
        {
            dungeonIndex = areaIdentifier.GetDungeonIndex();
        }

        worldIndexToLoad = areaToLoad.GetWorldIndex();
        dungeonIndexToLoad = 0;
        if(areaToLoad.IsDungeon())
        {
            dungeonIndexToLoad = areaToLoad.GetDungeonIndex();
        }

        this.facingDirection = facingDirection;
    }

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
            if(dungeonIndex == 0)
            {
                AreaInformation areaToLoad = new AreaInformation(worldIndexToLoad, new Optional<int>(dungeonIndexToLoad));
                areaExchange = areaToLoad;
                setupDone = false;
            }
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

        string sceneToLoad;
        if(dungeonIndexToLoad == 0)
        {
            sceneToLoad = "World " + worldIndexToLoad;
        }
        else
        {
            sceneToLoad = "Dungeon";
        }

        AsyncOperation asyncOperationLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncOperationLoad.isDone)
        {
            yield return null;
        }

        DataManager.Instance.SetPlayerPosition(worldIndexToLoad, dungeonIndexToLoad);
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

        GameObject panel = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity);

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