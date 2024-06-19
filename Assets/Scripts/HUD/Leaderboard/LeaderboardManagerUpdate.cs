using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;



public class LeaderboardManagerUpdate : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject rewardObject;
    [SerializeField] public TMP_Dropdown LeagueDropdown;
    [SerializeField] public TMP_Dropdown WorldDropdown;
    [SerializeField] public TMP_Dropdown MinigameDropdown;
    [SerializeField] private GameObject distributionPanel;
    [SerializeField] private WorldData worldNames;
    [SerializeField] private LeagueDefiner leagues;
    private string league;
    private string world;
    private string minigame;
    private List<PlayerStatisticData> ranking;
    private PlayerStatisticData ownData;
    
    private bool filterActive;

    private Button closeButton;
    public TMP_Text visibilityButton;
    public Button distributionButton;

    private bool isLeaderboardOpen = true;

    public void SetLeague()
    {
        int option = LeagueDropdown.value;
        league = LeagueDropdown.options[option].text;
        Debug.Log($"Selected league: {league}");

        UpdateUI();
    }

    public void SetWorld()
    {
        int option = WorldDropdown.value;
        world = WorldDropdown.options[option].text;
        UpdateUI();
    }

    private void Start()
    {    

      

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

        if (LeagueDropdown == null)
        {
            Debug.LogError("LeagueDropdown is not assigned in the Inspector.");
        }
        else
        {
            Debug.Log("LeagueDropdown is assigned: " + LeagueDropdown.gameObject.name);
        }

        if (WorldDropdown == null)
        {
            Debug.LogError("WorldDropdown is not assigned in the Inspector.");
        }
        else
        {
            Debug.Log("WorldDropdown is assigned: " + WorldDropdown.gameObject.name);
        }

        ranking = DataManager.Instance.GetAllPlayerStatistics();
        ownData = DataManager.Instance.GetOwnStatisticData();


        Debug.Log($"My player is: {ownData.GetUsername()},Rewards: {ownData.GetRewards()}");


        foreach (var playerData in ranking)
        {
            Debug.Log($"Player added to display: {playerData.GetUsername()},Rewards: {playerData.GetRewards()}, League: {playerData.GetLeague()}, World: {playerData.GetWorld()}");
        }

        Setup();

        
        UpdateUI();


    }

    private void Setup()
    {
        SetupDropdowns();
        league = "Filter by...";
        world = "Filter by...";
        
        filterActive = false;
    }

    private void SetupDropdowns()
    {
        List<string> leagues = GetLeague();
        LeagueDropdown.ClearOptions();
        LeagueDropdown.AddOptions(leagues);

        List<string> worlds = GetWorld();
        WorldDropdown.ClearOptions();
        WorldDropdown.AddOptions(worlds);

    }

    private void ToggleButtonText()
    {
        if (visibilityButton.text == "hide me")
        {
            visibilityButton.text = "show me";
            ownData.updateVisibility(true);
            Debug.Log($"Player showRewards value: {ownData.GetShowRewards()}");

        }
        else
        {
            visibilityButton.text = "hide me";
            ownData.updateVisibility(false);
            Debug.Log($"Player showRewards value: {ownData.GetShowRewards()}");


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

  

  public void SetMinigame()
    {
        int option = MinigameDropdown.value;
        minigame = MinigameDropdown.options[option].text;
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

    private List<string> GetLeague()
    {
        List<string> leagues = new()
        {
            "Filter by..."
        };
        foreach (PlayerStatisticData rank in ranking)
        {
            string league = rank.GetLeague();
            
            if (!leagues.Contains(league))
              {
                  leagues.Add(league);
              }
            
        }

        return leagues;
    }



    private List<string> GetWorld()
    {
        List<string> worlds = new()
        {
            "Filter by..."
        };
        foreach (PlayerStatisticData rank in ranking)
        {
            string world = rank.GetWorld();
                if (!worlds.Contains(world))
                {
                    worlds.Add(world);
                }
            
        }

        return worlds;
    }


    /*private List<string> GetMinigames()
     {
        List<string> minigames = new()
        {
            "Filter by..."
         };
         foreach (PlayerStatisticData rank in ranking)
        {
             foreach (string minigame in rank.GetWorld())
             {
                if (!minigames.Contains(minigame))
                {
                    minigames.Add(minigame);
                 }
             }
         }

        return minigames;
    }*/




    private List<PlayerStatisticData> FilterRewards()
    {
        List<PlayerStatisticData> rewardsToDisplay = new List<PlayerStatisticData>();

        foreach (PlayerStatisticData rank in ranking)
        {
            if (rank.GetShowRewards() && CheckLeague(rank) && CheckWorld(rank))
            {
                rewardsToDisplay.Add(rank);
                Debug.Log($"Player added to display: {rank.GetUsername()}, League: {rank.GetLeague()}");
            }
        }

        return rewardsToDisplay;
    }


    private bool CheckLeague(PlayerStatisticData ranking)
    {
        bool valid = false;
        if (league.Equals("Filter by...") || ranking.GetLeague().Equals(league))
        {
            valid = true;
        }

        return valid;
    }

    private bool CheckWorld(PlayerStatisticData ranking)
    {
        bool valid = false;
        if (world.Equals("Filter by...") || ranking.GetWorld().Equals(world))
        {
            valid = true;
        }

        return valid;
    }

    //private bool CheckMinigame(PlayerStatisticData ranking)
    //{
      //  bool valid = false;
        //if (minigame.Equals("Filter by ...") || ranking.GetMinigames().Contains(minigame))
        //{
          //  valid = true;
        //}

        //return valid;
    //}
    
    private void DisplayRewards(List<PlayerStatisticData> rewardsToDisplay)
    {
        // Ordnung des Leaderboards
        var sortedRewards = rewardsToDisplay.OrderByDescending(rank => rank.GetRewards()).ToList();

        for (int i = 0; i < sortedRewards.Count; i++)
        {
            // Platzierung
            DisplayRewards(sortedRewards[i], i + 1);
        }
    }

    private void DisplayRewards(PlayerStatisticData rank, int place)
    {
        GameObject achievementObject = Instantiate(rewardObject, content.transform, false);

        RewardElement rewardElement = achievementObject.GetComponent<RewardElement>(); // Hier wird das korrekte Component geholt

        if (rewardElement != null)
        {
            string playername = rank.GetUsername();
            int reward = rank.GetRewards();

            rewardElement.Setup(playername, reward, place, place == 1);
        }
        else
        {
            Destroy(achievementObject);
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
