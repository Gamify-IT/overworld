using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;


/// <summary>
///     This class manages the leaderboard such as placements, visibility, filter
/// </summary>
public class LeaderboardManager : MonoBehaviour
{

    [SerializeField] private GameObject content; 
    [SerializeField] private GameObject rewardObject; 
    [SerializeField] public TMP_Dropdown LeagueDropdown; 
    [SerializeField] public TMP_Dropdown WorldDropdown; 
    [SerializeField] private GameObject VisibilityMenu; 
    [SerializeField] private Button visibilityButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Toggle visibilityToggle;
    [SerializeField] private Image toggleBackground;
    [SerializeField] private Image visibilityImage; 
    [SerializeField] private TextMeshProUGUI toggleText;
    [SerializeField] private Button confirmButton; 



    private string league;
    private string world;
    private List<PlayerStatisticData> ranking;
    private PlayerStatisticData ownData;

    private Button closeButton;
    public Button resetButton;
    public Button closeVisibilityMenuButton;

    private bool previousToggleState;
    private bool isLeaderboardOpen = true;

    private Color pastelRed = new Color(1f, 0.6f, 0.6f);  
    private Color pastelGreen = new Color(0.6f, 1f, 0.6f);

    private AudioSource audioSource;
    public AudioClip clickSound;


    /// <summary>
    /// Initializes the leaderboard manager.
    /// This method is called when the script is first run.
    /// Sets up data, audio, UI listeners, and the initial state of the UI.
    /// </summary>
    private void Start()
    {
        FetchAndInitializePlayerData();
      
    }

    private async Task FetchAndInitializePlayerData()
    {
        bool errorLoadingPlayerData = await GameManager.Instance.FetchUserData();
        if (errorLoadingPlayerData)
        {
            Debug.LogError("Error loading player data.");
            return;
        }

        InitializeData();

        UpdateUI();

        InitializeAudioSource();
        SetupUIListeners();
        Setup();
        UpdateUI();
    }

    /// <summary>
    /// Retrieves and sets up the player’s own data and the ranking data.
    /// </summary>
    private void InitializeData()
    {
        ownData = DataManager.Instance.GetPlayerData();
        ranking = DataManager.Instance.GetAllPlayerStatistics();
    }

    /// <summary>
    /// Initializes the audio source component for playing sounds.
    /// Adds an AudioSource component if one is not already attached.
    /// </summary>
    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Sets up event listeners for UI elements such as buttons and toggles.
    /// Configures the placeholder text of the input field and loads the saved visibility state.
    /// </summary>
    private void SetupUIListeners()
    {
        visibilityButton?.onClick.AddListener(OpenVisibilityMenu);
        resetButton?.onClick.AddListener(ResetFilter);
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {ownData.GetPseudonym()}\nChange current pseudonym";
        confirmButton?.onClick.AddListener(ConfirmPseudonym);
        closeVisibilityMenuButton?.onClick.AddListener(CloseVisibilityMenu);
        visibilityToggle?.onValueChanged.AddListener(OnToggleChanged);
        LoadVisibilityState();
    }

    /// <summary>
    /// Performs initial setup tasks, such as configuring dropdowns and filter states.
    /// </summary>
    private void Setup()
    {
        SetupDropdowns();
        league = "Filter by...";
        world = "Filter by...";
    }

    /// <summary>
    /// Configures the options for the League and World dropdown menus.
    /// </summary>
    private void SetupDropdowns()
    {
        List<string> leagues = GetLeague();
        LeagueDropdown.ClearOptions();
        LeagueDropdown.AddOptions(leagues);

        List<string> worlds = GetWorld();
        WorldDropdown.ClearOptions();
        WorldDropdown.AddOptions(worlds);
    }

    /// <summary>
    /// Opens the visibility menu and updates the visibility icon based on the current toggle state.
    /// </summary>
    private void OpenVisibilityMenu()
    {
        if (VisibilityMenu != null)
        {
            VisibilityMenu.SetActive(true);
            audioSource.Play();
            bool isPublic = visibilityToggle.isOn;
            UpdateVisibilityImage(isPublic);
        }        
    }

