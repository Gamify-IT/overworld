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
    }

    /// <summary>
    ///     If 'canMove' is true, this function allows the player to move.
    /// </summary>
    private void Update()
    {
        if (canMove)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement = movement.normalized;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                targetSpeed = movementSpeed + sprintingSpeed;
                playerAnimator.speed = 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                targetSpeed = movementSpeed;
                playerAnimator.speed = 1;
            }
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 50);

            // The following lines are dev keybindings, if needed they can be activated again by uncommenting them
            if (Input.GetKeyDown("l") && currentSpeed == movementSpeed)
            {
                currentSpeed = currentSpeed + superSpeed;
                playerAnimator.speed = 20;
            }

            if (Input.GetKeyUp("l") && currentSpeed == movementSpeed + superSpeed)
            {
                currentSpeed = currentSpeed - superSpeed;
                playerAnimator.speed = 1;
            }

            if (Input.GetKeyDown("k"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger =
                    !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
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
