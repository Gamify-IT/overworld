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
    [SerializeField] private string category;
    [SerializeField] private TMP_Dropdown statusDropdown;
    [SerializeField] private string status;
    [SerializeField] private TextMeshPro filterinputField;
    [SerializeField] private string filterText;
    private List<AchievementUIElement> achievements;

    private void Start()
    {
        achievements = DataManager.Instance.GetAchievements();
    }
}
