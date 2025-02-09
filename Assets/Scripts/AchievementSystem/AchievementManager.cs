using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
///     The <c>AchievementManager</c> sets up the achievements menu and handles all UI inputs
/// </summary>
public class AchievementManager : MonoBehaviour
{
    [SerializeField] private GameObject achievementPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private TMP_Dropdown statusDropdown;
    [SerializeField] private TMP_InputField filterInputField;
    private string category;
    private string status;
    private string filterText;
    private bool filterActive;
    bool completed;
    private List<AchievementData> achievements;

    public void ValidateAllAchievements()
    {
        foreach (var achievement in achievements)
        {
            ValidateCompletionStatus(achievement);
        }
    }
    
    /// <summary>
    ///     Validates the completion status of the achievement based on current progress and requirements.
    /// </summary>
    public void ValidateCompletionStatus(AchievementData achievementData)
    {
        int currentAmountRequired = achievementData.GetAmountRequired();
        int progress = achievementData.GetProgress();
        completed = achievementData.IsCompleted();

        if (progress >= currentAmountRequired && !completed)
        {
            completed = true;
        } 

        if (completed) {
            completed = true;
            achievementData.SetAmountRequired(progress);
        }

        achievementData.SetCompleted(completed); 
    }

    /// <summary>
    ///     This function is called by the categoryDropdown and updates the selected category filter
    /// </summary>
    public void SetCategory()
    {
        int option = categoryDropdown.value;
        category = categoryDropdown.options[option].text;
        UpdateUI();
    }

    /// <summary>
    ///     This function is called by the statusDropdown and updates the selected status filter
    /// </summary>
    public void SetStatus()
    {
        int option = statusDropdown.value;
        status = statusDropdown.options[option].text;
        UpdateUI();
    }

    /// <summary>
    ///     This function sets the filter to the text in the input field
    /// </summary>
    public void SetFilter()
    {
        filterActive = true;
        filterText = filterInputField.text;
        UpdateUI();
    }

    /// <summary>
    ///     This function removes the filter and clears the text in the input field
    /// </summary>
    public void ResetFilter()
    {
        filterActive = false;
        filterText = "";
        filterInputField.text = "";
        UpdateUI();
    }

    private void Start()
    {
        achievements = DataManager.Instance.GetAchievements();
        ValidateAllAchievements();
        Setup();
        UpdateUI();
    }

    /// <summary>
    ///     This function sets the starting values
    /// </summary>
    private void Setup()
    {
        SetupCategoryDropdown();
        category = "All";
        status = "All";
        filterText = "";
        filterActive = false;
    }

    /// <summary>
    ///     This function sets the possible categories of the dropdown menu
    /// </summary>
    private void SetupCategoryDropdown()
    {
        List<string> categories = GetCategories();
        categoryDropdown.ClearOptions();
        categoryDropdown.AddOptions(categories);
    }

    /// <summary>
    ///     This function returns a list of all cateries of the achievements
    /// </summary>
    /// <returns></returns>
    private List<string> GetCategories()
    {
        List<string> categories = new()
        {
            "All"
        };
        foreach (AchievementData achievement in achievements)
        {
            foreach (string category in achievement.GetCategories())
            {
                if (!categories.Contains(category))
                {
                    categories.Add(category);
                }
            }
        }

        return categories;
    }

    /// <summary>
    ///     This function filters the achivements for which need to be shown and updates the UI accordingly
    /// </summary>
    private void UpdateUI()
    {
        ResetUI();
        List<AchievementData> achievementsToDisplay = FilterAchievements();
        DisplayAchievements(achievementsToDisplay);
    }

    /// <summary>
    ///     Deletes all achievement GameObjects
    /// </summary>
    private void ResetUI()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function filters the achievement based on the category, status and filter, if set
    /// </summary>
    /// <returns>A list containing all achievements that satify all constrains</returns>
    private List<AchievementData> FilterAchievements()
    {
        List<AchievementData> achievementsToDisplay = new List<AchievementData>();
        foreach (AchievementData achievement in achievements)
        {
            if (CheckCategory(achievement) && CheckStatus(achievement) && CheckFilter(achievement))
            {
                achievementsToDisplay.Add(achievement);
            }
        }

        return achievementsToDisplay;
    }

    /// <summary>
    ///     This function checks, whether an achievement should be shown based on the category selected
    /// </summary>
    /// <param name="achievement">The achievement to be checked</param>
    /// <returns>True, if the achievement should be shown, false otherwise</returns>
    private bool CheckCategory(AchievementData achievement)
    {
        bool valid = false;
        if (category.Equals("All") || achievement.GetCategories().Contains(category))
        {
            valid = true;
        }

        return valid;
    }

    /// <summary>
    ///     This function checks, whether an achievement should be shown based on the status selected
    /// </summary>
    /// <param name="achievement">The achievement to be checked</param>
    /// <returns>True, if the achievement should be shown, false otherwise</returns>
    private bool CheckStatus(AchievementData achievement)
    {
        bool valid = false;
        if (status.Equals("All") || (achievement.IsCompleted() && status.Equals("Completed")) ||
            (!achievement.IsCompleted() && status.Equals("Locked")))
        {
            valid = true;
        }

        return valid;
    }

    /// <summary>
    ///     This function checks, whether an achievement should be shown based on the filter selected
    /// </summary>
    /// <param name="achievement">The achievement to be checked</param>
    /// <returns>True, if the achievement should be shown, false otherwise</returns>
    private bool CheckFilter(AchievementData achievement)
    {
        bool valid = false;
        if (!filterActive || achievement.GetTitle().ToLower().Contains(filterText.ToLower()) ||
            achievement.GetDescription().ToLower().Contains(filterText.ToLower()))
        {
            valid = true;
        }

        return valid;
    }

    /// <summary>
    ///     This function displays all achievements in the given list
    /// </summary>
    /// <param name="achievementsToDisplay">The achievements to be displayed</param>
    private void DisplayAchievements(List<AchievementData> achievementsToDisplay)
    {
        foreach (AchievementData achievement in achievementsToDisplay)
        {
            DisplayAchievement(achievement);
        }
    }

    /// <summary>
    ///     This function creates a GameObject for the given <c>AchievementData</c>
    /// </summary>
    /// <param name="achievement">The achievement a GameObject should be created for</param>
    private void DisplayAchievement(AchievementData achievement)
    {
        GameObject achievementObject = Instantiate(achievementPrefab, content.transform, false);

        AchievementUIElement achievementUIElement = achievementObject.GetComponent<AchievementUIElement>();
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
        }
    }
}