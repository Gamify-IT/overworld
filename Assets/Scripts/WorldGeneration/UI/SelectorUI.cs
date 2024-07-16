using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SelectorUI : MonoBehaviour
{

    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    [SerializeField] private TMP_Dropdown courseIDDropDownMenu;
    [SerializeField] private TMP_InputField wordlIndexInputField;
    [SerializeField] private TMP_InputField dungeonIndexInputField;

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

    public void OnContinueButtonPressed()
    {
        ContinueButton();
    }

    private void CourseIDDropDownMenu()
    {
        string path = GameSettings.GetOverworldBackendPath() + "/courses/";
        //Optional<> areaDTO = await RestRequest.GetRequest<AreaDTO>(path);
    }

    /// <summary>
    /// This function is called by the continue button and saves the entered course ID, dungeon Index (if entered) and world index.
    /// </summary>
    /// <returns></returns>
    public async UniTask ContinueButton()
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

    public void QuitButtonPressed()
    {
        CloseOverworld();
    }

}
