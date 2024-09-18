using UnityEngine;
using System;
using System.Collections;

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
    public Animator accessoireAnimator;
    private bool busy;
    private bool canMove;
    private float currentSpeed;
    private float targetSpeed;
    private int volumeLevel;

    private Vector3 lastPosition;
    private float distanceWalked;
    private readonly int achievementUpdateIntervall = 1;

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
    private DateTime lastPlayDate;
    private bool checkIfChanged=false;
    /// <summary>
    ///     This method is called before the first frame update.
    ///     It is used to initialize variables.
    /// </summary>
    private void Start()
    {
        accessoireAnimator = this.gameObject.transform.GetChild(2).GetComponent<Animator>();
        string lastPlayDateStr = PlayerPrefs.GetString("LastPlayDate", "");
        int daysCount = PlayerPrefs.GetInt("DaysPlayed", 0);

        if (!string.IsNullOrEmpty(lastPlayDateStr))
        {
            lastPlayDate = DateTime.Parse(lastPlayDateStr);
            DateTime today = DateTime.Today;
            DateTime lastPlayDay = lastPlayDate.Date;

            if (lastPlayDay < today)
            {
                int daysSinceLastPlay = (today - lastPlayDay).Days;
                daysPlayed = daysCount + daysSinceLastPlay;
                PlayerPrefs.SetInt("DaysPlayed", daysPlayed);
            }
            else
            {
                daysPlayed = daysCount;
            }
        }
        else
        {
            lastPlayDate = DateTime.Now;
            daysPlayed = 1;
            PlayerPrefs.SetInt("DaysPlayed", daysPlayed);
        }

        if (daysPlayed > daysCount)
        {
            checkIfChanged=true;
            
            Debug.Log("success!!!!!");
        }
        PlayerPrefs.SetString("LastPlayDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")); // store the current date and time with milliseconds
        Debug.Log("day: days played "+daysPlayed);
        Debug.Log("day: current date "+DateTime.Now);
        Debug.Log("day: last play date "+lastPlayDate);
        
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

        //get AudioSource component
        audioSource = GetComponent<AudioSource>();
        //add AudioSource component if necessary
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        //set audio clip
        audioSource.clip = moveSound;
        //set AudioSource to loop
        audioSource.loop = true;
        //AudioSource does not start playing automatically when the GameObject awakens
        audioSource.playOnAwake = false;

        volumeLevel = PlayerPrefs.GetInt("VolumeLevel", 3);
        UpdateVolume();
    }

    /// <summary>
    /// This function updates the level volume and applies the changes to all audio in the game
    /// </summary>
    private void UpdateVolume()
    {
        float volume = 0f;
        switch (volumeLevel)
        {
            case 0:
                volume = 0f;
                break;
            case 1:
                volume = 0.5f;
                break;
            case 2:
                volume = 1f;
                break;
            case 3:
                volume = 2f;
                break;
        }
        AudioListener.volume = volume;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastPlayDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        PlayerPrefs.SetInt("DaysPlayed", daysPlayed);
        PlayerPrefs.Save();
    }

    /// <summary>
    ///     If 'canMove' is true, this function allows the player to move.
    /// </summary>
    private void Update()
    {
        if(checkIfChanged){
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GAMER, 1);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.PROFESSIONAL_GAMER, 1);
            checkIfChanged=false;
            Debug.Log("success in update");
        }
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
                    GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.SPEEDRUNNER, 1);
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

            /*if (Input.GetKeyDown("x"))
            {
                accessoireAnimator.runtimeAnimatorController = Resources.Load("AnimatorControllers/retro_brille") as RuntimeAnimatorController;
                playerAnimator.runtimeAnimatorController = Resources.Load("AnimatorControllers/character_ironman") as RuntimeAnimatorController;
            }*/
            // dev keybindings

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 50);

            UpdateAchievement();

            // dev keybindings
            if (Input.GetKeyDown("k"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger =
                    !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
            }
            // dev keybindings
            if (isMoving && !GameManager.Instance.isPaused)
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
    ///     this function changes the animation of the character based on the outfit selected in the 
    ///     character selection
    /// </summary>
    public void SetOutfitAnimator(String body, String head)
    {
        if (!validOutfits.Contains(body) || !validOutfits.Contains(head))
        {
            throw ArgumentException;
        }

        String bodyPath = "AnimatorControllers/" + body;
        String headPath = "AnimatorControllers/" + head;
        accessoireAnimator.runtimeAnimatorController = Resources.Load(headPath) as RuntimeAnimatorController;
        playerAnimator.runtimeAnimatorController = Resources.Load(bodyPath) as RuntimeAnimatorController;

        // TODO adjust offset from list
        offset = offsets[head];
        
        
    }

    ArrayList<String> validOutfits = {"3D_brille", "blonde_haare", "character_anzug", "character_black_and_white", "character_blue_and_purple", "character_default", "character_ironman", 
        "character_jeans_karo", "character_lange_haare", "character_santa", "character_trainingsanzug", "coole_brille", "flammen_haare", "globus_hut", "herzbrille", "retro_brille", "schutzhelm"};

    IDictionary<String, Offset> offsets = {} // TODO fill with outfit offset pairs of the head accesoires

    struct Coordinates
    {
        int x;
        int y;
        int z;
    }

    struct Offset
    {
        Coordinates position;
        Coordinates Rotation;
        Coordinates Scale;
    }

    /// <summary>
    ///     this function updates time achievement each 60 seconds when the player is playing
    /// </summary>
    private void UpdateAchievementForTimeInGame()
    {
        while (timeInGameDuration >= 60f)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.BEGINNER, 1);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.EXPERIENCED_PLAYER, 1);
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
                accessoireAnimator.SetBool("LookUp", true);
                accessoireAnimator.SetBool("LookRight", false);
            }

            if
                (movement.y < 0f)
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

            if
                (movement.x < 0f)
            {
                playerAnimator.SetBool("LookRight", false);
                playerAnimator.SetBool("LookUp", false);
                accessoireAnimator.SetBool("LookUp", false);
                accessoireAnimator.SetBool("LookRight", false);
            }

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

    private void UpdateAchievement()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance <= 5)
        {
            distanceWalked += distance;
        }

        if (distanceWalked >= achievementUpdateIntervall)
        {
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GO_FOR_A_WALK,
                achievementUpdateIntervall);
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.GO_FOR_A_LONGER_WALK,
                achievementUpdateIntervall);
            distanceWalked -= achievementUpdateIntervall;
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
    /// <summary>
    ///     Resets the current instance to null.
    /// </summary>
    public void ResetInstance()
    {
        Instance = null;
    }

    #endregion
}