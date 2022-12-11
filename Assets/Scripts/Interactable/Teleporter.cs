using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Teleporter : MonoBehaviour
{
    public string teleporterName = "MyTeleporter";
    public int worldID;
    public bool unlocked = false;
    public int dungeonID = 0;
    [SerializeField]
    private GameObject teleporterCanvas;

    private Transform player;

    private GameObject currentTeleporterCanvas;
    private TeleporterUI teleporterUI;

    private Vector2 finalTargetPosition;
    private int finalTargetWorld;
    private int finalTargetDungeon;

    private bool inTrigger = false;
    private bool interactable = true;

    private void Start()
    {
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
    /// Recognize the player entering the teleporter. When this is the first time, it unlocks the teleporter and registers it in the DataManager
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
                DataManager dataManager = DataManager.Instance;
                if (!dataManager.registeredTeleporters.ContainsKey(worldID)){
                    dataManager.registeredTeleporters.Add(worldID, new List<Teleporter>());
                }
                dataManager.registeredTeleporters[worldID].Add(this);               
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
            GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f);;
        } 
    }


    /// <summary>
    /// Setups the buttons inside the TeleporterUI.
    /// </summary>
    /// <param name="ui"></param>
    private void SetupTeleporterUI(TeleporterUI ui)
    {
        ui.SetupUI(this);
    }

    /// <summary>
    /// This function is called by a button of the TeleporterUI. It will initialize the teleporting process.
    /// </summary>
    /// <param name="worldIndex"></param>
    public void TeleportPlayerTo(Vector2 position,int worldID,int dungeonID)
    {
        Destroy(currentTeleporterCanvas);
        finalTargetPosition = position;
        finalTargetWorld = worldID;
        finalTargetDungeon = dungeonID;
        Animation ufoAnimation = GetComponent<Animation>();
        ufoAnimation.Play();
        player.GetComponent<PlayerAnimation>().DisableMovement();
        
    }

    /// <summary>
    /// This function is called by an animation event of the Ufo Animation. It loads the new world and teleports the player there.
    /// </summary>
    public void FinishTeleportation()
    {
        player.position = finalTargetPosition;

        Debug.Log("Final world" + finalTargetWorld);
        Debug.Log("Final dungeon" + finalTargetDungeon);
        GameManager.Instance.SetReloadLocation(finalTargetPosition, finalTargetWorld, finalTargetDungeon);
        GameManager.Instance.ExecuteTeleportation();
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