    /// <summary>
    /// Closes the visibility menu and plays a click sound.
    /// </summary>
    private void CloseVisibilityMenu()
    {
        if (VisibilityMenu != null)
        {
            audioSource.Play();
            VisibilityMenu.SetActive(false);
        }
        
    }

    /// <summary>
    /// Saves the current state of the visibility toggle to player preferences.
    /// </summary>
    private void SaveVisibilityState()
    {
        PlayerPrefs.SetInt("VisibilityState", visibilityToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the visibility state from player preferences and updates the UI accordingly.
    /// </summary>
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

    /// <summary>
    /// Resets the filter settings and updates the UI to reflect these changes.
    /// </summary>
    public void ResetFilter()
    {
        league = "Filter by...";
        world = "Filter by...";
        SetupDropdowns();
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI by resetting current elements, filtering rewards, and displaying them.
    /// </summary>
    private void UpdateUI()
    {
        ResetUI();
        List<PlayerStatisticData> rewardsToDisplay = FilterRewards();
        DisplayRewards(rewardsToDisplay);
    }

    /// <summary>
    /// Clears all child objects from the content container.
    /// </summary>
    private void ResetUI()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Retrieves a list of unique league names from the ranking data.
    /// </summary>
    /// <returns>A list of league names including "All".</returns>
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

    // <summary>
    /// Retrieves a list of unique world names from the ranking data.
    /// </summary>
    /// <returns>A list of world names with "Filter by..." as the default option.</returns>
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

    /// <summary>
    /// Filters the list of rewards based on the current league and world filters.
    /// </summary>
    /// <returns>A list of player statistics that match the filter criteria.</returns>    
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

    /// <summary>
    /// Checks if a player’s league matches the selected league filter.
    /// </summary>
    /// <param name="ranking">The player’s ranking data.</param>
    /// <returns>True if the league matches or if no filter is applied; otherwise, false.</returns>
    private bool CheckLeague(PlayerStatisticData ranking)
    {
        return league.Equals("Filter by...") || ranking.GetLeague().Equals(league);
    }

    // <summary>
    /// Checks if a player’s world matches the selected world filter.
    /// </summary>
    /// <param name="ranking">The player’s ranking data.</param>
    /// <returns>True if the world matches or if no filter is applied; otherwise, false.</returns>
    private bool CheckWorld(PlayerStatisticData ranking)
    {
        return world.Equals("Filter by...") || ranking.GetWorld().Equals(world);
    }

    /// <summary>
    /// Opens the input field for changing the pseudonym and sets its initial value.
    /// </summary>
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
       
    }

    /// <summary>
    /// Updates the player’s pseudonym and refreshes the ranking data.
    /// Only saves if the new pseudonym is valid (non-empty).
    /// </summary>
    public void SetPseudonym()
    {
        if (inputField != null && inputField.gameObject.activeSelf)
        {
            string newPseudonym = inputField.text.Trim();

            if (string.IsNullOrEmpty(newPseudonym))
            {
                Debug.LogWarning("Pseudonym cannot be empty. Please enter a valid pseudonym.");
                return;
            }

            DataManager.Instance.UpdatePseudonym(newPseudonym);
            GameManager.Instance.SavePlayerData();

            inputField.text = string.Empty;
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {newPseudonym}\nClick here to change your pseudonym";

            Debug.Log($"Pseudonym successfully updated to: {newPseudonym}");
        }
    }


    /// <summary>
    /// Displays a list of rewards, grouping them by their reward value and sorting them in descending order.
    /// Rewards are displayed with ranks, and the top few ranks are highlighted.
    /// </summary>
    /// <param name="rewardsToDisplay">A list of player statistics to be displayed in the leaderboard.</param>
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

    /// <summary>
    /// Instantiates and sets up the display of a single player's reward.
    /// Configures the appearance of the reward element based on the player's visibility and rank.
    /// </summary>
    /// <param name="rank">The player's ranking data to be displayed.</param>
    /// <param name="place">The rank place to be assigned to this player.</param>
    /// <param name="highlight">Whether this reward should be highlighted.</param>
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

    /// <summary>
    /// Subscribes to the scene loaded event when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the scene loaded event when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Handles actions when a new scene is loaded.
    /// Specifically, sets up the close button if the "Rewards" scene is loaded and updates the UI.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The mode in which the scene was loaded.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Rewards")
        {
            closeButton = GameObject.Find("CloseButton").GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseLeaderboardScene);
            }
            
            UpdateUI();
        }
    }

    /// <summary>
    /// Closes the leaderboard scene and unloads it after a brief delay.
    /// </summary>
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

    /// <summary>
    /// Unloads the "Rewards" scene asynchronously.
    /// </summary>
    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync("Rewards");
    }

    /// <summary>
    /// Updates the leaderboard and pseudonym input field based on user input and toggle state.
    /// Handles key presses for closing the scene and updating the pseudonym.
    /// </summary>
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && isLeaderboardOpen)
        {
            CloseLeaderboardScene();
        }

        if (inputField != null && confirmButton != null)
        {
            confirmButton.interactable = !string.IsNullOrWhiteSpace(inputField.text);
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

    /// <summary>
    /// Toggles the visibility of the input field for changing the pseudonym.
    /// Updates the placeholder text and activates the input field if opened.
    /// </summary>
    public void OpenOrCloseInputField()
    {
        if (inputField != null)
        {
            bool isOpen = !inputField.gameObject.activeSelf;
            inputField.gameObject.SetActive(isOpen);

            if (isOpen)
            {
                inputField.text = string.Empty;
                inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {ownData.GetPseudonym()}\nClick here to change your current pseudonym";
                inputField.Select();
                inputField.ActivateInputField();
            }
        }
        
    }

    /// <summary>
    /// Updates the color of the toggle button background and text based on the visibility state.
    /// </summary>
    /// <param name="isPublic">The visibility state (true for public, false for private).</param>
    private void UpdateToggleButtonColor(bool isPublic)
    {
        if (toggleBackground != null)
        {
            toggleBackground.color = isPublic ? pastelGreen : pastelRed;  
        }
       
        if (toggleText != null)
        {
            toggleText.color = isPublic ? pastelGreen : pastelRed;
        }  
    }

    /// <summary>
    /// Handles changes to the visibility toggle.
    /// Updates the button color, text, and visibility image, and refreshes player data and UI.
    /// </summary>
    /// <param name="isOn">The new state of the toggle (true for public, false for private).</param>
    private void OnToggleChanged(bool isOn)
    {
        UpdateToggleButtonColor(isOn);
        UpdateToggleText(isOn); 
        DataManager.Instance.UpdateVisibility(isOn);
        GameManager.Instance.SavePlayerData();
        SaveVisibilityState();
        UpdateVisibilityImage(isOn);
        ranking = DataManager.Instance.GetAllPlayerStatistics();
        UpdateUI();
    }

    /// <summary>
    /// Updates the text displayed on the visibility toggle based on the current state.
    /// </summary>
    /// <param name="isPublic">The visibility state (true for public, false for private).</param>
    private void UpdateToggleText(bool isPublic)
    {
        if (toggleText != null)
        {
            toggleText.text = isPublic ? "Public" : "Private";
        }
        
    }

    /// <summary>
    /// Updates the alpha value of the visibility image based on the visibility state.
    /// </summary>
    /// <param name="isPublic">The visibility state (true for public, false for private).</param>
    private void UpdateVisibilityImage(bool isPublic)
    {
        if (visibilityImage != null)
        {
            Color imageColor = visibilityImage.color; 
            imageColor.a = isPublic ? 1f : 0.5f;     
            visibilityImage.color = imageColor;       
        }
        
    }

    private void ConfirmPseudonym()
    {
        if (inputField != null && inputField.gameObject.activeSelf)
        {
            string newPseudonym = inputField.text.Trim();

            if (!string.IsNullOrEmpty(newPseudonym))
            {
                DataManager.Instance.UpdatePseudonym(newPseudonym);
                GameManager.Instance.SavePlayerData();

                inputField.text = string.Empty;
                inputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Current pseudonym: {newPseudonym}\nClick here to change your pseudonym";

                Debug.Log($"Pseudonym successfully updated to: {newPseudonym}");
            }
            else
            {
                Debug.LogWarning("Pseudonym cannot be empty!");
            }
        }
    }


}
