using UnityEngine;

/*
 * this script manages the player animations
 */
public class PlayerMovementAnimation : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float superSpeed = 20f;
    private float currentSpeed;
    public Vector2 movement;
    public Rigidbody2D playerRigidBody;


    public Animator playerAnimator;

    void Start()
    {
        playerRigidBody = this.GetComponent<Rigidbody2D>();
        currentSpeed = movementSpeed;
    }

    /*
     * The Update function is called every frame. It updates all values according to the changes happened since the last frame. 
     * It manages the walking animations of the player
     */
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        //when holding Shift Key the player sprints. Animations are faster
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentSpeed == movementSpeed)
        {
            currentSpeed += sprintingSpeed;
            playerAnimator.speed = 2;
        }

        //if no longer holding the shift key, revert animation speed and walking speed
        if (Input.GetKeyUp(KeyCode.LeftShift) && currentSpeed == (movementSpeed + sprintingSpeed))
        {
            currentSpeed -= sprintingSpeed;
            playerAnimator.speed = 1;
        }

        //super fast sprinting, only used for debug purpose
        if (Input.GetKeyDown("l") && currentSpeed == movementSpeed)
        {
            currentSpeed += superSpeed;
            playerAnimator.speed = 20;
        }

        if (Input.GetKeyUp("l") && currentSpeed == (movementSpeed + superSpeed))
        {
            currentSpeed -= superSpeed;
            playerAnimator.speed = 1;
        }

        //for debug purpose it is possible to deactivate the Colliders with a keypress
        if (Input.GetKeyDown("k"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger =
                !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
        }
    }

    /*
     * depending on horizontalAnimationFloat and verticalAnimationFloat it is defined whether a vertical or horizontal movement takes place
     * then depending on movement.x and movement.y are looked to decide if movement is up/down or left/right and depending on this the correct 
     * animation is executed
     */
    void FixedUpdate()
    {
        float horizontalAnimationFloat = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
        float verticalAnimationFloat = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

        playerRigidBody.MovePosition(playerRigidBody.position + movement * currentSpeed * Time.fixedDeltaTime);

        playerAnimator.SetFloat("Horizontal", movement.x);
        playerAnimator.SetFloat("Vertical", movement.y);
        playerAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
        playerAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
    }
}