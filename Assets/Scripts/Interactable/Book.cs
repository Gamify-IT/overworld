using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

/// <summary>
///     This class is responsible for the Book logic.
/// </summary>
public class Book : MonoBehaviour, IGameEntity<BookData>
{
    [SerializeField] private int world;
    [SerializeField] private int dungeon;
    [SerializeField] private int number;
    public string nameOfBook;
    public string bookContent;
    private TextMeshProUGUI bookText;
    private bool playerIsClose;
    private string uuid;

    private List<(int, int, int)> readBooks;

    //KeyCodes
    private KeyCode interact;

    /// <summary>
    ///     This function is called when the object becomes enabled and active.
    ///     It is used to initialize the Books.
    /// </summary>
    private void Awake()
    {
        interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;

        if (GameSettings.GetGamemode() != Gamemode.TUTORIAL)
        {
            readBooks = DataManager.Instance.GetAchievements().Find(achievement => achievement.GetTitle() == "READER_LEVEL_3").GetInteractedObjects();
        }
    }

    /// <summary>
    ///     This method opens the Book text if the player is close to the Book and presses "E".
    /// </summary>
    private void Update()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY || GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            if (Input.GetKeyDown(interact) && playerIsClose && !SceneManager.GetSceneByName("Book").isLoaded &&
            !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                StartCoroutine(LoadBookScene());
            }
            else if (Input.GetKeyDown(interact) && playerIsClose && SceneManager.GetSceneByName("Book").isLoaded &&
                     !PauseMenu.menuOpen && !PauseMenu.subMenuOpen)
            {
                SceneManager.UnloadSceneAsync("Book");
            }
        }            
    }

    /// <summary>
    ///     This method removes the Book from the GameManager.
    /// </summary>
    private void OnDestroy()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            ObjectManager.Instance.RemoveGameEntity<Book, BookData>(world, dungeon, number);
            GameEvents.current.onKeybindingChange -= UpdateKeybindings;
        }            
    }

    /// <summary>
    ///     This function initializes the <c>Book</c> object
    /// </summary>
    /// <param name="areaIdentifier">The area the <c>Book</c> is in</param>
    /// <param name="index">The index of the <c>Book</c> in its area</param>
    public void Initialize(AreaInformation areaIdentifier, int index, string name)
    {
        world = areaIdentifier.GetWorldIndex();
        dungeon = 0;
        if (areaIdentifier.IsDungeon())
        {
            dungeon = areaIdentifier.GetDungeonIndex();
        }
        number = index;
        nameOfBook = name;

        RegisterToGameManager();
    }

    /// <summary>
    ///     If the player is in range of the Book the playerIsClose check will be set to true.
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
            SceneManager.UnloadSceneAsync("Book");
        }
        else if (other.CompareTag("Player") && !SceneManager.GetSceneByName("Book").isLoaded)
        {
            playerIsClose = false;
        }
    }

    /// <summary>
    ///     This function registers the Book to the GameManager.
    /// </summary>
    private void RegisterToGameManager()
    {
        ObjectManager.Instance.AddGameEntity<Book, BookData>(gameObject, world, dungeon, number);
    }

    /// <summary>
    ///     setup called by game manager
    /// </summary>
    /// <param name="data"></param>
    public void Setup(BookData data)
    {
        uuid = data.GetUuid();
        bookContent = data.GetBookText();
        string text = bookContent;
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
        bookText.text = bookContent;
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            UpdateListOfBooks();
        }
    }

    /// <summary>
    ///     This method adds a new read book to the list and calls the method for the achievement update by interacting with the new book. 
    /// </summary>
    private void UpdateListOfBooks()
    {
        var key = (world, dungeon, number);
        if(!readBooks.Contains(key))
        {
            readBooks.Add(key);
            UpdateAchievements(world);
        } 
    }

    /// <summary>
    ///     This method updates the "read books" achievements in general and for a specific world.
    /// </summary>
    /// <param name="worldNumber">The number of the world in which is the interacted book</param>
    private void UpdateAchievements(int worldNumber)
    {
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.READER_LEVEL_1, 1, readBooks);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.READER_LEVEL_2, 1, readBooks);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.READER_LEVEL_3, 1, readBooks);

        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"READER_LEVEL_1_WORLD_{worldNumber}"), 1, readBooks);
        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"READER_LEVEL_2_WORLD_{worldNumber}"), 1, readBooks);
        GameManager.Instance.IncreaseAchievementProgress((AchievementTitle)Enum.Parse(typeof(AchievementTitle), $"READER_LEVEL_3_WORLD_{worldNumber}"), 1, readBooks);
    }

    /// <summary>
    ///     This function returns the Book object info
    /// </summary>
    /// <returns>Book object info</returns>
    public string GetInfo()
    {
        string info = "";
        string text = bookContent;

        info = world + "-" + dungeon + "-" + number + ": Text: " + text + ", completed: ";
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