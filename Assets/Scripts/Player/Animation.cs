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
    public float hf = 0.0f;
    public float vf = 0.0f;

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

    void FixedUpdate()
    {
        hf = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
        vf = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

        rigidbody.MovePosition(rigidbody.position + movement * currentSpeed * Time.fixedDeltaTime);

        playerAnimator.SetFloat("Horizontal", movement.x);
        playerAnimator.SetFloat("Vertical", movement.y);
        playerAnimator.SetFloat("VerticalSpeed", vf);
        playerAnimator.SetFloat("HorizontalSpeed", hf);
    }
}
