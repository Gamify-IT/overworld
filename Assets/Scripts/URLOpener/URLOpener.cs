using UnityEngine;

public class URLOpener : MonoBehaviour
{
    public string Path;

    public void Open()
    {
        string Url = Application.absoluteURL.Replace("/app", "/").replace("/overworld", "") + Path;
        
        Application.OpenURL(Url);
    }
}
