using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    #region Singleton
    public static Animation instance { get; private set; }

    /*
     * The Awake function is called after an object is initialized and before the Start function.
     * It sets up the Singleton. 
     */
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public float movementSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float superSpeed = 20f;
    private float currentSpeed;
    public Vector2 movement;
    public Rigidbody2D playerRigidBody;
    private bool canMove;
    private bool busy;
    public Animator playerAnimator;

    void Start()
    {
        canMove = true;
        busy = false;
        playerRigidBody = this.GetComponent<Rigidbody2D>();
        currentSpeed = movementSpeed;
    }

    void Update()
    {
        if(canMove)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement = movement.normalized;

            if (Input.GetKeyDown(KeyCode.LeftShift) && currentSpeed == movementSpeed)
            {
                currentSpeed = currentSpeed + sprintingSpeed;
                playerAnimator.speed = 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && currentSpeed == (movementSpeed + sprintingSpeed))
            {
                currentSpeed = currentSpeed - sprintingSpeed;
                playerAnimator.speed = 1;
            }

            if (Input.GetKeyDown("l") && currentSpeed == movementSpeed)
            {
                currentSpeed = currentSpeed + superSpeed;
                playerAnimator.speed = 20;
            }
            if (Input.GetKeyUp("l") && currentSpeed == (movementSpeed + superSpeed))
            {
                currentSpeed = currentSpeed - superSpeed;
                playerAnimator.speed = 1;
            }

            if (Input.GetKeyDown("k"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger = !GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().isTrigger;
            }
        }
    }

    //depending on horizontalAnimationFloat and verticalAnimationFloat it is defined whether a vertical or horizontal movement takes place
    //then depending on movement.x and movement.y are looked to decide if movement is up/down or left/right and depending on this the correct 
    //animation is executed
    void FixedUpdate()
    {
        if(canMove)
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

    public void disableMovement()
    {
        canMove = false;
    }

    public void enableMovement()
    {
        canMove = true;
    }

    public bool isBusy()
    {
        return busy;
    }

    public void setBusy(bool busy)
    {
        this.busy = busy;
    }
}
