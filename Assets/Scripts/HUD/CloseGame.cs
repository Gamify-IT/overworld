using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using TMPro;

public class CloseGame : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    [SerializeField] private Canvas savingCanvas;
    [SerializeField] private TextMeshProUGUI savingText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        savingCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    ///     This function saves all achievement progress and then closes the game
    /// </summary>
    public async void CloseButtonPressed()
    {
        InitCanvas();
        bool achievementsSuccess = await GameManager.Instance.SaveAchievements();
        bool logoutSuccess = await GameManager.Instance.SaveLogoutData();
        if(achievementsSuccess && logoutSuccess)
        {
            Debug.Log("Saved all achievement progress");
            Debug.Log("Saved all player position");
            CloseOverworld();
        } else if (!logoutSuccess)
        {
            Debug.Log("Could not save player position");
            savingText.text = "YOUR CURRENT POSITION COULD NOT BE SAVED, DO YOU WANT TO LEAVE AND RISK LOOSING YOUR CURRENT POSITION?";
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Could not save all achievement progress");
            savingText.text = "NOT ALL PROGRESS COULD BE SAVED, DO YOU WANT TO LEAVE AND RISK LOOSING THE PROGRESS OF SOME ACHIEVEMENTS?";
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    ///     This function sets the canvas to the default values
    /// </summary>
    private void InitCanvas()
    {
        savingText.text = "SAVING ACHIEVEMENT PROGRESS...";
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        savingCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    ///      This function is called by the confirm button and leaves the game without saving achievement progress
    /// </summary>
    public void ConfirmButtonPressed()
    {
        CloseOverworld();
    }

    /// <summary>
    ///     This function is called by the cancel button and returns to the main menu
    /// </summary>
    public void CancelButtonPressed()
    {
        savingCanvas.gameObject.SetActive(false);
    }
}
