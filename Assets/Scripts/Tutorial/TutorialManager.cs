using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private GameObject infoScreen;
    private ContentScreenData[] data;
    private int progressCounter = 0;
    private bool showScreen = true;
    private string json;

    [Header("Content Screen")] 
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text content;
    [SerializeField] private TMP_Text buttonLabel;
    private string taskDescription;

    [Header("Intercatable Elements")]
    [SerializeField] private GameObject[] interactables;
    [SerializeField] private GameObject trigger;

    // tutorial info texts in interactables
    private readonly string bookText = "Congratulations, you found the book! \nIf you walk away, you can continue your journey...";
    private readonly string npcText = "Welcome to the game apprentice! \nCome to me if you want to hear some secrets...";
    private readonly string signText = "This is a sign! \nIf you ever get lost, look at the map.";

    #region singelton
    // <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


    private void Start()
    {
        Time.timeScale = 0f;
        SetupData();
        
        foreach (GameObject interactable in interactables)
        {
            interactable.SetActive(false);
        }

        ProgressBar.Instance.SetupTutorial();
    }

    private void Update()
    {
        ProgressBar.Instance.setProgress((float) progressCounter /data.Length);

        if(showScreen)
        {
            ActivateInfoScreen(true);
        }
    }

    /// <summary>
    ///     Loads the data to be displayed on the content screens
    /// </summary>
    private void SetupData()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("Tutorial/content");
        json = targetFile.text;

        data = JsonHelper.GetJsonArray<ContentScreenData>(json);
    }

    /// <summary>
    ///     (De)ctivates the info screen bases on the given input
    /// </summary>
    /// <param name="status">State whether the info screen is active or not</param>
    public void ActivateInfoScreen(bool status)
    {
        infoScreen.SetActive(status);

        if (status)
        {
            Time.timeScale = 0f;
            showScreen = false;
            GameManager.Instance.SetIsPaused(true);
            UpdateScreen();
        }
        else
        {
            Time.timeScale = 1f;
            progressCounter++;
            GameManager.Instance.SetIsPaused(false);

            if (progressCounter == 1)
            {
                ActivateInfoScreen(true);
            }
        }
    }

    /// <summary>
    ///     Updates the content screen with the next part of the tutorial
    /// </summary>
    public void UpdateScreen()
    {
        ContentScreenData screen = data[progressCounter];

        header.text = screen.GetHeader();
        content.text = screen.GetContent();
        buttonLabel.text = screen.GetButtonLabel();

        if (screen.GetButtonLabel() != "CONTINUE" && screen.GetButtonLabel() != "START")
        {
            ProgressBar.Instance.DisplayTaskOnScreen(screen.GetButtonLabel() + "!");

            GameObject currentInteractable = interactables[progressCounter - 2];
            currentInteractable.SetActive(true);

            ShowInteractableText(currentInteractable, progressCounter - 2);
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
            case 0:
                currentInteractable.GetComponent<Sign>().text = signText;
                trigger.SetActive(false);
                break;
            case 1:
                currentInteractable.GetComponent<Book>().SetText(bookText);
                break;
            case 2:
                currentInteractable.GetComponent<NPC>().SetText(npcText);
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    /// <summary>
    ///     Signal taht the (next) info screen should be shown.
    /// </summary>
    public void ShowScreen()
    {
        showScreen = true;
    }

    /// <summary>
    ///     Loads the next screen in the tutorial after the player has intercated with the current object.
    /// </summary>
    /// <param name="delay">seconds after which the next screen should load</param>
    public IEnumerator LoadNextScreen(int delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateInfoScreen(true);
    }

}
