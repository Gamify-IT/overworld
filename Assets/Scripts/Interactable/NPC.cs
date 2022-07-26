using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    [SerializeField] private int world;
    [SerializeField] private int number;
    public Sprite imageOfNPC;
    public string nameOfNPC;
    private TextMeshProUGUI dialogueText;
    public string[] dialogue;
    private int index;
    public float wordSpeed;
    private bool playerIsClose;
    private bool typingIsFinished;
    private bool hasBeenTalkedTo;
    public GameObject speechIndicator;

    private void Awake()
    {
        GameObject child = transform.GetChild(0).gameObject;
        if(child != null)
        {
            speechIndicator = child;
        }
    }

    void Start()
    {
        registerToGameManager();
    }

    /// <summary>
    /// This method opens the NPC dialogue if the player is close to the NPC and presses "E".
    /// If the player wants to skip the typing out of the dialogue they can press "E" again.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose && !SceneManager.GetSceneByBuildIndex(12).isLoaded)
        {
            markAsRead();
            StartCoroutine(LoadDialogueScene());
        } else if (Input.GetKeyDown(KeyCode.E) && playerIsClose && SceneManager.GetSceneByBuildIndex(12).isLoaded && typingIsFinished)
        {
            Debug.Log(dialogue.Length - 1);
            Debug.Log("index before next" +  index);
            NextLine();
            Debug.Log("index after next" + index);
        }
        else if (Input.GetKeyDown(KeyCode.E) && playerIsClose && SceneManager.GetSceneByBuildIndex(12).isLoaded && !typingIsFinished)
        {
            StopCoroutine("Typing");
            dialogueText.text = "";
            dialogueText.text = dialogue[index];
            typingIsFinished = true;
        }
    }

    //mark
    private void markAsRead()
    {
        hasBeenTalkedTo = true;
        speechIndicator.SetActive(false);
    }

    // register to game manager
    private void registerToGameManager()
    {
        Debug.Log("register NPC " + world + " - " + number);
        GameManager.instance.addNPC(this.gameObject, world, number);
    }
    
    //setup called by game manager
    public void setup(NPCData data)
    {
        dialogue = data.getDialogue();
        Debug.Log("setup npc " + world + "-" + number + "with new dialogue: " + dialogue.ToString());
    }

    /// <summary>
    /// This method resets the text of the NPC and closes the dialogue overlay
    /// </summary>
    public void ResetText()
    {
        dialogueText.text = "";
        index = 0;
        SceneManager.UnloadSceneAsync("Dialogue Overlay");
    }

    /// <summary>
    /// This method types out the text of the NPC dialogue with a given wordSpeed.
    /// </summary>
    IEnumerator Typing()
    {
        typingIsFinished = false;
        Debug.Log(typingIsFinished);
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        typingIsFinished = true;
        Debug.Log(typingIsFinished);
    }

    /// <summary>
    /// This method will show the next line of the NPC dialogue and deletes the old line.
    /// If the current line is the last line of the Dialogue ResetText() will be executed.
    /// </summary>
    public void NextLine()
    {
        if(index < dialogue.Length - 1)
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
    /// If the player is in range of the NPC the playerIsClose check will be set to true.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    /// <summary>
    /// If the player leaves the range of the NPC while the dialogue window is open, the window will be closed, text will be reset and playerIsClose check will be set to false.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && SceneManager.GetSceneByBuildIndex(12).isLoaded)
        {
            playerIsClose = false;
            ResetText();
        }
    }

    /// <summary>
    /// This method loads the dialogue window and will change the text and name of the NPC to the text and name set in the NPC.
    /// </summary>
    IEnumerator LoadDialogueScene()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync("Dialogue Overlay", LoadSceneMode.Additive);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }
        GameObject.Find("ImageOfNPC").GetComponent<Image>().sprite = imageOfNPC;
        GameObject.Find("NPC_Name").GetComponent<TextMeshProUGUI>().text = nameOfNPC;
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        StartCoroutine(Typing());
    }
}
