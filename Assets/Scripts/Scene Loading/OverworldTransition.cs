using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

/// <summary>
///     This class is used to load the overworld after sucessfully completing the tutorial
/// </summary>
public class OverworldTransition : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    void OnTriggerEnter2D(Collider2D playerCollision)
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        CloseOverworld();
#endif
    }
}
