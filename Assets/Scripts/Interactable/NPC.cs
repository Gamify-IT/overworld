using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
///     This class is responsible for the NPC logic.
/// </summary>
public class NPC : MonoBehaviour, IGameEntity<NPCData>
{
    [SerializeField] private int world;
    [SerializeField] private int dungeon;
    [SerializeField] private int number;
    public Sprite imageOfNPC;
    public string nameOfNPC;
    public string[] dialogue;
    public float wordSpeed;
    public GameObject speechIndicator;
    private TextMeshProUGUI dialogueText;
    private bool hasBeenTalkedTo;
    private int index;
    private bool playerIsClose;
    private bool typingIsFinished;
    private string uuid;

    private static List<(int, int, int)> alreadyTalkedNPC;
    //KeyCodes
    private KeyCode interact;

    /// <summary>
    ///     This function is called when the object becomes enabled and active.
    ///     It is used to initialize the NPCs.
    /// </summary>
    private void Awake()
    {
        GameObject child = transform.GetChild(0).gameObject;
        if (child != null)
        {
            speechIndicator = child;
        }

        if (GameSettings.GetGamemode() != Gamemode.TUTORIAL)
        {
            alreadyTalkedNPC = DataManager.Instance.GetAchievements().Find(achievement => achievement.GetTitle() == "COMMUNICATOR_LEVEL_3").GetInteractedObjects();
        }

        InitNewStuffSprite();
        interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
    }

