using UnityEngine;

/// <summary>
///     This class is part of an teleporter game object
/// </summary>
public class Teleporter : MonoBehaviour, IGameEntity<TeleporterData>
{
    [field: SerializeField] public string teleporterName { get; private set; } = "My Teleporter";
    [field: SerializeField] public int worldID { get; private set; } = 1;
    [field: SerializeField] public int dungeonID { get; private set; }
    [field: SerializeField] public int teleporterNumber { get; private set; } = 1;

    public bool isUnlocked { get; private set; }
    private string uuid;
    [SerializeField] private GameObject teleporterCanvas;
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteHighlight;

    private Transform player;

    private GameObject currentTeleporterCanvas;
    private TeleporterUI teleporterUI;

    private Vector2 finalTargetPosition;
    private int finalTargetWorld;
    private int finalTargetDungeon;

    private bool inTrigger;
    private bool interactable = true;

    public AudioClip teleporterOpeningSound;
    public AudioClip ufoTakesSound;
    public AudioClip ufoReturnsSound;
    private AudioSource audioSourceTeleport;
    private AudioSource audioSourceUfoTakes;
    private AudioSource audioSourceUfoReturns;

    //KeyCodes
    private KeyCode interact;

    private void Awake()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            ObjectManager.Instance.AddGameEntity<Teleporter, TeleporterData>(gameObject, worldID, dungeonID,
            teleporterNumber);
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
            GameEvents.current.onKeybindingChange += UpdateKeybindings;

            //add AudioSource component
            audioSourceTeleport = gameObject.AddComponent<AudioSource>();
            audioSourceUfoTakes = gameObject.AddComponent<AudioSource>();
            audioSourceUfoReturns = gameObject.AddComponent<AudioSource>();

            //Load the sound from Resources folder
            teleporterOpeningSound = Resources.Load<AudioClip>("Music/teleporter_opening");
            //set audio clip
            audioSourceTeleport.clip = teleporterOpeningSound;

            //Load the sound from Resources folder
            ufoTakesSound = Resources.Load<AudioClip>("Music/ufo_takes_the_player");
            //set audio clip
            audioSourceUfoTakes.clip = ufoTakesSound;

