using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // singleton
    public static TutorialManager Instance { get; private set; }

    // data objects to be displayed
    private ContentScreenData[] tutorialContentData;

    // global state variables
    private static int progressCounter = 0;
    private static bool showScreen = true;

    [Header("Content Screen")] 
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text content;
    [SerializeField] private TMP_Text buttonLabel;

    [Header("Intercatable Elements")]
    [SerializeField] private GameObject[] interactables;
    [SerializeField] private GameObject trigger;
    [SerializeField] private GameObject dungeonBarrier;
    [SerializeField] private GameObject overworldBarrier;

    [Header("Intercatable Elements")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource audioSource;

    // text shown in the interactable objects
    private readonly string bookText = "Congratulations, you found the book! \nIf you walk away, you can continue your journey...";
    private readonly string npcText = "Welcome to the game apprentice! \nCome to me if you want to hear some secrets...";
    private readonly string signText = "This is a sign! \nHint: If you ever get lost, look at the map.";

    #region singleton
    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        SetupData();

        audioSource.clip = clickSound;

        foreach (GameObject interactable in interactables)
        {
            interactable.SetActive(false);
        }

        ProgressBar.Instance.SetupTutorial();

        dungeonBarrier.SetActive(true);
        overworldBarrier.SetActive(true);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void Update()
    {
        ProgressBar.Instance.setProgress((float) progressCounter /tutorialContentData.Length);   
        
        if(showScreen)
        {
            showScreen = false;
            ActivateInfoScreen(true);
        }
    }

    /// <summary>
    ///     Loads the data to be displayed on the content screens
    /// </summary>
    private void SetupData()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("Tutorial/content");
        string json= targetFile.text;

        tutorialContentData = JsonHelper.GetJsonArray<ContentScreenData>(json);
    }

    /// <summary>
    ///     (De)ctivates the info screen bases on the given input
    /// </summary>
    /// <param name="status">State whether the info screen is active or not</param>
    public async UniTask ActivateInfoScreen(bool status)
    {
        if (status)
        {
            await SceneManager.LoadSceneAsync("Content Screen", LoadSceneMode.Additive);            
            Time.timeScale = 0f;
            GameManager.Instance.SetIsPaused(true);
            UpdateScreen();
        }
        else
        {
            PlayClickSound();
            await SceneManager.UnloadSceneAsync("Content Screen");
            Time.timeScale = 1f;           
            progressCounter++;
            GameManager.Instance.SetIsPaused(false);

            if (progressCounter <= 2)
            {
                StartCoroutine(LoadNextScreen(0));
            }
        }
    }

    /// <summary>
    ///     Updates the content screen with the next part of the tutorial
    /// </summary>
    public void UpdateScreen()
    {
        ContentScreenData contentData = tutorialContentData[progressCounter];

        ContentScreenManager.Instance.Setup(contentData);

        if (contentData.GetButtonLabel() != "CONTINUE" && contentData.GetButtonLabel() != "START" && contentData.GetButtonLabel() != "GOT IT")
        {
            ProgressBar.Instance.DisplayTaskOnScreen(contentData.GetButtonLabel() + "!");

            if (progressCounter - 3 < interactables.Length)
            {
                GameObject currentInteractable = interactables[progressCounter - 3];
                currentInteractable.SetActive(true);
                ShowInteractableText(currentInteractable, progressCounter - 3);
            }
        }
    }

    /// <summary>
    ///     Uniquely shows the instruction text for each interactable object
    /// </summary>
    /// <param name="currentInteractable">current interactable object</param>
    /// <param name="index">position of the current interactable in the tutorial progress</param>
    private void ShowInteractableText(GameObject currentInteractable, int index)
    {
        switch (index)
        {
            // show sign text
            case 0:
                currentInteractable.GetComponent<Sign>().text = signText;
                trigger.SetActive(false);
                break;
            // show book text
            case 1:
                currentInteractable.GetComponent<Book>().SetText(bookText);
                break;
            // show npc text
            case 2:
                currentInteractable.GetComponent<NPC>().SetText(npcText);
                break;
        }
    }

    /// <summary>
    ///     Signal that the next info screen should be shown
    /// </summary>
    public void ShowScreen()
    {
        showScreen = true;
    }

    /// <summary>
    ///     Loads the next screen in the tutorial after the player has intercated with the current object
    /// </summary>
    /// <param name="delay">seconds after which the next screen should load</param>
    public IEnumerator LoadNextScreen(int delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ActivateInfoScreen(true);
    }

    /// <summary>
    ///    Loads the next info screen after the player returns from another scene
    /// </summary>
    public void SetupAfterScene(int delay)
    {
        StartCoroutine(LoadNextScreen(delay));
    }

    /// <summary>
    ///     Activates the dungeon after showing leaderboard and shop to the player
    /// </summary>
    public void ActivateDungeon()
    {
        dungeonBarrier.SetActive(false);
        StartCoroutine(LoadNextScreen(1));
    }

    /// <summary>
    ///     Activates the overworld after completing the dungeon minigame and returning back to the tutorial world
    /// </summary>
    public void ActivateOverworld()
    {
        // activate overworld connection after second minigame is completed
        if (progressCounter == 13)
        {
            overworldBarrier.SetActive(false);
        }
    }

    /// <summary>
    ///     Gets the current progress as counter
    /// </summary>
    /// <returns>current progress</returns>
    public int GetProgress()
    {
        return progressCounter;
    }

}
