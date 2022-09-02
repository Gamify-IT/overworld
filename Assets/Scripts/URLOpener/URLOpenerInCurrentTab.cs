using UnityEngine;

/// <summary>
///     This class opens a link on the current webserver with path 'path' in current tab.
/// </summary>
public class URLOpenerInCurrentTab : MonoBehaviour
{
    /// <summary>
    ///     The path that opened by 'Open()'.
    /// </summary>
    public string path;

    /// <summary>
    ///     This function opens a link on the current webserver with path 'path' in current tab.
    /// </summary>
    public void Open()
    {
        Application.ExternalEval("window.open('/" + path + "','_self')");
    }
}