            //Load the sound from Resources folder
            ufoReturnsSound = Resources.Load<AudioClip>("Music/ufo_returns_the_player");
            //set audio clip
            audioSourceUfoReturns.clip = ufoTakesSound;
        }            
    }

    private void OnDestroy()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            ObjectManager.Instance.RemoveGameEntity<Teleporter, TeleporterData>(worldID, dungeonID, teleporterNumber);
            GameEvents.current.onKeybindingChange -= UpdateKeybindings;
        }            
    }

    private void Start()
    {
        SetUnLockedState(isUnlocked);

    }

    /// <summary>
    ///     Open TeleporterUI when the player interacts with the teleporter.
    /// </summary>
    private void Update()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            if (currentTeleporterCanvas != null && currentTeleporterCanvas.activeInHierarchy &&
            Input.GetKeyDown(interact) && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                Debug.Log("Close Teleporter UI");
                CloseTeleporterUI();
                return;
            }

            if (inTrigger && interactable && currentTeleporterCanvas == null && !PauseMenu.menuOpen &&
                !PauseMenu.subMenuOpen)
            {
                if (Input.GetKeyDown(interact))
                {
                    Debug.Log("Open Teleporter UI");
                    interactable = false;
                    GameObject newCanvas = Instantiate(teleporterCanvas);
                    teleporterUI = newCanvas.transform.GetChild(0).GetComponent<TeleporterUI>();
                    SetupTeleporterUI(teleporterUI);
                    currentTeleporterCanvas = newCanvas;
                    PlayTeleporterOpeningSound();
                }
            }
        }         
    }

    /// <summary>
    ///     This function initializes the <c>Teleporter</c> object
    /// </summary>
    /// <param name="areaIdentifier">The area the <c>Teleporter</c> is in</param>
    /// <param name="index">The index of the <c>Teleporter</c> in its area</param>
    public void Initialize(AreaInformation areaIdentifier, int index, string name)
    {
        worldID = areaIdentifier.GetWorldIndex();
        dungeonID = 0;
        if (areaIdentifier.IsDungeon())
        {
            dungeonID = areaIdentifier.GetDungeonIndex();
        }
        teleporterNumber = index;
        teleporterName = name;
    }

    /// <summary>
    ///     Recognize the player entering the teleporter. When this is the first time, it unlocks the teleporter and registers
    ///     it in the DataManager
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayTeleporterOpeningSound();
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            inTrigger = true;
            if (!isUnlocked)
            {
                isUnlocked = true;
                SetUnLockedState(isUnlocked);
                GameManager.Instance.ActivateTeleporter(worldID, dungeonID, teleporterNumber);
            }
        }
    }

    /// <summary>
    ///     Recognize the player exiting the teleporter
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CloseTeleporterUI();
            inTrigger = false;
        }
    }

    /// <summary>
    ///     Closes teleporter UI
    /// </summary>
    private void CloseTeleporterUI()
    {
        interactable = true;
        Destroy(currentTeleporterCanvas);
        currentTeleporterCanvas = null;
    }

    /// <summary>
    ///     Makes visual changes to the teleporter to indicate wether it is locked or not.
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
    ///     Setups the buttons inside the TeleporterUI.
    /// </summary>
    /// <param name="ui"></param>
    private void SetupTeleporterUI(TeleporterUI ui)
    {
        ui.SetupUI(this);
    }

    /// <summary>
    ///     This function is called by a button of the TeleporterUI. It will initialize the teleporting process.
    /// </summary>
    /// <param name="worldIndex"></param>
    public void TeleportPlayerTo(Vector2 position, int worldID, int dungeonID)
    {
        //setup scene transition exchange
        LoadSubScene.transitionBlocked = true;
        Optional<int> dungeonIndex = new Optional<int>();
        if(dungeonID != 0)
        {
            dungeonIndex.SetValue(dungeonID);
        }
        LoadSubScene.areaExchange = new AreaInformation(worldID, dungeonIndex);

        Debug.Log("TeleportPlayerTo start");
        Debug.Log("Teleport to: " + worldID + "-" + dungeonID + position.ToString());

        Destroy(currentTeleporterCanvas);
        finalTargetPosition = position;
        finalTargetWorld = worldID;
        finalTargetDungeon = dungeonID;
        Animation ufoAnimation = player.GetComponent<Animation>();
        player.GetComponent<PlayerAnimation>().DisableMovement();
        interactable = false;
        ufoAnimation.Play("UfoDeparture");
        PlayUfoTakesSound();
        Debug.Log("Animation length: " + ufoAnimation.clip.length);
        Invoke("FinishTeleportation", ufoAnimation.clip.length);
    }


    /// <summary>
    ///     This function is called by an animation event of the Ufo Animation. It loads the new world and teleports the player
    ///     there.
    /// </summary>
    public async void FinishTeleportation()
    {
        player.position = finalTargetPosition;

        Debug.Log("Final world:" + finalTargetWorld);
        Debug.Log("Final dungeon:" + finalTargetDungeon);

        if (finalTargetWorld != worldID || finalTargetDungeon != dungeonID)
        {
            GameManager.Instance.SetReloadLocation(finalTargetPosition, finalTargetWorld, finalTargetDungeon);
            await GameManager.Instance.ExecuteTeleportation();
        }
        else
        {
            player.position = finalTargetPosition;
        }
        Animation animation = player.GetComponent<Animation>();
        animation.Play("UfoArrival");
        PlayUfoReturnsSound();
        LoadSubScene.transitionBlocked = false;
    }


    public void Setup(TeleporterData data)
    {
        isUnlocked = data.isUnlocked;
        transform.position = data.position;
        teleporterName = data.teleporterName;
        worldID = data.worldID;
        dungeonID = data.dungeonID;
        teleporterNumber = data.teleporterNumber;
        SetUnLockedState(isUnlocked);
    }

    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.INTERACT)
        {
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        }
    }

    /// <summary>
    /// This function plays the teleporter opening sound.
    /// </summary>
    private void PlayTeleporterOpeningSound()
    {
        if (teleporterOpeningSound != null && audioSourceTeleport != null)
        {
            audioSourceTeleport.PlayOneShot(teleporterOpeningSound);
        }
    }

    /// <summary>
    /// This function plays the ufo sound.
    /// </summary>
    private void PlayUfoTakesSound()
    {
        if (ufoTakesSound != null && audioSourceUfoTakes != null)
        {
            audioSourceUfoTakes.PlayOneShot(ufoTakesSound);
        }
    }

    /// <summary>
    /// This function plays the ufo sound.
    /// </summary>
    private void PlayUfoReturnsSound()
    {
        if (ufoReturnsSound != null && audioSourceUfoReturns != null)
        {
            audioSourceUfoReturns.PlayOneShot(ufoReturnsSound);
        }
    }
}