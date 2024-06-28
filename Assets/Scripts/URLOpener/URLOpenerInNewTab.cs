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

    public AudioClip clickSound;
    private AudioSource audioSource;

    /// <summary>
    ///     This function opens a link on the current webserver with path 'path' in new tab.
    /// </summary>
    public void Open()
    {
        Application.OpenURL("/" + path);
        audioSource=GetComponent<AudioSource>();
        if(audioSource==null){
            audioSource=gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip=clickSound;
        PlayClickSound();
    }

    private void PlayClickSound(){
    if (clickSound!=null && audioSource!=null)
    {
        audioSource.PlayOneShot(clickSound);
    }
}
}