    /// <summary>
    ///     This method opens the NPC dialogue if the player is close to the NPC and presses "E".
    ///     If the player wants to skip the typing out of the dialogue they can press "E" again.
    /// </summary>
    private void Update()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY || GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            if (Input.GetKeyDown(interact) && playerIsClose && !SceneManager.GetSceneByBuildIndex(12).isLoaded &&
            !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                if (!hasBeenTalkedTo && GameSettings.GetGamemode() == Gamemode.PLAY)
                {
                    Complete();
                }

                StartCoroutine(LoadDialogueScene());
            }
            else if (Input.GetKeyDown(interact) && playerIsClose && SceneManager.GetSceneByBuildIndex(12).isLoaded &&
                     typingIsFinished && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                NextLine();
            }
            else if (Input.GetKeyDown(interact) && playerIsClose && SceneManager.GetSceneByBuildIndex(12).isLoaded &&
                     !typingIsFinished && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                StopCoroutine("Typing");
                dialogueText.text = "";
                dialogueText.text = dialogue[index];
                typingIsFinished = true;
            }
        }            
    }

    /// <summary>
    ///     This method removes the NPC from the GameManager.
    /// </summary>
    private void OnDestroy()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY || GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            ObjectManager.Instance.RemoveGameEntity<NPC, NPCData>(world, dungeon, number);
            GameEvents.current.onKeybindingChange -= UpdateKeybindings;
        }
    }

    /// <summary>
    ///     This function initializes the <c>NPC</c> object
    /// </summary>
    /// <param name="areaIdentifier">The area the <c>NPC</c> is in</param>
    /// <param name="index">The index of the <c>NPC</c> in its area</param>
    public void Initialize(AreaInformation areaIdentifier, int index, string name, Sprite sprite)
    {
        world = areaIdentifier.GetWorldIndex();
        dungeon = 0;
        if (areaIdentifier.IsDungeon())
        {
            dungeon = areaIdentifier.GetDungeonIndex();
        }
        number = index;
        nameOfNPC = name;
        imageOfNPC = sprite;

        RegisterToGameManager();
    }

    /// <summary>
    ///     If the player is in range of the NPC the playerIsClose check will be set to true.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    /// <summary>
    ///     If the player leaves the range of the NPC while the dialogue window is open, the window will be closed, text will
    ///     be reset and playerIsClose check will be set to false.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && SceneManager.GetSceneByBuildIndex(12).isLoaded)
        {
            playerIsClose = false;
            ResetText();
        }
        else if (other.CompareTag("Player") && !SceneManager.GetSceneByBuildIndex(12).isLoaded)
        {
            playerIsClose = false;
        }   
    }

    /// <summary>
    ///     This method sets the NPC as talked to.
    /// </summary>
    private void Complete()
    {
        hasBeenTalkedTo = true;
        speechIndicator.SetActive(false);
        GameManager.Instance.CompleteNPC(world, dungeon, number, uuid);
    }

    /// <summary>
    ///     This function registers the NPC to the GameManager.
    /// </summary>
    private void RegisterToGameManager()
    {
        ObjectManager.Instance.AddGameEntity<NPC, NPCData>(gameObject, world, dungeon, number);
    }

    /// <summary>
    ///     setup called by game manager
    /// </summary>
    /// <param name="data"></param>
    public void Setup(NPCData data)
    {
        uuid = data.GetUuid();
        dialogue = data.GetDialogue();
        hasBeenTalkedTo = data.GetHasBeenTalkedTo();
        InitNewStuffSprite();
        string text = "";
        for (int index = 0; index < dialogue.Length; index++)
        {
            text += dialogue[index];
            text += " ; ";
        }
    }

    /// <summary>
    ///     This method initializes the new stuff sprite if the NPC has not been talked to yet.
    /// </summary>
    private void InitNewStuffSprite()
    {
        if (hasBeenTalkedTo)
        {
            speechIndicator.SetActive(false);
        }
        else
        {
            speechIndicator.SetActive(true);
        }
    }

    /// <summary>
    ///     This method resets the text of the NPC and closes the dialogue overlay
    /// </summary>
    public void ResetText()
    {
        dialogueText.text = "";
        index = 0;
        SceneManager.UnloadSceneAsync("Dialogue Overlay");
    }

    /// <summary>
    ///     This method types out the text of the NPC dialogue with a given wordSpeed.
    /// </summary>
    private IEnumerator Typing()
    {
        typingIsFinished = false;
        foreach (char letter in dialogue[index])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        typingIsFinished = true;
    }

    /// <summary>
    ///     This method will show the next line of the NPC dialogue and deletes the old line.
    ///     If the current line is the last line of the Dialogue ResetText() will be executed.
    /// </summary>
    public void NextLine()
    {
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine("Typing");
        }
        else
        {
            ResetText();
        }
    }

    /// <summary>
    ///     This method loads the dialogue window and will change the text and name of the NPC to the text and name set in the
    ///     NPC.
    /// </summary>
    private IEnumerator LoadDialogueScene()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync("Dialogue Overlay", LoadSceneMode.Additive);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }

        GameObject.Find("ImageOfNPC").GetComponent<Image>().sprite = imageOfNPC;
        GameObject.Find("NPC_Name").GetComponent<TextMeshProUGUI>().text = nameOfNPC;
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        StartCoroutine("Typing");

        if (GameSettings.GetGamemode() != Gamemode.TUTORIAL)
        {
            UpdateListOfNPC();
        }
    }

    /// <summary>
    ///     This method adds a new NPC that the player has already talked to to the list and 
    ///     calls the method for the achievement update by interacting with the new NPC.  
    /// </summary>
    private void UpdateListOfNPC()
    {
        var key = (world, dungeon, number);
        if(!alreadyTalkedNPC.Contains(key))
        {
            alreadyTalkedNPC.Add(key);
            UpdateAchievements(world);
        } 
    }

    /// <summary>
    ///     This method updates the "talk to NPC" achievements in general and for a specific world.
    /// </summary>
    /// <param name="worldNumber">The number of the world where the NPC the player spoke to is located</param>
    private void UpdateAchievements(int worldNumber)
    {
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.COMMUNICATOR_LEVEL_1, 1, alreadyTalkedNPC);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.COMMUNICATOR_LEVEL_2, 1, alreadyTalkedNPC);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.COMMUNICATOR_LEVEL_3, 1, alreadyTalkedNPC);

        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"COMMUNICATOR_LEVEL_1_WORLD_{worldNumber}"), 1, alreadyTalkedNPC);
        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"COMMUNICATOR_LEVEL_2_WORLD_{worldNumber}"), 1, alreadyTalkedNPC);
        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"COMMUNICATOR_LEVEL_3_WORLD_{worldNumber}"), 1, alreadyTalkedNPC);
    }

    /// <summary>
    ///     This function returns the NPC object info
    /// </summary>
    /// <returns>NPC object info</returns>
    public string GetInfo()
    {
        string info = "";
        string text = "";
        for (int index = 0; index < dialogue.Length; index++)
        {
            text += dialogue[index];
            text += "; ";
        }

        info = world + "-" + dungeon + "-" + number + ": Text: " + text + ", completed: " + hasBeenTalkedTo;
        return info;
    }

    /// <summary>
    ///     This function updates the keybindings
    /// </summary>
    /// <param name="binding">The binding that changed</param>
    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.INTERACT)
        {
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        }
    }
}