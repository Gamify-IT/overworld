using UnityEngine;
using System.Runtime.InteropServices;

public class CloseGame : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    /// <summary>
    ///     This function saves all achievement progress and then closes the game
    /// </summary>
    public async void CloseButtonPressed()
    {
        bool success = await GameManager.Instance.SaveAchievements();
        if(success)
        {
            Debug.Log("Saved all achievement progress");
            CloseOverworld();
        }
        else
        {
            Debug.Log("Could not save all achievement progress");
        }
    }
}
