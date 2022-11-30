using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private int teleporterWorldID;
    [SerializeField]
    private GameObject teleporterCanvas;

    [SerializeField]
    private Vector2[] teleporterPositions = new Vector2[4];

    private Transform player;

    private GameObject currentTeleporterCanvas;
    private TeleporterUI teleporterUI;

    private int finalTargetIndex = 0;
    private bool inTrigger = false;


    /// <summary>
    /// Open TeleporterUI when the player interacts with the teleporter.
    /// </summary>
    void Update()
    {
        if (inTrigger && Input.GetKeyDown("e") && currentTeleporterCanvas == null)
        {
            GameObject newCanvas = GameObject.Instantiate(teleporterCanvas);
            teleporterUI = newCanvas.transform.GetChild(0).GetComponent<TeleporterUI>();
            SetupTeleporterUI(teleporterUI);
            currentTeleporterCanvas = newCanvas;
        }
        

    }
    /// <summary>
    /// Recognize the player entering the teleporter
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            inTrigger = true;
        }
    }

    /// <summary>
    /// Recognize the player exiting the teleporter
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inTrigger = false;
            Destroy(currentTeleporterCanvas);
            currentTeleporterCanvas = null;

        }
    }


    /// <summary>
    /// Setups the buttons inside the TeleporterUI.
    /// </summary>
    /// <param name="ui"></param>
    private void SetupTeleporterUI(TeleporterUI ui)
    {
        List<int> destinationIndices = new List<int> { 0,1, 2, 3};
        destinationIndices.Remove(teleporterWorldID);
        for (int i = 0; i < destinationIndices.Count; i++)
        {
            bool worldUnlocked = true;
            //bool worldUnlocked = DataManager.Instance.IsWorldUnlocked(destinationIndices[i]);
            Button currentButton = ui.targetButtons[i];
            //string worldName = ?
            currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "World " + (destinationIndices[i]+1).ToString();
            currentButton.interactable = worldUnlocked;
            int index = i;
            currentButton.onClick.AddListener(() => TeleportPlayerToWorld(destinationIndices[index]));
        }
        
    }

    /// <summary>
    /// This function is called by a button of the TeleporterUI. It will teleport the player to a specific location.
    /// </summary>
    /// <param name="worldIndex"></param>
    public void TeleportPlayerToWorld(int worldIndex)
    {
        Destroy(currentTeleporterCanvas);
        finalTargetIndex = worldIndex;
        Animation ufoAnimation = GetComponent<Animation>();
        ufoAnimation.Play();
        player.GetComponent<PlayerAnimation>().DisableMovement();
        
    }

    /// <summary>
    /// This function is called by an animation event of the Ufo Animation. It loads the new world and teleports the player there.
    /// </summary>
    public void FinishTeleportation()
    {
        player.position = teleporterPositions[finalTargetIndex];
        player.GetComponent<PlayerAnimation>().EnableMovement();
        player.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void FadeOutPlayer()
    {
        player.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void LoadTargetScene()
    {
        // add triggers at the marketplaces of each world with a LoadMaps Component
        // LoadMaps needs to work without scene origin!
        // or manually:
        StartCoroutine(LoadTargetSceneAsync());
    }

    IEnumerator LoadTargetSceneAsync()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("World1");

        while (!load.isDone)
        {
            
            yield return null;
        }
    }
}
