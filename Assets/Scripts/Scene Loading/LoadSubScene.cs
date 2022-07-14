using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSubScene : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float loadingTime;
    public Vector2 playerPosition;
    public VectorValue playerStorage;

    private void Awake()
    {
        if(fadeInPanel != null)
        {
            GameObject panel = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
            Destroy(panel, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag == "Player")
        {
            playerStorage.initialValue = playerPosition;
            StartCoroutine(FadeCoroutine());
        }
    }

    public IEnumerator FadeCoroutine()
    {
        if(fadeOutPanel != null)
        {
            Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSeconds(loadingTime);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        AsyncOperation loadHUD = SceneManager.LoadSceneAsync("Player HUD", LoadSceneMode.Additive);
        while (!asyncOperation.isDone && !loadHUD.isDone)
        {
            yield return null;
        }
        
    }
}
