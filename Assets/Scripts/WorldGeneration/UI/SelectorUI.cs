using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class SelectorUI : MonoBehaviour
{

    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    // UI elements
    [SerializeField] private TMP_Dropdown courseIDDropDownMenu;
    [SerializeField] private TMP_InputField wordlIndexInputField;
    [SerializeField] private TMP_InputField dungeonIndexInputField;

    // world data
    private string courseID;
    private int worldIndex;
    private Optional<int> dungeonIndex;

    /// <summary>
    ///     This function resets the content panel
    /// </summary>
    public void ResetContentPanel()
    {
        courseIDDropDownMenu.value = 0;
        wordlIndexInputField.text = "";
        dungeonIndexInputField.text = "";
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
    private void CourseIDDropDownMenu()
    {
        string path = GameSettings.GetOverworldBackendPath() + "/courses/";
        // TODO: retieve all courses from backend via get request
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
            Debug.Log("World Index: " + wordlIndexInputField.text);
            Debug.Log("Dungeon Index: " + dungeonIndexInputField.text);

            // retrieve entered data
            courseID = courseIDDropDownMenu.value.ToString();
            worldIndex = int.Parse(wordlIndexInputField.text);
            dungeonIndex = dungeonIndexInputField.text != "" ? new Optional<int>(int.Parse(dungeonIndexInputField.text)) : new Optional<int>();
            Debug.Log(AreaGeneratorManager.Instance);
            AreaGeneratorManager.Instance.StartGenerator(courseID, worldIndex, dungeonIndex);
        }
        else
        {
            InterfaceInfo.Instance.DisplayErrorInfo();
        }
       
    }

    /// <summary>
    /// Checks if the entered data by the users is correect, i.e., a couse is selected and the world and dungeon index are correct.
    /// </summary>
    /// <returns></returns>
    private bool CheckEnteredData()
    {
        Regex worldIndexValue = new(@"^[1-9]\d*$");
        Regex dungeonIndexValue = new(@"^[1-4]$");

        if (courseIDDropDownMenu.value != 0 && worldIndexValue.IsMatch(wordlIndexInputField.text) && dungeonIndexValue.IsMatch(dungeonIndexInputField.text))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
