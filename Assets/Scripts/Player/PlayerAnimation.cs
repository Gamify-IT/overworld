using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

/// <summary>
///     This class manages the movement and the animations of the player.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    public float movementSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float superSpeed = 20f;
    public Vector2 movement;
    public Rigidbody2D playerRigidBody;
    public Animator playerAnimator;
    public Animator accessoireAnimator;
    public Transform accessoireTransform;
    private bool busy;
    private bool canMove;
    private float currentSpeed;
    private float targetSpeed;

    private Vector3 lastPosition;
    private float distanceWalked;

    //KeyCodes
    private KeyCode moveUp;
    private KeyCode moveLeft;
    private KeyCode moveDown;
    private KeyCode moveRight;
    private KeyCode sprint;
    private float sprintStartTime = 0f;
    private float sprintDuration = 0f;

    private float timeInGameStart = 0f;
    private float timeInGameDuration = 0f;

    public AudioClip moveSound;
    private AudioSource audioSource;
    private bool isMoving;

    readonly Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
    // Names of the animators available
    public static readonly List<string> validOutfits = new List<string> {"3D_glasses", "blonde_hair", "character_suit", "character_black_and_white", 
        "character_blue_and_purple", "character_default", "character_ironman", "none", "character_jeans_checkshirt", "character_long_hair", 
        "character_santa", "character_tracksuit", "cool_glasses", "flaming_hair", "globe_hat", "heart_glasses", "retro_glasses", "safety_helmet"};

    private PlayerStatisticData ownPlayerData;
    private int rewardsAmount;

    // Multiplayer
    private const float positionThreshold = 0.01f;

    /// <summary>
    ///     This method is called before the first frame update.
    ///     It is used to initialize variables.
    /// </summary>
    private void Start()
    {
        accessoireAnimator = this.gameObject.transform.GetChild(2).GetComponent<Animator>();
        accessoireTransform = this.gameObject.transform.GetChild(2).GetComponent<Transform>();
        InitializeOutfitPositions();

        string lastPlayDateStr = PlayerPrefs.GetString("LastPlayDate", "");
        int daysCount = PlayerPrefs.GetInt("DaysPlayed", 0);
        InitializeAudio();

        timeInGameStart = Time.time;

        canMove = true;
        busy = false;
        playerRigidBody = GetComponent<Rigidbody2D>();
        currentSpeed = movementSpeed;
        targetSpeed = currentSpeed;

        lastPosition = transform.position;

        moveUp = GameManager.Instance.GetKeyCode(Binding.MOVE_UP);
        moveLeft = GameManager.Instance.GetKeyCode(Binding.MOVE_LEFT);
        moveDown = GameManager.Instance.GetKeyCode(Binding.MOVE_DOWN);
        moveRight = GameManager.Instance.GetKeyCode(Binding.MOVE_RIGHT);
        sprint = GameManager.Instance.GetKeyCode(Binding.SPRINT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
        Invoke("CheckForRewardsAmount", 3.5f);
    }

    /// <summary>
    ///     Initializes the position of each outfit.
    /// </summary>
    private void InitializeOutfitPositions()
    {
        positions.Add("3D_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("blonde_hair", new Vector3(0, 0.3f, 0));
        positions.Add("cool_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("flaming_hair", new Vector3(0, 0.3f, 0));
        positions.Add("globe_hat", new Vector3(0, 0.76f, 0));
        positions.Add("heart_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("retro_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("safety_helmet", new Vector3(0, 0.31f, 0));
        positions.Add("none", new Vector3(0, 0, 0));
    }

    /// <summary>
    ///     This function updates the rewards amount for achievement regarding saved data in the leaderboard
    /// </summary>
    private void CheckForRewardsAmount()
    {
        ownPlayerData = DataManager.Instance.GetPlayerData();
        rewardsAmount = ownPlayerData.GetRewards();
        GameManager.Instance.UpdateAchievement(AchievementTitle.GET_COINS, rewardsAmount, null);
        GameManager.Instance.UpdateAchievement(AchievementTitle.GET_MORE_COINS, rewardsAmount, null);
    }

    /// <summary>
    ///     This function adds new audio sources and sets clip with move sound
    /// </summary>
    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = moveSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }
    
    /// <summary>
    ///     If 'canMove' is true, this function allows the player to move.
    /// </summary>
    private void Update()
    {
        CheckForRewardsAmount();
        if (canMove)
        {
            
            isMoving = false;
            movement.x = 0;
            movement.y = 0;
            if (Input.GetKey(moveLeft))
            {
                movement.x -= 1;
                isMoving = true;
            }

            if (Input.GetKey(moveRight))
            {
                movement.x += 1;
                isMoving = true;
            }

            if (Input.GetKey(moveDown))
            {
                movement.y -= 1;
                isMoving = true;
            }

            if (Input.GetKey(moveUp))
            {
                movement.y += 1;
                isMoving = true;
            }

            movement = movement.normalized;

            if (Input.GetKeyDown(sprint))
            {
                targetSpeed = movementSpeed + sprintingSpeed;
                playerAnimator.speed = 2;
                accessoireAnimator.speed = 2;
                sprintStartTime = Time.time;
            }
            
            if (Input.GetKey(sprint))
            {
                float currentTime = Time.time;
                sprintDuration = currentTime - sprintStartTime;

                while (sprintDuration >= 1f)
                {
                    GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.SPEEDRUNNER, 1, null);
                    sprintDuration -= 1f;
                    sprintStartTime += 1f;
                }
                audioSource.pitch = 1.75f;
            }

            if (Input.GetKeyUp(sprint) || busy)
            {
                targetSpeed = movementSpeed;
                playerAnimator.speed = 1;
                accessoireAnimator.speed = 1;
                sprintDuration = 0f; 
                audioSource.pitch = 1f;
            }

#if UNITY_EDITOR
            // dev keybindings
            if (Input.GetKeyDown("l") && targetSpeed == movementSpeed)
            {
                targetSpeed = targetSpeed + superSpeed;
                playerAnimator.speed = 20;
                accessoireAnimator.speed = 20;
            }

            if (Input.GetKeyUp("l") && targetSpeed == movementSpeed + superSpeed)
            {
                targetSpeed = targetSpeed - superSpeed;
                playerAnimator.speed = 1;
                accessoireAnimator.speed = 1;
            }
#endif
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 50);
            UpdateWalkAchievement();

#if UNITY_EDITOR
            // dev keybindings
            if (Input.GetKeyDown("k"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger =
                    !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
            }
#endif
            if (isMoving && !GameManager.Instance.GetIsPaused())
            {
                PlayMoveSound();
            }
            else
            {
                StopMoveSound();
            }
        }
        timeInGameDuration = Time.time - timeInGameStart;
        UpdateAchievementForTimeInGame();
    }

    /// <summary>
    ///     This function changes the animation of the character based on the outfit selected in the 
    ///     character selection. Also adjusts the hitbox or scaling for certain outfits.
    /// </summary>
    /// <param name="body"> A string that needs to match one from the validOutfits list. It determines which body animator to use. </param>
    /// <param name="head"> A string that needs to match one from the validOutfits list. It determines which accessory animator to use. </param>
    public void SetOutfitAnimator(string body, string head)
    {
        if (!validOutfits.Contains(body) || !validOutfits.Contains(head))
        {
            throw new ArgumentException("Provided argument(s) are invalid, provide an existing name of a outfit.");
        }

        string bodyPath = "AnimatorControllers/" + body;
        string headPath = "AnimatorControllers/" + head;
        accessoireAnimator.runtimeAnimatorController = Resources.Load(headPath) as RuntimeAnimatorController;
        playerAnimator.runtimeAnimatorController = Resources.Load(bodyPath) as RuntimeAnimatorController;

        if (head == "safety_helmet")
        {
            accessoireTransform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        } else
        {
            accessoireTransform.localScale = new Vector3(1, 1, 1);
        }

        if (new List<string> {"character_default", "character_blue_and_purple", "character_black_and_white"}.Contains(body))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.05f);
            accessoireTransform.localPosition = positions[head];
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.25f);
            accessoireTransform.localPosition = positions[head] - new Vector3(0, 0.3f, 0);
        }
    }

    /// <summary>
    ///     This function updates time achievements each 60 seconds when the player is playing
    /// </summary>
    private void UpdateAchievementForTimeInGame()
    {
        while (timeInGameDuration >= 60f)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.BEGINNER, 1, null);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.EXPERIENCED_PLAYER, 1, null);
            timeInGameDuration -= 60f;
            timeInGameStart += 60f;
        }
    }

    /// <summary>
    ///     depending on horizontalAnimationFloat and verticalAnimationFloat it is defined whether a vertical or horizontal
    ///     movement takes place then depending on movement.x and movement.y are looked to decide if movement is up/down or
    ///     left/right and depending on this the correct animation is executed
    /// </summary>
    private void FixedUpdate()
    {
        if (canMove)
        {
            float horizontalAnimationFloat = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
            float verticalAnimationFloat = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

            Vector2 newPosition = playerRigidBody.position + movement * currentSpeed * Time.fixedDeltaTime;

            playerRigidBody.MovePosition(newPosition);

            if (Vector2.Distance(playerRigidBody.position, newPosition) > positionThreshold)
            {
                EventManager.Instance.TriggerPositionChanged(newPosition, movement);
            }

            SetLookingDirection();

            playerAnimator.SetFloat("Horizontal", movement.x);
            playerAnimator.SetFloat("Vertical", movement.y);
            playerAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
            playerAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
            accessoireAnimator.SetFloat("Horizontal", movement.x);
            accessoireAnimator.SetFloat("Vertical", movement.y);
            accessoireAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
            accessoireAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
        }
    }

    /// <summary>
    ///     Sets the looking direction of the player, required for animation.
    /// </summary>
    private void SetLookingDirection()
    {
        if (movement.y > 0.01f)
        {
            playerAnimator.SetBool("LookUp", true);
            playerAnimator.SetBool("LookRight", false);
            accessoireAnimator.SetBool("LookUp", true);
            accessoireAnimator.SetBool("LookRight", false);
        }

        if (movement.y < 0f)
        {
            playerAnimator.SetBool("LookUp", false);
            playerAnimator.SetBool("LookRight", false);
            accessoireAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookRight", false);
        }

        if (movement.x > 0.01f)
        {
            playerAnimator.SetBool("LookRight", true);
            playerAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookRight", true);
        }

        if (movement.x < 0f)
        {
            playerAnimator.SetBool("LookRight", false);
            playerAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookRight", false);
        }
    }

    /// <summary>
    ///     This function updates the walk achievements
    /// </summary>
    private void UpdateWalkAchievement()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance <= 5)
        {
            distanceWalked += distance;
        }

        if (distanceWalked >= 1)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GO_FOR_A_WALK, 1, null);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GO_FOR_A_LONGER_WALK, 1, null);
            distanceWalked -= 1;
        }
        lastPosition = transform.position;
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    /// <summary>
    ///     This function updates the keybindings
    /// </summary>
    /// <param name="binding">The binding that changed</param>
    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.MOVE_UP)
        {
            moveUp = GameManager.Instance.GetKeyCode(Binding.MOVE_UP);
        }
        else if (binding == Binding.MOVE_LEFT)
        {
            moveLeft = GameManager.Instance.GetKeyCode(Binding.MOVE_LEFT);
        }
        else if (binding == Binding.MOVE_DOWN)
        {
            moveDown = GameManager.Instance.GetKeyCode(Binding.MOVE_DOWN);
        }
        else if (binding == Binding.MOVE_RIGHT)
        {
            moveRight = GameManager.Instance.GetKeyCode(Binding.MOVE_RIGHT);
        }
        else if (binding == Binding.SPRINT)
        {
            sprint = GameManager.Instance.GetKeyCode(Binding.SPRINT);
        }
    }

    /// <summary>
    ///     This function sets 'canMove' to false, so that the player can't move.
    /// </summary>
    public void DisableMovement()
    {
        canMove = false;
    }

    /// <summary>
    ///     This function sets 'canMove' to true, so that the player can move again.
    /// </summary>
    public void EnableMovement()
    {
        canMove = true;
    }

    /// <summary>
    ///     This method returns the current value of 'busy'.
    /// </summary>
    /// <returns>busy</returns>
    public bool IsBusy()
    {
        return busy;
    }

    /// <summary>
    ///     This functions sets the busy variable to status of 'busy' parameter.
    /// </summary>
    /// <param name="busy">true if busy, else false</param>
    public void SetBusy(bool busy)
    {
        this.busy = busy;
    }
    
    /// <summary>
    /// This function plays the movement sound.
    /// </summary>
    private void PlayMoveSound()
    {
        if (moveSound != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// This function stops the movement sound.
    /// </summary>
    public void StopMoveSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // tutorial only
        if (collision.CompareTag("Trigger"))
        {
            TutorialManager.Instance.ShowScreen();
        }

        // return to course selection page after completing tutorial
        if (collision.CompareTag("Tutorial"))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            CloseOverworld();
#endif
        }
    }


#region Singleton

    public static PlayerAnimation Instance { get; private set; }

    /// <summary>
    ///     The Awake function is called after an object is initialized and before the Start function.
    ///     It sets up the Singleton.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
#endregion

}