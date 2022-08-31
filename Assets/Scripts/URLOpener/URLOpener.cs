using UnityEngine;

/// <summary>
///     This class opens a link on the current webserver with path 'Path'.
/// </summary>
public class URLOpener : MonoBehaviour
{
    /// <summary>
    ///     The path that opened by 'Open()'.
    /// </summary>
    public string Path;

    /// <summary>
    ///     This function opens a link on the current webserver with path 'Path'.
    /// </summary>
    public void Open()
    {
        string Url = Application.absoluteURL.Replace("/app", "/").Replace("/overworld", "") + Path;

        Application.OpenURL(Url);
    }
}