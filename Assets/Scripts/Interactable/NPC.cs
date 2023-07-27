using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        RegisterToGameManager();
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
        if (Input.GetKeyDown(interact) && playerIsClose && !SceneManager.GetSceneByBuildIndex(12).isLoaded &&
            !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
        {
            if (!hasBeenTalkedTo)
            {
                Complete();
            }

            StartCoroutine(LoadDialogueScene());
        }
        else if (Input.GetKeyDown(interact) && playerIsClose && SceneManager.GetSceneByBuildIndex(12).isLoaded &&
                 typingIsFinished && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
        {
            Debug.Log(dialogue.Length - 1);
            Debug.Log("index before next" + index);
            NextLine();
            Debug.Log("index after next" + index);
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

    /// <summary>
    ///     This method removes the NPC from the GameManager.
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("remove NPC " + world + "-" + dungeon + "-" + number);
        ObjectManager.Instance.RemoveGameEntity<NPC, NPCData>(world, dungeon, number);
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
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
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.TALK_TO_NPCS, 1);
    }

    /// <summary>
    ///     This function registers the NPC to the GameManager.
    /// </summary>
    private void RegisterToGameManager()
    {
        Debug.Log("register NPC " + world + "-" + dungeon + "-" + number);
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

        Debug.Log("setup npc " + world + "-" + number + " with new dialogue: " + text + ", new info: " +
                  !hasBeenTalkedTo);
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
        Debug.Log(typingIsFinished);
        foreach (char letter in dialogue[index])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        typingIsFinished = true;
        Debug.Log(typingIsFinished);
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

    #region Getter

    /// <summary>
    ///     This method returns the world index of the NPC.
    /// </summary>
    /// <returns>world</returns>
    public int GetWorldIndex()
    {
        return world;
    }

    public void SetWorldIndex(int worldIndex)
    {
        world = worldIndex;
    }

    /// <summary>
    ///     This method returns the dungeon index of the NPC.
    /// </summary>
    /// <returns>dungeon</returns>
    public int GetDungeonIndex()
    {
        return dungeon;
    }

    public void SetDungeonIndex(int dungeonIndex)
    {
        dungeon = dungeonIndex;
    }

    /// <summary>
    ///     This method returns the number of the NPC.
    /// </summary>
    /// <returns>number</returns>
    public int getIndex()
    {
        return number;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetName(string name)
    {
        nameOfNPC = name;
    }

    public void SetSprite(Sprite sprite)
    {
        imageOfNPC = sprite;
    }

    #endregion
}