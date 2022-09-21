using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
///     This class opens a link on the current webserver with path 'path' in current tab.
/// </summary>
public class URLOpenerInCurrentTab : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    /// <summary>
    ///  This function opens a link on the current webserver with path 'path' in current tab.
    ///  It calls a JavaScript method that closes the Overworld IFrame.
    /// </summary>
    public void Open()
    {
        CloseOverworld();
    }
}
