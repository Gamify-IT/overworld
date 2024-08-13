using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

//new comment

public class LeaderboardManagerUpdate : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject rewardObject;
    [SerializeField] public TMP_Dropdown LeagueDropdown;
    [SerializeField] public TMP_Dropdown WorldDropdown;
    [SerializeField] public TMP_Dropdown MinigameDropdown;
    [SerializeField] private GameObject walletPanel;
    [SerializeField] private WorldData worldNames;
    [SerializeField] private LeagueDefiner leagues;
    [SerializeField] private GameObject VisibilityMenu;
    [SerializeField] private Button visButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text walletField;


    private string league;
    private string world;
    private string minigame;
    private List<PlayerStatisticData> ranking;
    private PlayerStatisticData ownData;
    
    private bool filterActive;

    private Button closeButton;
    public TMP_Text visibilityButton;
    public Button walletButton;
    public Button resetButton;
    public Button changeVisibilityButton;
    public Button closeInputfieldButton;
    public Button closeVisibilityMenuButton;

    private AudioSource audioSource;
    public AudioClip clickSound;


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
        ownData = DataManager.Instance.GetPlayerData();
        ranking = DataManager.Instance.GetAllPlayerStatistics();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;

        if (visibilityButton!= null)
        {

            visButton.onClick.AddListener(OpenVisibilityMenu);


        }
        else
        {
            Debug.LogError("Visibility Button is not assigned in the Inspector.");
        }

        if (changeVisibilityButton != null)
        {
            changeVisibilityButton.onClick.AddListener(ToggleButtonText);
        }
        else
        {
            Debug.LogError("Visibility Change Button is not assigned in the Inspector.");
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
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetFilter);
        }
        else
        {
            Debug.LogError("Reset Button is not assigned in the Inspector.");
        }

        if (inputField != null)
        {
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {ownData.GetPseudonym()}\nChange current pseudonym";

           

           
        }
        else
        {
            Debug.LogError("InputField is not assigned in the Inspector.");
        }

       
       

        if (closeVisibilityMenuButton != null)
        {
            closeVisibilityMenuButton.onClick.AddListener(CloseVisibilityMenu); 
        }
        else
        {
            Debug.LogError("Close Visibility Menu Button is not assigned in the Inspector.");
        }

      


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
        if (visibilityButton.text == "your username is anonymous")
        {
            visibilityButton.text = "your username is public";
            ownData.SetVisibility(true);
            Debug.Log($"Player showRewards value: {ownData.GetShowRewards()}");


        }
        else
        {
            visibilityButton.text = "your username is anonymous";
            ownData.SetVisibility(false);
            Debug.Log($"Player showRewards value: {ownData.GetShowRewards()}");


        }
        SaveVisibilityState();

    }

    private void OpenVisibilityMenu()
    {
        if (VisibilityMenu != null)
        {
            VisibilityMenu.SetActive(true);
            Debug.Log("Visibility Menu opened.");
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Visibility Menu is not assigned in the Inspector.");
        }
    }

    private void CloseVisibilityMenu()
    {
        if (VisibilityMenu != null)
        {
            audioSource.Play();
            VisibilityMenu.SetActive(false);
            Debug.Log("Visibility Menu closed.");
        }
        else
        {
            Debug.LogError("Visibility Menu is not assigned in the Inspector.");
        }
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
            visibilityButton.text = "your username is anonymous"; 
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
        league = "Filter by...";
        world = "Filter by...";

        filterActive = false;

        SetupDropdowns(); 

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
        List<string> leagues = new();
        string playerLeague = ownData.GetLeague();
        leagues.Add(playerLeague);
        foreach (PlayerStatisticData rank in ranking)
        {
            string league = rank.GetLeague();
            
            if (!leagues.Contains(league))
              {
                  leagues.Add(league);
              }
            
        }
        leagues.Add("All");
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


    

    private List<PlayerStatisticData> FilterRewards()
    {
        List<PlayerStatisticData> rewardsToDisplay = new List<PlayerStatisticData>();

        foreach (PlayerStatisticData rank in ranking)
        {
            if (league == "All" || CheckLeague(rank))
            {
                if (CheckWorld(rank))
                {
                    rewardsToDisplay.Add(rank);
                }
            }
        }

        if (league == "Filter by..." && world == "Filter by...")
        {
            rewardsToDisplay = ranking.Where(rank =>
                rank.GetLeague() == ownData.GetLeague() 
            ).ToList();
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

    public void OpenInputField()
    {
        if (inputField != null)
        {
            inputField.gameObject.SetActive(true); 
            inputField.text = ownData.GetPseudonym();
            inputField.Select(); 
            inputField.ActivateInputField();
            SetPseudonym();
        }
        else
        {
            Debug.LogError("InputField is not assigned in the Inspector.");
        }
    }

    public void SetPseudonym()
    {
        if (inputField != null && inputField.gameObject.activeSelf)
        {
            string newPseudonym = inputField.text;
            ownData.SetPseudonym(newPseudonym);
            Debug.Log($"Updated pseudonym of {ownData.GetUsername()} to: {ownData.GetPseudonym()}");
            inputField.text = string.Empty;
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {newPseudonym}\nChange current pseudonym";
            inputField.Select();
            inputField.ActivateInputField();
        }
        else
        {
            Debug.LogError("InputField is not assigned in the Inspector or not active.");
        }
    }



   

    private void DisplayRewards(List<PlayerStatisticData> rewardsToDisplay)
    {
        var sortedRewards = rewardsToDisplay.OrderByDescending(rank => rank.GetRewards()).ToList();

        for (int i = 0; i < sortedRewards.Count; i++)
        {
            DisplayRewards(sortedRewards[i], i + 1);
        }
    }

    private void DisplayRewards(PlayerStatisticData rank, int place)
    {
        GameObject achievementObject = Instantiate(rewardObject, content.transform, false);
        RewardElement rewardElement = achievementObject.GetComponent<RewardElement>();

        if (rewardElement != null)
        {
            string playername;
            if (rank.GetShowRewards())
            {
                playername = rank.GetUsername();
            }
            else
            {
                playername = rank.GetPseudonym();
            }

            if (rank.GetId() == ownData.GetId())
            {
                playername += " (you)";
            }

            int reward = rank.GetRewards();

            rewardElement.Setup(playername, reward, place, place == 1 || place == 2 || place == 3);
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
            audioSource.Play();
            Invoke("UnloadScene", 0.15f);
            isLeaderboardOpen = false;
            Time.timeScale = 1f; 
        }
    }

    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync("Rewards");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLeaderboardOpen)
        {
            CloseLeaderboardScene();
        }

        if (Input.GetKeyDown(KeyCode.Return) && walletPanel != null && walletPanel.activeSelf)
        {
            ClosewalletPanel();
        }

        if (Input.GetKeyDown(KeyCode.Return) && inputField.gameObject.activeSelf)
        {
            SetPseudonym();
        }
    }


    private void OpenwalletPanel()
    {
        if (walletPanel != null)
        {
            walletPanel.SetActive(true);
            Debug.Log("wallet Panel opened.");
            

        }
        else
        {
            Debug.LogError("wallet Panel is not assigned in the Inspector.");
        }
    }

    private void ClosewalletPanel()
    {
        if (walletPanel != null)
        {
            walletPanel.SetActive(false);
            Debug.Log("wallet Panel closed.");
        }
    }

    public void OpenOrCloseInputField()
    {
        if (inputField != null)
        {
            bool isOpen = !inputField.gameObject.activeSelf;
            inputField.gameObject.SetActive(isOpen);

            if (isOpen)
            {
                inputField.text = string.Empty; 
                inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {ownData.GetPseudonym()}\nChange current pseudonym";
                inputField.Select();
                inputField.ActivateInputField();
            }
        }
        else
        {
            Debug.LogError("InputField is not assigned in the Inspector.");
        }
    }

   


    private void CloseInputField()
    {
        if (inputField != null)
        {
            inputField.gameObject.SetActive(false);
        }
    }




}
