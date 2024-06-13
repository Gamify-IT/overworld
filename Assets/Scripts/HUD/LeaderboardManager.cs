using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public Button openButton; 
    private Button closeButton; 

    private bool isLeaderboardOpen = false;

    private void Start()
    {
        
        if (openButton != null)
        {
            
            openButton.onClick.AddListener(OpenLeaderboardScene);
        }
        else
        {
            Debug.LogError("Open Button is not assigned in the Inspector.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Rewards")
        {
            
            closeButton = GameObject.Find("CloseButton").GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseLeaderboardScene);
            }
            else
            {
                Debug.LogError("Close Button is not found in the RewardsScene.");
            }
        }
    }

    public void OpenLeaderboardScene()
    {
       
        if (!isLeaderboardOpen)
        {
            
            SceneManager.LoadScene("Rewards", LoadSceneMode.Additive);
            isLeaderboardOpen = true;
            Time.timeScale = 0f; 
        }
    }

    public void CloseLeaderboardScene()
    {
        if (isLeaderboardOpen)
        {
            SceneManager.UnloadSceneAsync("Rewards");
            isLeaderboardOpen = false;
            Time.timeScale = 1f; 
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLeaderboardOpen)
        {
            CloseLeaderboardScene();
        }
    }
}
