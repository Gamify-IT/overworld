using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaySpotInteraction : MonoBehaviour
{

    [Header("Replay Menu")]
    public GameObject replayMenu; 
    public Button closeReplayMenuButton;

    private bool playerInRange = false;

    private void Start()
    {
        replayMenu.SetActive(false);
        closeReplayMenuButton.onClick.AddListener(CloseReplayMenu);
    }

    private void Update()
    {
        // Check for player input when in range
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            EnterReplayMenu();
        }
    }

    // Trigger when the player enters the range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Trigger when the player exits the range
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Show the Replay Menu when the player is in range and presses E
    private void EnterReplayMenu()
    {
        replayMenu.SetActive(true);
        // Pause the game
        Time.timeScale = 0f;
    }

    // Close the Replay Menu and resume the game
    private void CloseReplayMenu()
    {
        replayMenu.SetActive(false); 
        Time.timeScale = 1f;         
    }
}

