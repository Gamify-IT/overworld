using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// überall DTO zu DATA ändern (check)
/// anstatt highscores -> rewards (check)
/// </summary>

public class LeaderboardManagerUpdate : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject leaderboardPrefab;
    [SerializeField] private TMP_Dropdown leagueDropdown;
    [SerializeField] private TMP_Dropdown worldDropdown;
    [SerializeField] private TMP_Dropdown minigameDropdown;
    [SerializeField] private GameObject distributionPanel;
    [SerializeField] private WorldData worldNames;
    private string league;
    private string world;
    private string minigame;
    private List<PlayerStatisticData> ranking;
    private LeagueDefiner leagues;
    private bool filterActive;

    public Button openButton;
    private Button closeButton;
    public TMP_Text visibilityButton;
    public Button distributionButton;

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

        if (distributionButton != null) 
        {
            distributionButton.onClick.AddListener(OpenDistributionPanel);
        }
        else
        {
            Debug.LogError("Distribution Button is not assigned in the Inspector.");
        }

        if (visibilityButton != null)
        {

            visibilityButton.transform.parent.GetComponent<Button>().onClick.AddListener(ToggleButtonText);
            LoadVisibilityState();

        }
        else
        {
            Debug.LogError("Visibility Button is not assigned in the Inspector.");
        }

    }

    private void ToggleButtonText()
    {
        if (visibilityButton.text == "hide me")
        {
            visibilityButton.text = "show me";
        }
        else
        {
            visibilityButton.text = "hide me";
        }
        SaveVisibilityState();

    }

    private void SaveVisibilityState()
    {
        PlayerPrefs.SetString("VisibilityState", visibilityButton.text);
        PlayerPrefs.Save();
    }

    private void LoadVisibilityState()
    {
        if (PlayerPrefs.HasKey("VisibilityState"))
        {
            visibilityButton.text = PlayerPrefs.GetString("VisibilityState");
        }
        else
        {
            visibilityButton.text = "hide me"; 
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

    private void UpdateUI()
    {
        ResetUI();
        List<PlayerStatisticData> rewardsToDisplay = FilterRewards();
        DisplayRewards(rewardsToDisplay);
    }
    private void ResetUI()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);

        }
    }

  // Hole Ligen aus neuer LigaKlasse mit der Methode GetLeagues
    private List<string> GetAllLeagues()
    {
        List<string> allLeagues = new()
        {
            "Filter by..."
        };
       
            foreach (string league in leagues.GetLeagues())
            {
                if (!allLeagues.Contains(league))
                {
                    allLeagues.Add(league);
                }
            }
        

        return allLeagues;
    }


    // Hole namen aus neuer NameMethode aus WorldData
    private List<string> GetWorldNames()
    {
        List<string> worlds = new()
        {
            "Filter by..."
        };
       
            foreach (string world in worldNames.GetWorldNames())
            {
                if (!worlds.Contains(world))
                {
                    worlds.Add(world);
                }
            }
        

        return worlds;
    }

    private List<string> GetLeagues()
    {
        List<string> leagues = new()
        {
            "Filter by..."
        };
        foreach (PlayerStatisticData rank in ranking)
        {
            foreach (string league in rank.GetLeagues())
            {
                if (!leagues.Contains(league))
                {
                    leagues.Add(league);
                }
            }
        }

        return leagues;
    }



    private List<string> GetWorlds()
    {
        List<string> worlds = new()
        {
            "Filter by..."
        };
        foreach (PlayerStatisticData rank in ranking)
        {
            foreach (string world in rank.GetWorlds())
            {
                if (!worlds.Contains(world))
                {
                    worlds.Add(world);
                }
            }
        }

        return worlds;
    }


    private List<string> GetMinigames()
    {
        List<string> minigames = new()
        {
            "Filter by..."
        };
        foreach (PlayerStatisticData rank in ranking)
        {
            foreach (string minigame in rank.GetWorlds())
            {
                if (!minigames.Contains(minigame))
                {
                    minigames.Add(minigame);
                }
            }
        }

        return minigames;
    }

    private List<PlayerStatisticData> FilterRewards()
    {
        List<PlayerStatisticData> rewardsToDisplay = new List<PlayerStatisticData>();
        foreach (PlayerStatisticData ranking in ranking)
        {
            if (CheckLeague(ranking) && CheckWorld(ranking) && CheckMinigame(ranking))
            {
                rewardsToDisplay.Add(ranking);
            }
        }

        return rewardsToDisplay;
    }


    private bool CheckLeague(PlayerStatisticData ranking)
    {
        bool valid = false;
        if (league.Equals("Filter by...") || ranking.GetLeagues().Contains(league))
        {
            valid = true;
        }

        return valid;
    }

    private bool CheckWorld(PlayerStatisticData ranking)
    {
        bool valid = false;
        if (world.Equals("Filter by...") || ranking.GetWorlds().Contains(world))
        {
            valid = true;
        }

        return valid;
    }

    private bool CheckMinigame(PlayerStatisticData ranking)
    {
        bool valid = false;
        if (minigame.Equals("Filter by ...") || ranking.GetMinigames().Contains(minigame))
        {
            valid = true;
        }

        return valid;
    }
    
    private void DisplayRewards(List<PlayerStatisticData> rewardsToDisplay)
    {
        foreach (PlayerStatisticData rank in rewardsToDisplay)
        {
            DisplayRewards(rank);
        }
    }

    private void DisplayRewards(PlayerStatisticData rank)
    {
        GameObject achievementObject = Instantiate(leaderboardPrefab, content.transform, false);

        /*  AchievementUIElement achievementUIElement = achievementObject.GetComponent<AchievementUIElement>();
         if (achievementUIElement != null)
        {
             string title = achievement.GetTitle();
             string description = achievement.GetDescription();
             Sprite image = achievement.GetImage();
             int progress = achievement.GetProgress();
             int amountRequired = achievement.GetAmountRequired();
             bool completed = achievement.IsCompleted();
             achievementUIElement.Setup(title, description, image, progress, amountRequired, completed);
         }
         else
         {
             Destroy(achievementObject);
         }*/
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

        if (Input.GetKeyDown(KeyCode.Return) && distributionPanel != null && distributionPanel.activeSelf)
        {
            CloseDistributionPanel();
        }
    }

    private void OpenDistributionPanel()
    {
        if (distributionPanel != null)
        {
            distributionPanel.SetActive(true);
            Debug.Log("Distribution Panel opened.");

        }
        else
        {
            Debug.LogError("Distribution Panel is not assigned in the Inspector.");
        }
    }

    private void CloseDistributionPanel()
    {
        if (distributionPanel != null)
        {
            distributionPanel.SetActive(false);
            Debug.Log("Distribution Panel closed.");
        }
    }


}
