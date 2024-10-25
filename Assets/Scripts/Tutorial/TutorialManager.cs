using Cysharp.Threading.Tasks;
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
        Setup();
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
    /// 
    /// </summary>
    private void Setup()
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
            UpdateScreen();
        }
        else
        {
            Time.timeScale = 1f;
            progressCounter++;
            Debug.Log(progressCounter + ", " + data.Length);
            Debug.Log((float) progressCounter / data.Length);
        }
    }

    public void UpdateScreen()
    {
        showScreen = false;

        ContentScreenData screen = data[progressCounter];

        header.text = screen.GetHeader();
        content.text = screen.GetContent();
        buttonLabel.text = screen.GetButtonLabel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            showScreen = true;
        }
    }
}
