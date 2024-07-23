using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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
    private string courseID;
    private int worldIndex;
    private Optional<int> dungeonIndex;
    private List<CourseData> courseData;

    private void Awake()
    {
        LoadExistingCourses();
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
    /// Closes the Selector Menu and returns to the Landing Page.
    /// </summary>
    public void QuitButtonPressed()
    {
        CloseOverworld();
    }

    /// <summary>
    /// 
    /// </summary>
    async private void LoadExistingCourses()
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
    /// 
    /// </summary>
    private void FillCourseDropDownMenu()
    {
        List<string> courseNames = new List<string>();

        foreach (CourseData courseData in courseData)
        {
            Debug.Log(courseData.GetCourseName());
            courseNames.Add(courseData.GetCourseName());
        }

        courseIDDropDownMenu.AddOptions(courseNames);

    }

    /// <summary>
    /// This function is called by the continue button and saves the entered course ID, dungeon Index (if entered) and world index.
    /// After that, the World Generation continues with the Genrator Menu.
    /// </summary>
    /// <returns></returns>
    public void OnContinueButtonPressed()
    {
        if (CheckEnteredData())
        {
            Debug.Log("Course ID: " + courseIDDropDownMenu.value);
            Debug.Log("World Index: " + wordlIndexDropDownMenu.value);
            Debug.Log("Dungeon Index: " + dungeonIndexDropDownMenu.value);

            // retrieve entered data
            courseID = courseIDDropDownMenu.value.ToString();
            worldIndex = wordlIndexDropDownMenu.value;
            dungeonIndex = dungeonIndexDropDownMenu.value != 0 ? new Optional<int>(dungeonIndexDropDownMenu.value) : new Optional<int>();
            Debug.Log(AreaGeneratorManager.Instance);
            AreaGeneratorManager.Instance.StartGenerator(courseID, worldIndex, dungeonIndex);
        }
        else
        {
            InterfaceInfo.Instance.DisplayErrorInfo();
        }
       
    }

    /// <summary>
    /// Checks if the user has selected course and world id.
    /// </summary>
    /// <returns>true if values have been selected</returns>
    private bool CheckEnteredData()
    {
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
