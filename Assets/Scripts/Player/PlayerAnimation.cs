using UnityEngine;

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
    private readonly int achievementUpdateIntervall = 1;

    //KeyCodes
    private KeyCode moveUp;
    private KeyCode moveLeft;
    private KeyCode moveDown;
    private KeyCode moveRight;
    private KeyCode sprint;

    public AudioClip moveSound;
    private AudioSource audioSource;
    private bool isMoving;


    /// <summary>
    ///     This method is called before the first frame update.
    ///     It is used to initialize variables.
    /// </summary>
    private void Start()
    {
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
                audioSource.pitch = 1.75f;
            }

            if (Input.GetKeyUp(sprint))
            {
                targetSpeed = movementSpeed;
                playerAnimator.speed = 1;
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

            UpdateAchievement();

            // dev keybindings
            if (Input.GetKeyDown("k"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger =
                    !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
            }
            // dev keybindings

            if (isMoving)
            {
                PlayMoveSound();
            }
            else
            {
                StopMoveSound();
            }
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