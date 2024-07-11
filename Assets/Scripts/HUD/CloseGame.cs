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

    public AudioClip clickSound;
    private AudioSource audioSource;

    private void Start()
    {
        savingCanvas.gameObject.SetActive(false);

        //get AudioSource component
        audioSource=GetComponent<AudioSource>();
        //add AudioSource component if necessary
        if(audioSource==null){
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        //set audio clip
        audioSource.clip=clickSound;
    }

    /// <summary>
    ///     This function saves all achievement progress and then closes the game
    /// </summary>
    public async void CloseButtonPressed()
    {
        PlayClickSound();
        InitCanvas();
        bool success = await GameManager.Instance.SaveAchievements();
        if(success)
        {
            Debug.Log("Saved all achievement progress");
            CloseOverworld();
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

    /// <summary>
    /// This function plays the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if (clickSound!=null && audioSource!=null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
