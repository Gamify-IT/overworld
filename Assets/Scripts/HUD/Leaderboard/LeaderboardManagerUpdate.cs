using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardManagerUpdate : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject rewardObject;
    [SerializeField] public TMP_Dropdown LeagueDropdown;
    [SerializeField] public TMP_Dropdown WorldDropdown;
    [SerializeField] private WorldData worldNames;
    [SerializeField] private LeagueDefiner leagues;
    [SerializeField] private GameObject VisibilityMenu;
    [SerializeField] private Button visButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Toggle visibilityToggle;
    [SerializeField] private Image toggleBackground;
    [SerializeField] private Image visibilityImage; 


    [SerializeField] private TextMeshProUGUI toggleText;


    private string league;
    private string world;
    private List<PlayerStatisticData> ranking;
    private PlayerStatisticData ownData;
    private bool filterActive;
    private Button closeButton;
    public Button resetButton;
    public Button closeVisibilityMenuButton;

    private Color pastelRed = new Color(1f, 0.6f, 0.6f);  
    private Color pastelGreen = new Color(0.6f, 1f, 0.6f);

    private AudioSource audioSource;
    public AudioClip clickSound;
    private bool previousToggleState;

    private bool isLeaderboardOpen = true;

    private void Start()
    {
        ownData = DataManager.Instance.GetOwnStatisticData();
        ranking = DataManager.Instance.GetAllPlayerStatistics();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;

        if (visButton != null)
        {
            visButton.onClick.AddListener(OpenVisibilityMenu);
        }
        else
        {
            Debug.LogError("Visibility Button is not assigned in the Inspector.");
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

        if (visibilityToggle != null)
        {
            visibilityToggle.onValueChanged.AddListener(OnToggleChanged);
            LoadVisibilityState();
        }
        else
        {
            Debug.LogError("Visibility Toggle is not assigned in the Inspector.");
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

    private void OpenVisibilityMenu()
    {
        if (VisibilityMenu != null)
        {
            VisibilityMenu.SetActive(true);
            Debug.Log("Visibility Menu opened.");
            audioSource.Play();
            bool isPublic = visibilityToggle.isOn;
            UpdateVisibilityImage(isPublic);
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
        PlayerPrefs.SetInt("VisibilityState", visibilityToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadVisibilityState()
    {
        if (PlayerPrefs.HasKey("VisibilityState"))
        {
            bool isPublic = PlayerPrefs.GetInt("VisibilityState") == 1;
            visibilityToggle.isOn = isPublic;
            UpdateToggleButtonColor(isPublic);
            UpdateToggleText(isPublic);
        }
        else
        {
            visibilityToggle.isOn = false;
            SaveVisibilityState();
            UpdateToggleButtonColor(false);
            UpdateToggleText(false);
        }
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
        List<string> worlds = new() { "Filter by..." };
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
            rewardsToDisplay = ranking.Where(rank => rank.GetLeague() == ownData.GetLeague()).ToList();
        }
        return rewardsToDisplay;
    }

    private bool CheckLeague(PlayerStatisticData ranking)
    {
        return league.Equals("Filter by...") || ranking.GetLeague().Equals(league);
    }

    private bool CheckWorld(PlayerStatisticData ranking)
    {
        return world.Equals("Filter by...") || ranking.GetWorld().Equals(world);
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
            GameManager.Instance.UpdatePseudonym(newPseudonym);
            GameManager.Instance.SavePlayerData();
            ranking = DataManager.Instance.GetAllPlayerStatistics();

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
        var groupedRewards = rewardsToDisplay
            .GroupBy(rank => rank.GetRewards())
            .OrderByDescending(group => group.Key)
            .ToList();

        int currentRank = 1;
        int highlightRankLimit = 3;
        int uniqueRankCount = 0;

        foreach (var group in groupedRewards)
        {
            var sortedGroup = group.OrderBy(rank => rank.GetUsername()).ToList();
            int groupSize = sortedGroup.Count;

            bool shouldHighlight = uniqueRankCount < highlightRankLimit;

            foreach (var player in sortedGroup)
            {
                DisplayRewards(player, currentRank, shouldHighlight);
            }

            uniqueRankCount++;
            currentRank += 1;
        }
    }

    private void DisplayRewards(PlayerStatisticData rank, int place, bool highlight)
    {
        GameObject achievementObject = Instantiate(rewardObject, content.transform, false);
        RewardElement rewardElement = achievementObject.GetComponent<RewardElement>();

        if (rewardElement != null)
        {
            string playername;
            if (rank.GetVisibility())
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
            rewardElement.Setup(playername, reward, place, highlight);
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
            UpdateUI();
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

        if (Input.GetKeyDown(KeyCode.Return) && inputField.gameObject.activeSelf)
        {
            SetPseudonym();
        }

        if (visibilityToggle != null)
        {
            bool isOn = visibilityToggle.isOn;

            if (isOn != previousToggleState)
            {
                OnToggleChanged(isOn);
                previousToggleState = isOn;
            }
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

    private void UpdateToggleButtonColor(bool isPublic)
    {
        if (toggleBackground != null)
        {
            toggleBackground.color = isPublic ? pastelGreen : pastelRed;  
        }
        else
        {
            Debug.LogError("Toggle Background Image is not assigned in the Inspector.");
        }

        if (toggleText != null)
        {
            toggleText.color = isPublic ? pastelGreen : pastelRed;
        }
        else
        {
            Debug.LogError("Toggle TextMeshProUGUI is not assigned in the Inspector.");
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        UpdateToggleButtonColor(isOn);
        UpdateToggleText(isOn); 
        GameManager.Instance.UpdateVisibility(isOn);
        GameManager.Instance.SavePlayerData();
        SaveVisibilityState();
        UpdateVisibilityImage(isOn);
        ranking = DataManager.Instance.GetAllPlayerStatistics();
        UpdateUI();
    }

    private void UpdateToggleText(bool isPublic)
    {
        if (toggleText != null)
        {
            toggleText.text = isPublic ? "Public" : "Private";
        }
        else
        {
            Debug.LogError("Toggle TextMeshProUGUI is not assigned in the Inspector.");
        }
    }

    private void UpdateVisibilityImage(bool isPublic)
    {
        if (visibilityImage != null)
        {
            Color imageColor = visibilityImage.color; 
            imageColor.a = isPublic ? 1f : 0.5f;     
            visibilityImage.color = imageColor;       
        }
        else
        {
            Debug.LogError("Visibility Image is not assigned in the Inspector.");
        }
    }

}
