using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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

    private static List<(int, int, int)> unlockedTeleporters;
    public AudioClip teleporterOpeningSound;
    public AudioClip ufoTakesSound;
    public AudioClip ufoReturnsSound;

    private AudioSource audioSourceTeleport;
    private AudioSource audioSourceUfoTakes;
    private AudioSource audioSourceUfoReturns;
    
    private bool isUfoSoundPlaying = false;

    //KeyCodes
    private KeyCode interact;

    private void Awake()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            ObjectManager.Instance.AddGameEntity<Teleporter, TeleporterData>(gameObject, worldID, dungeonID, teleporterNumber);
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
            GameEvents.current.onKeybindingChange += UpdateKeybindings;
            InitializeAudio();            
        }
#if !UNITY_EDITOR
        unlockedTeleporters = DataManager.Instance.GetAchievements().Find(achievement => achievement.GetTitle() == "TELEPORTER_MASTER").GetInteractedObjects();    
#endif
    }

    /// <summary>
    ///     This function adds new audio sources and loads all necessary audio clips
    /// </summary>
    private void InitializeAudio()
    {
        audioSourceTeleport = gameObject.AddComponent<AudioSource>();
        audioSourceUfoTakes = gameObject.AddComponent<AudioSource>();
        audioSourceUfoReturns = gameObject.AddComponent<AudioSource>();

        teleporterOpeningSound = Resources.Load<AudioClip>("Music/teleporter_opening");
        audioSourceTeleport.clip = teleporterOpeningSound;

        ufoTakesSound = Resources.Load<AudioClip>("Music/ufo_takes_the_player");
        audioSourceUfoTakes.clip = ufoTakesSound;

        ufoReturnsSound = Resources.Load<AudioClip>("Music/ufo_returns_the_player");
        audioSourceUfoReturns.clip = ufoReturnsSound;
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
            if (IsUfoArrivalAnimationPlaying())
            {
                PlayUfoReturnsSound();
                return;
            }
            if (IsUfoDepartureAnimationPlaying())
            {
                PlayUfoTakesSound();
                return;
            }

            if (currentTeleporterCanvas != null && currentTeleporterCanvas.activeInHierarchy &&
            Input.GetKeyDown(interact) && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                CloseTeleporterUI();
                return;
            }

            if (inTrigger && interactable && currentTeleporterCanvas == null && !PauseMenu.menuOpen &&
                !PauseMenu.subMenuOpen)
            {
                if (Input.GetKeyDown(interact))
                {
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
    ///     This function checks if the ufo arrival animation playing
    /// </summary>
    private bool IsUfoArrivalAnimationPlaying()
    {
        if (player == null)
        {
            return false;
        }
        Animation animation = player.GetComponent<Animation>();
        if (animation == null)
        {
            return false;
        } 
        if (animation.IsPlaying("UfoArrival"))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    ///     This function checks if the ufo departure animation playing
    /// </summary>
    private bool IsUfoDepartureAnimationPlaying()
    {
        if (player == null)
        {
            return false;
        }
        Animation animation = player.GetComponent<Animation>();
        if (animation == null)
        {
            return false;
        }
        if (animation.IsPlaying("UfoDeparture"))
        {
            return true;
        }
        return false;
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
                UpdateListOfOpenedTeleporters();
            }
        }
    }

    /// <summary>
    ///     This method adds a new opened teleporter to the list and 
    ///     calls the method for the achievement update by interacting with the new teleporter. 
    /// </summary>
    private void UpdateListOfOpenedTeleporters()
    {
        var key = (worldID, dungeonID, teleporterNumber);
        if(!unlockedTeleporters.Contains(key))
        {
            unlockedTeleporters.Add(key);
            UpdateAchievements(worldID);
        } 
    }

    /// <summary>
    ///     This method updates the "open teleporters" achievements in general and for a specific world.
    /// </summary>
    /// <param name="worldNumber">The number of the world in which is the opened teleporter</param>
    private void UpdateAchievements(int worldNumber)
    {
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.TELEPORTER_BEGINNER, 1, unlockedTeleporters);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.TELEPORTER_PROFESSIONAL, 1, unlockedTeleporters);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.TELEPORTER_MASTER, 1, unlockedTeleporters);

        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"TELEPORTER_BEGINNER_WORLD_{worldNumber}"), 1, unlockedTeleporters);
        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"TELEPORTER_PROFESSIONAL_WORLD_{worldNumber}"), 1, unlockedTeleporters);
        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"TELEPORTER_MASTER_WORLD_{worldNumber}"), 1, unlockedTeleporters);
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
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.TRAVELER, 1, null);
        //setup scene transition exchange
        LoadSubScene.transitionBlocked = true;
        Optional<int> dungeonIndex = new Optional<int>();
        if(dungeonID != 0)
        {
            dungeonIndex.SetValue(dungeonID);
        }
        LoadSubScene.areaExchange = new AreaInformation(worldID, dungeonIndex);

        Destroy(currentTeleporterCanvas);
        finalTargetPosition = position;
        finalTargetWorld = worldID;
        finalTargetDungeon = dungeonID;
        Animation ufoAnimation = player.GetComponent<Animation>();
        player.GetComponent<PlayerAnimation>().DisableMovement();
        interactable = false;
        ufoAnimation.Play("UfoDeparture");
        PlayUfoTakesSound();
        Invoke("FinishTeleportation", ufoAnimation.clip.length);
        isUfoSoundPlaying = false;
    }


    /// <summary>
    ///     This function is called by an animation event of the Ufo Animation. It loads the new world and teleports the player
    ///     there.
    /// </summary>
    public async void FinishTeleportation()
    {
        player.position = finalTargetPosition;

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
        isUfoSoundPlaying = false;
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
    /// This function plays the ufo sound when it picks up the player.
    /// </summary>
    private void PlayUfoTakesSound()
    {
        if (!isUfoSoundPlaying && ufoTakesSound != null && audioSourceUfoTakes != null)
        {
            audioSourceUfoTakes.PlayOneShot(ufoTakesSound);
            isUfoSoundPlaying = true;
        }
    }

    /// <summary>
    /// This function plays the ufo sound when it returns the player.
    /// </summary>
    private void PlayUfoReturnsSound()
    {
        if (!isUfoSoundPlaying && ufoReturnsSound != null && audioSourceUfoReturns != null)
        {
            audioSourceUfoReturns.PlayOneShot(ufoReturnsSound);
            isUfoSoundPlaying = true;
        }
    }
}