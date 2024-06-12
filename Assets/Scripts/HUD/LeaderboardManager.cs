using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public Button openButton; // Button, um das Leaderboard zu �ffnen
    private Button closeButton; // Button, um das Leaderboard zu schlie�en

    private bool isLeaderboardOpen = false;

    private void Start()
    {
        // Sicherstellen, dass der Open-Button zugewiesen ist
        if (openButton != null)
        {
            // OnClick Listener f�r den "Open Button" hinzuf�gen
            openButton.onClick.AddListener(OpenLeaderboardScene);
        }
        else
        {
            Debug.LogError("Open Button is not assigned in the Inspector.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Rewards")
        {
            // Finde den Close-Button in der RewardsScene
            closeButton = GameObject.Find("CloseButton").GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseLeaderboardScene);
            }
            else
            {
                Debug.LogError("Close Button is not found in the RewardsScene.");
            }
        }
    }

    public void OpenLeaderboardScene()
    {
        // �berpr�fen, ob das Leaderboard bereits ge�ffnet ist
        if (!isLeaderboardOpen)
        {
            // Szene additiv laden, damit der Hintergrund sichtbar bleibt
            SceneManager.LoadScene("Rewards", LoadSceneMode.Additive);
            isLeaderboardOpen = true;
            Time.timeScale = 0f; // Zeit anhalten
        }
    }

    public void CloseLeaderboardScene()
    {
        // �berpr�fen, ob das Leaderboard ge�ffnet ist
        if (isLeaderboardOpen)
        {
            // Szene entladen, um das Leaderboard zu schlie�en
            SceneManager.UnloadSceneAsync("Rewards");
            isLeaderboardOpen = false;
            Time.timeScale = 1f; // Zeit fortsetzen
        }
    }

    private void Update()
    {
        // �berpr�fen, ob die "Escape"-Taste gedr�ckt wird und das Leaderboard ge�ffnet ist
        if (Input.GetKeyDown(KeyCode.Escape) && isLeaderboardOpen)
        {
            CloseLeaderboardScene();
        }
    }
}
