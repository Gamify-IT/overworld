using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Teleporter : MonoBehaviour
{
    [field: SerializeField]
    public string teleporterName { get; private set; } = "My Teleporter";
    [field: SerializeField]
    public int worldID { get; private set; } = 1;
    [field: SerializeField]
    public int dungeonID { get; private set; } = 0;
    [field: SerializeField]
    public int teleporterNumber { get; private set; } = 1;

    private bool isUnlocked = false;
    [SerializeField]
    private GameObject teleporterCanvas;
    [SerializeField]
    private Sprite spriteNormal;
    [SerializeField]
    private Sprite spriteHighlight;

    private Transform player;

    private GameObject currentTeleporterCanvas;
    private TeleporterUI teleporterUI;

    private Vector2 finalTargetPosition;
    private int finalTargetWorld;
    private int finalTargetDungeon;

    private bool inTrigger = false;
    private bool interactable = true;

    private void Awake()
    {
        ObjectManager.Instance.AddTeleporter(this.gameObject, worldID, dungeonID, teleporterNumber);
    }
    private void OnDestroy()
    {
        ObjectManager.Instance.RemoveTeleporter(worldID, dungeonID, teleporterNumber);
    }

    private void Start()
    {
        SetUnLockedState(isUnlocked);
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
            if (!isUnlocked)
            {
                isUnlocked = true;
                SetUnLockedState(isUnlocked);
                WorldData worldData = DataManager.Instance.GetWorldData(worldID);
                worldData.UpdateTeleporterData(teleporterNumber, teleporterName, worldID, dungeonID, transform.position);       
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

    /// <summary>
    /// This function sets up the teleporter based on the provided data
    /// </summary>
    /// <param name="data"></param>
    public void Setup(TeleporterData data)
    {
        isUnlocked = data.isUnlocked;
        SetUnLockedState(isUnlocked);
    }


    /// <summary>
    /// Makes visual changes to the teleporter to indicate wether it is locked or not.
    /// </summary>
    /// <param name="isUnLocked"></param>
    private void SetUnLockedState(bool isUnLocked)
    {
        interactable = true;
        if (isUnLocked)
        {
            GetComponent<SpriteRenderer>().sprite = spriteHighlight;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = spriteNormal;
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

        Debug.Log("Final world:" + finalTargetWorld);
        Debug.Log("Final dungeon:" + finalTargetDungeon);

        if (finalTargetWorld != worldID || finalTargetDungeon != dungeonID)
        {
            GameManager.Instance.SetReloadLocation(finalTargetPosition, finalTargetWorld, finalTargetDungeon);
            GameManager.Instance.ExecuteTeleportation();
        }
        else
        {
            player.position = finalTargetPosition;
        }
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
