using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mymove : MonoBehaviour
{

    public float moveSpeed = 4f;
    public float moveSpeedSprint;
    public float currentSpeedX;
    public float currentSpeedY;
    public float currentSpeedDeltaTime;
    public VectorValue startingPosition;

    public Rigidbody2D rb;

    public Vector2 movement;

    public static bool menuOpen = false;

    private void Start()
    {
        transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = moveSpeed * 1.5f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 4f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rb.transform.position = new Vector2(0, 0);
        }
        //esc handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //close menu when its opened
            if (menuOpen)
            {
                SceneManager.UnloadScene("Menu");
                menuOpen = false;
            }
            //open menu when its not opened
            else
            {
                menuOpen = true;
                SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
            }
        }
    }

    void FixedUpdate()
    {
        if (movement.x != 0 && movement.y != 0)
        {
            rb.MovePosition(rb.position + (movement * 0.75f) * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        currentSpeedX = movement.x;
        currentSpeedY = movement.y;
        currentSpeedDeltaTime = Time.fixedDeltaTime;
    }

}
