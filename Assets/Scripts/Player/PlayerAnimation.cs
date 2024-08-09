using UnityEngine;
using System;
using UnityEngine.UIElements;

/// <summary>
///     This class manages the movement and the animations of the player.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float superSpeed = 20f;
    public Vector2 movement;
    public Rigidbody2D playerRigidBody;
    public Animator playerAnimator;
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
    
    private int daysPlayed;
    private DateTime lastLoginDate;

    /// <summary>
    ///     This method is called before the first frame update.
    ///     It is used to initialize variables.
    /// </summary>
    private void Start()
    { 
        timeInGameStart=Time.time;

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

        LoadSavedCharacter();
        InitializeAudio();
        Invoke("CheckForLastLogin", 4.5f);
    }

    private void LoadSavedCharacter()
    {
        // get player components
        SpriteRenderer currentSprite = GetComponent<SpriteRenderer>();
        Animator currentAnimator = GetComponent<Animator>();
        Image characterHead = GameObject.Find("Player Face").GetComponent<Image>();

        // initialize the saved player sprite, animations and image on the minimap
        int currentIndex = DataManager.Instance.GetCharacterIndex();
        currentSprite.sprite = DataManager.Instance.GetCharacterSprites()[currentIndex];
        currentAnimator.runtimeAnimatorController = DataManager.Instance.GetCharacterAnimators()[currentIndex];
        characterHead.sprite = DataManager.Instance.GetCharacterHeads()[currentIndex];
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
    ///     This function checks if a new day has already started since the player's last login and if so, the achievement for the login is updated
    /// </summary>
    private void CheckForLastLogin()
    {
        string lastLoginDateStr = PlayerPrefs.GetString("LastLoginDate", "");
        int daysCount = PlayerPrefs.GetInt("DaysPlayed", 0);

        DateTime today = DateTime.Today;
        if (!string.IsNullOrEmpty(lastLoginDateStr))
        {
            lastLoginDate = DateTime.Parse(lastLoginDateStr);

            if (lastLoginDate.Date < today)
            {
                daysPlayed = daysCount + 1;
                UpdateLoginAchievement();
            }
            else
            {
                daysPlayed = daysCount;
            }
        }
        else
        {
            lastLoginDate = DateTime.Now;
            daysPlayed = 1;
            UpdateLoginAchievement();
        }

        PlayerPrefs.SetString("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt("DaysPlayed", daysPlayed);
        PlayerPrefs.Save();
        Debug.Log("day: days played " + daysPlayed);
        Debug.Log("day: current date " + DateTime.Now);
        Debug.Log("day: last play date " + lastLoginDate);
    }

    /// <summary>
    ///     This function updates login achievements
    /// </summary>
    private void UpdateLoginAchievement()
    {
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GAMER, 1, null);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.PROFESSIONAL_GAMER, 1, null);
    }
    
    /// <summary>
    ///     If 'canMove' is true, this function allows the player to move.
    /// </summary>
    private void Update()
    { 
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

            if (Input.GetKeyUp(sprint))
            {
                targetSpeed = movementSpeed;
                playerAnimator.speed = 1;
                sprintDuration = 0f; 
                audioSource.pitch = 1f;
            }

            // dev keybindings
            if (Input.GetKeyDown("l") && targetSpeed == movementSpeed)
            {
                targetSpeed = targetSpeed + superSpeed;
                playerAnimator.speed = 20;
            }

            if (Input.GetKeyUp("l") && targetSpeed == movementSpeed + superSpeed)
            {
                targetSpeed = targetSpeed - superSpeed;
                playerAnimator.speed = 1;
            }
            // dev keybindings

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 50);

            UpdateWalkAchievement();

            // dev keybindings
            if (Input.GetKeyDown("k"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger =
                    !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
            }
            // dev keybindings
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

            playerRigidBody.MovePosition(playerRigidBody.position + movement * currentSpeed * Time.fixedDeltaTime);

            // the following 4 if statements set the looking direction of the player, which we need for the animation
            if (movement.y > 0.01f)
            {
                playerAnimator.SetBool("LookUp", true);
                playerAnimator.SetBool("LookRight", false);
            }

            if
                (movement.y < 0f)
            {
                playerAnimator.SetBool("LookUp", false);
                playerAnimator.SetBool("LookRight", false);
            }

            if (movement.x > 0.01f)
            {
                playerAnimator.SetBool("LookRight", true);
                playerAnimator.SetBool("LookUp", false);
            }

            if
                (movement.x < 0f)
            {
                playerAnimator.SetBool("LookRight", false);
                playerAnimator.SetBool("LookUp", false);
            }

            playerAnimator.SetFloat("Horizontal", movement.x);
            playerAnimator.SetFloat("Vertical", movement.y);
            playerAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
            playerAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
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
    private void StopMoveSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
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