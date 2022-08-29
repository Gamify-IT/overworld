using UnityEngine;

public class URLOpener : MonoBehaviour
{
    public string Path;

    public void Open()
    {
        string Url = Application.absoluteURL.Replace("/app", "/").Replace("/overworld", "") + Path;
        
        Application.OpenURL(Url);
    }
}
