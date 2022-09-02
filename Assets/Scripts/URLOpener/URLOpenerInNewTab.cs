using UnityEngine;

/// <summary>
///     This class opens a link on the current webserver with path 'path' in new tab.
/// </summary>
public class URLOpenerInNewTab : MonoBehaviour
{
    /// <summary>
    ///     The path that opened by 'Open()'.
    /// </summary>
    public string path;

    /// <summary>
    ///     This function opens a link on the current webserver with path 'path' in new tab.
    /// </summary>
    public void Open()
    {
        Application.OpenURL("/" + path);
    }
}
