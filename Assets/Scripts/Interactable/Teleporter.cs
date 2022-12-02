using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Teleporter : MonoBehaviour
{
    public bool unlocked = false;
    [SerializeField]
    private int teleporterWorldID;
    [SerializeField]
    private GameObject teleporterCanvas;

    [SerializeField]
    private Vector2[] teleporterPositions = new Vector2[4];


    private Teleporter[] registeredTeleporters;

    private Transform player;

    private GameObject currentTeleporterCanvas;
    private TeleporterUI teleporterUI;

    private int finalTargetIndex = 0;
    private bool inTrigger = false;
    private bool interactable = true;

    private void Start()
    {
        // Receive teleporter data
        //registeredTeleporters = ...
        SetUnLockedState(unlocked);
    }

    /// <summary>
    /// Open TeleporterUI when the player interacts with the teleporter.
    /// </summary>
    void Update()
    {
        if (inTrigger && interactable && currentTeleporterCanvas == null)
        {
            if (Input.GetKeyDown("e"))
            {
                interactable = false;
                GameObject newCanvas = GameObject.Instantiate(teleporterCanvas);
                teleporterUI = newCanvas.transform.GetChild(0).GetComponent<TeleporterUI>();
                SetupTeleporterUI(teleporterUI);
                currentTeleporterCanvas = newCanvas;
            }
            
        }
        

    }
    /// <summary>
    /// Recognize the player entering the teleporter
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            inTrigger = true;
            if (!unlocked)
            {
                unlocked = true;
                SetUnLockedState(unlocked);
                // Register teleport in datamanager
            }
        }
    }

    /// <summary>
    /// Recognize the player exiting the teleporter
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactable = true;
            inTrigger = false;
            Destroy(currentTeleporterCanvas);
            currentTeleporterCanvas = null;

        }
    }

    private void SetUnLockedState(bool isUnLocked)
    {
        interactable = true;
        if (isUnLocked)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        
    }


    /// <summary>
    /// Setups the buttons inside the TeleporterUI.
    /// </summary>
    /// <param name="ui"></param>
    private void SetupTeleporterUI(TeleporterUI ui)
    {
        List<int> destinationIndices = new List<int> { 0,1, 2, 3};
        destinationIndices.Remove(teleporterWorldID);
        for (int i = 0; i < destinationIndices.Count; i++)
        {
            bool worldUnlocked = true;
            //bool worldUnlocked = DataManager.Instance.IsWorldUnlocked(destinationIndices[i]);
            Button currentButton = ui.targetButtons[i];
            //string worldName = ?
            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "World " + (destinationIndices[i]+1).ToString();
            currentButton.interactable = worldUnlocked;
            int index = i;
            currentButton.onClick.AddListener(() => TeleportPlayerToWorld(destinationIndices[index]));
        }     
    }

    /// <summary>
    /// This function is called by a button of the TeleporterUI. It will teleport the player to a specific location.
    /// </summary>
    /// <param name="worldIndex"></param>
    public void TeleportPlayerToWorld(int worldIndex)
    {
        Destroy(currentTeleporterCanvas);
        finalTargetIndex = worldIndex;
        Animation ufoAnimation = GetComponent<Animation>();
        ufoAnimation.Play();
        player.GetComponent<PlayerAnimation>().DisableMovement();
        
    }

    /// <summary>
    /// This function is called by an animation event of the Ufo Animation. It loads the new world and teleports the player there.
    /// </summary>
    public void FinishTeleportation()
    {
        player.position = teleporterPositions[finalTargetIndex];
        player.GetComponent<PlayerAnimation>().EnableMovement();
        player.GetComponent<SpriteRenderer>().enabled = true;
        interactable = true;
    }


    /// <summary>
    /// This function is called by an animation event of the Ufo Animation. It hides the player for some time.
    /// </summary>
    public void FadeOutPlayer()
    {
        player.GetComponent<SpriteRenderer>().enabled = false;
    }
}
