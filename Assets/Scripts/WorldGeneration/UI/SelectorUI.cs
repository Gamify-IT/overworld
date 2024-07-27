using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

/// <summary>
///     Menu where user can select the course, world and dungeon to create/update a new world with the world generation
/// </summary>
public class SelectorUI : MonoBehaviour
{

    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    // UI elements
    [SerializeField] private TMP_Dropdown courseIDDropDownMenu;
    [SerializeField] private TMP_Dropdown wordlIndexDropDownMenu;
    [SerializeField] private TMP_Dropdown dungeonIndexDropDownMenu;

    // world data
    private List<CourseData> courseData;
    private readonly List<int> courseIDs = new List<int>();
    readonly List<string> courseNames = new List<string>();

    private void Awake()
    {
#if UNITY_EDITOR
        List<string> dummyCourses = new() { "Test 1", "Test 2" };
        courseIDDropDownMenu.AddOptions(dummyCourses);
#else
        LoadExistingCourses();
#endif
    }

    /// <summary>
    ///     This function resets the content panel
    /// </summary>
    public void ResetContentPanel()
    {
        courseIDDropDownMenu.value = 0;
        wordlIndexDropDownMenu.value = 0;
        dungeonIndexDropDownMenu.value = 0;
    }

    /// <summary>
    ///     Closes the Selector Menu and returns to the Landing Page
    /// </summary>
    public void QuitButtonPressed()
    {
        CloseOverworld();
    }

    /// <summary>
    ///     Gets all existing courses from the backend
    /// </summary>
    private async void LoadExistingCourses()
    {
        string path = GameSettings.GetOverworldBackendPath() + "/courses/";

        Optional<List<CourseDTO>> courseDTO = await RestRequest.GetListRequest<CourseDTO>(path);

        if (courseDTO.IsPresent())
        {
            courseData = CourseData.ConvertDtoToData(courseDTO.Value());
        }

        FillCourseDropDownMenu();

    }

    /// <summary>
    ///     Puts all existing courses loaded from the backend into the dropdown menu 
    /// </summary>
    private void FillCourseDropDownMenu()
    {
        foreach (CourseData data in courseData)
        {
            courseNames.Add(data.GetCourseName());
            courseIDs.Add(data.GetCourseID());
        }

        courseNames.ForEach(name => Debug.Log("Course Names: " + name));
        courseIDs.ForEach(id => Debug.Log("Course IDs: " + id));

        courseIDDropDownMenu.AddOptions(courseNames);
    }

    /// <summary>
    ///     This function is called by the continue button and saves the entered course ID, dungeon Index (if selected) and world index
    ///     After that, the World Generation continues with the Genrator Menu.
    /// </summary>
    /// <returns></returns>
    public void OnContinueButtonPressed()
    {
# if UNITY_EDITOR
        if (CheckEnteredData())
        {
            // retrieve entered data from dropdownmenus
            string courseID = courseIDDropDownMenu.value.ToString();
            int worldIndex = wordlIndexDropDownMenu.value;
            Optional<int> dungeonIndex = dungeonIndexDropDownMenu.value != 0 ? new Optional<int>(dungeonIndexDropDownMenu.value) : new Optional<int>();

            AreaGeneratorManager.Instance.StartGenerator(courseID, worldIndex, dungeonIndex);
        }
        else
        {
            InterfaceInfo.Instance.DisplayErrorInfo();
        }
#else
        if (CheckEnteredData())
        {
            // retrieve entered data from dropdownmenus
            string courseID = courseIDs[courseIDDropDownMenu.value - 1].ToString();
            int worldIndex = wordlIndexDropDownMenu.value;
            Optional<int> dungeonIndex = dungeonIndexDropDownMenu.value != 0 ? new Optional<int>(dungeonIndexDropDownMenu.value) : new Optional<int>();

            AreaGeneratorManager.Instance.StartGenerator(courseID, worldIndex, dungeonIndex);
        }
        else
        {
            InterfaceInfo.Instance.DisplayErrorInfo();
        }
# endif
    }

    /// <summary>
    /// Checks if the user has selected course and world id
    /// </summary>
    /// <returns>true if values have been selected</returns>
    private bool CheckEnteredData()
    {
        courseIDs.ForEach(id => Debug.Log("id list: " + id));
        if (courseIDDropDownMenu.value != 0 && wordlIndexDropDownMenu.value != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
