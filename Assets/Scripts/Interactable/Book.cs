using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///     This class is responsible for the Book logic.
/// </summary>
public class Book : MonoBehaviour
{
    [SerializeField] private int world;
    [SerializeField] private int dungeon;
    [SerializeField] private int number;
    public string nameOfBook;
    public string[] bookContent;
    private TextMeshProUGUI bookText;
    private int index;
    private bool playerIsClose;
    private string uuid;

    /// <summary>
    ///     This function is called when the object becomes enabled and active.
    ///     It is used to initialize the Books.
    /// </summary>
    private void Awake()
    {
        RegisterToGameManager();
    }

    /// <summary>
    ///     This method opens the Book text if the player is close to the Book and presses "E".
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose && !SceneManager.GetSceneByName("Book").isLoaded &&
            !PauseMenu.menuOpen)
        {
            StartCoroutine(LoadBookScene());
        }
    }

    /// <summary>
    ///     This method removes the Book from the GameManager.
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("remove Book " + world + "-" + dungeon + "-" + number);
        GameManager.Instance.RemoveNpc(world, dungeon, number); /////////////////////////////////////////
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
    ///     If the player leaves the range of the Book while the Book window is open, the window will be closed, text will
    ///     be reset and playerIsClose check will be set to false.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && SceneManager.GetSceneByName("Book").isLoaded)
        {
            playerIsClose = false;
        }
    }


    /// <summary>
    ///     This function registers the NPC to the GameManager.
    /// </summary>
    private void RegisterToGameManager()
    {
        Debug.Log("register Book " + world + "-" + dungeon + "-" + number);
        GameManager.Instance.AddNpc(gameObject, world, dungeon, number); //////////////////////////
    }

    /// <summary>
    ///     setup called by game manager
    /// </summary>
    /// <param name="data"></param>
    public void Setup(BookData data)
    {
        uuid = data.GetUuid();
        bookContent = data.GetBookText();
        string text = "";
        for (int index = 0; index < bookContent.Length; index++)
        {
            text += bookContent[index];
            text += " ; ";
        }

        Debug.Log("setup book " + world + "-" + number + " with new dialogue: " + text + ", new info: ");
    }


    /// <summary>
    ///     This method loads the dialogue window and will change the text and name of the Book to the text and name set in the
    ///     Book.
    /// </summary>
    private IEnumerator LoadBookScene()
    {
        var asyncLoadScene = SceneManager.LoadSceneAsync("Book", LoadSceneMode.Additive);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }

        GameObject.Find("Book_Name").GetComponent<TextMeshProUGUI>().text = nameOfBook;
        bookText = GameObject.Find("Book_Text").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    ///     This function returns the Book object info
    /// </summary>
    /// <returns>NPC object info</returns>
    public string GetInfo()
    {
        string info = "";
        string text = "";
        for (int index = 0; index < bookContent.Length; index++)
        {
            text += bookContent[index];
            text += "; ";
        }

        info = world + "-" + dungeon + "-" + number + ": Text: " + text + ", completed: ";
        return info;
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

    /// <summary>
    ///     This method returns the dungeon index of the NPC.
    /// </summary>
    /// <returns>dungeon</returns>
    public int GetDungeonIndex()
    {
        return dungeon;
    }

    /// <summary>
    ///     This method returns the number of the NPC.
    /// </summary>
    /// <returns>number</returns>
    public int getIndex()
    {
        return number;
    }

    #endregion
}