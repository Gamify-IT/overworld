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
    private bool isDead = false;

    public Rigidbody2D rb;

    public Vector2 movement;

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            moveSpeed = moveSpeed * 1.5f;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) {
            moveSpeed = 4f;
        }
       
        if(Input.GetKeyDown(KeyCode.R)) {
            rb.transform.position = new Vector2(0, 0);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }

    }

    void FixedUpdate() {
        if(movement.x != 0 && movement.y != 0) {
            rb.MovePosition(rb.position + (movement * 0.75f) * moveSpeed * Time.fixedDeltaTime);
        }
        else {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        
        currentSpeedX = movement.x;
        currentSpeedY = movement.y;
        currentSpeedDeltaTime =  Time.fixedDeltaTime;
    }

}
