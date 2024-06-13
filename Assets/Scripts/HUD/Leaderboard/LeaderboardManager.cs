using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject leaderboardPrefab;
    [SerializeField] private TMP_Dropdown leagueDropdown;
    [SerializeField] private TMP_Dropdown worldDropdown;
    [SerializeField] private TMP_Dropdown minigameDropdown;
    private string league;
    private string world;
    private string minigame;
    private List<PlayerstatisticDTO> ranking;
    private bool filterActive;

    public Button openButton; 
    private Button closeButton; 

    private bool isLeaderboardOpen = false;

    private void Start()
    {

        /*achievements = DataManager.Instance.GetAchievements();
        Setup();
        UpdateUI();*/

        if (openButton != null)
        {
            
            openButton.onClick.AddListener(OpenLeaderboardScene);
        }
        else
        {
            Debug.LogError("Open Button is not assigned in the Inspector.");
        }
    }

    public void SetLeague()
    {
        int option = leagueDropdown.value;
        league = leagueDropdown.options[option].text;
        UpdateUI();
    }

    public void SetWorld()
    {
        int option = worldDropdown.value;
        world = worldDropdown.options[option].text;
        UpdateUI();
    }

    public void SetMinigame()
    {
        int option = minigameDropdown.value;
        minigame = minigameDropdown.options[option].text;
        UpdateUI();
    }

    public void ResetFilter()
    {
        filterActive = false;
        
        UpdateUI();
    }

   /* private void UpdateUI()
    {
        ResetUI();
        List<PlayerTaskDTO> highscoresToDisplay = FilterHighscores();
        DisplayHighscores(highscoresToDisplay);
    }
    private void ResetUI()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
            
        }
    }

      private List<PlayerstatisticDTO> FilterHighscores()
    {
        List<PlayerstatisticDTO> highscoresToDisplay = new List<PlayerstatisticDTO>();
        foreach (AchievementData achievement in achievements)
        {
            if (CheckCategory(achievement) && CheckStatus(achievement) && CheckFilter(achievement))
            {
                achievementsToDisplay.Add(achievement);
            }
        }

        return achievementsToDisplay;
    }*/
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
       
        if (!isLeaderboardOpen)
        {
            
            SceneManager.LoadScene("Rewards", LoadSceneMode.Additive);
            isLeaderboardOpen = true;
            Time.timeScale = 0f; // Zeit anhalten
        }
    }

    public void CloseLeaderboardScene()
    {
        if (isLeaderboardOpen)
        {
            SceneManager.UnloadSceneAsync("Rewards");
            isLeaderboardOpen = false;
            Time.timeScale = 1f; // Zeit fortsetzen
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLeaderboardOpen)
        {
            CloseLeaderboardScene();
            
        }
    }
}
