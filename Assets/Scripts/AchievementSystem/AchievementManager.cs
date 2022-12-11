using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
///     The <c>AchievementManager</c> sets up the achievements menu and handles all UI inputs
/// </summary>
public class AchievementManager : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Dropdown categoryDropdown;   
    [SerializeField] private TMP_Dropdown statusDropdown;
    [SerializeField] private TMP_Text filterinputField;
    private string category;
    private string status;
    private string filterText;
    private List<AchievementUIElement> achievements;

    private void Start()
    {
        achievements = DataManager.Instance.GetAchievements();
        Setup();
    }

    private void Setup()
    {
        SetupCategoryDropdown();
        category = "All";
        status = "All";
        filterText = "";
    }


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
        foreach (AchievementUIElement achievement in achievements)
        {
            foreach(string category in achievement.GetCategories())
            {
                if(!categories.Contains(category))
                {
                    categories.Add(category);
                }
            }
        }
        return categories;
    }
}
