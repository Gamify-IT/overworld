using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public Button openButton; // Button, um das Leaderboard zu öffnen
    private Button closeButton; // Button, um das Leaderboard zu schließen

    private bool isLeaderboardOpen = false;

    private void Start()
    {
        // Sicherstellen, dass der Open-Button zugewiesen ist
        if (openButton != null)
        {
            // OnClick Listener für den "Open Button" hinzufügen
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
        // Überprüfen, ob das Leaderboard bereits geöffnet ist
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
        // Überprüfen, ob das Leaderboard geöffnet ist
        if (isLeaderboardOpen)
        {
            // Szene entladen, um das Leaderboard zu schließen
            SceneManager.UnloadSceneAsync("Rewards");
            isLeaderboardOpen = false;
            Time.timeScale = 1f; // Zeit fortsetzen
        }
    }

    private void Update()
    {
        // Überprüfen, ob die "Escape"-Taste gedrückt wird und das Leaderboard geöffnet ist
        if (Input.GetKeyDown(KeyCode.Escape) && isLeaderboardOpen)
        {
            CloseLeaderboardScene();
        }
    }
}
