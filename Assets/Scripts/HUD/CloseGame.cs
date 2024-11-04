using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

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

        audioSource=GetComponent<AudioSource>();
        if(audioSource==null){
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip=clickSound;
    }

    /// <summary>
    ///     This function saves all achievement progress and then closes the game
    /// </summary>
    public async void CloseButtonPressed()
    {
        PlayClickSound();
        InitCanvas();

        if (GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            Debug.Log("quitting tutorial");
            savingText.text = "ARE YOU SURE YOU WANT TO QUIT THE TUTORIAL?";
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            return;
        }

        bool playerDataSaved = await GameManager.Instance.SavePlayerData();

        if (playerDataSaved)
        {
            Debug.Log("Saved all progress");
            CloseOverworld();
        }
        else
        {
            Debug.Log("Could not save all progress");
            savingText.text = "NOT ALL PROGRESS COULD BE SAVED, DO YOU WANT TO LEAVE AND RISK LOOSING PROGRESS?";
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    ///     This function sets the canvas to the default values
    /// </summary>
    private void InitCanvas()
    {
        savingText.text = "SAVING PROGRESS...";
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        savingCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    ///      This function is called by the confirm button and leaves the game without saving achievement progress
    /// </summary>
    public void ConfirmButtonPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        CloseOverworld();
#endif
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
