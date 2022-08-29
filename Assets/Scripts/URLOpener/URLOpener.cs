using UnityEngine;

public class URLOpener : MonoBehaviour
{
    public string Path;

    public void Open()
    {
        string Url = Application.absoluteURL.Replace("/app", "/") + Path;
        
        Application.OpenURL(Url);
    }
}
