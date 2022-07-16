using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public float movementSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float superSpeed = 20f;
    private float currentSpeed;
    public Vector2 movement;
    public Rigidbody2D rigidbody;


    public Animator playerAnimator;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
        currentSpeed = movementSpeed;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift) && currentSpeed == movementSpeed)
        {
            currentSpeed = currentSpeed + sprintingSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && currentSpeed == (movementSpeed + sprintingSpeed))
        {
            currentSpeed = currentSpeed - sprintingSpeed;
        }

        if (Input.GetKeyDown("l") && currentSpeed == movementSpeed)
        {
            currentSpeed = currentSpeed + superSpeed;
        }
        if (Input.GetKeyUp("l") && currentSpeed == (movementSpeed + superSpeed))
        {
            currentSpeed = currentSpeed - superSpeed;
        }

    }

    //depending on horizontalAnimationFloat and verticalAnimationFloat it is defined whether a vertical or horizontal movement takes place
    //then depending on movement.x and movement.y are looked to decide if movement is up/down or left/right and depending on this the correct 
    //animation is executed
    void FixedUpdate()
    {
        float horizontalAnimationFloat = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
        float verticalAnimationFloat = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

        rigidbody.MovePosition(rigidbody.position + movement * currentSpeed * Time.fixedDeltaTime);

        playerAnimator.SetFloat("Horizontal", movement.x);
        playerAnimator.SetFloat("Vertical", movement.y);
        playerAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
        playerAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
    }
}
