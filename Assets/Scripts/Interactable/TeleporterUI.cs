using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// This class is used to handle the Teleporter UI
/// </summary>
public class TeleporterUI : MonoBehaviour
{
    [SerializeField]
    private GameObject prototypeButton;
    [SerializeField]
    private GameObject prototypeToggle;
    [SerializeField]
    private Transform worldSelectionContent;
    [SerializeField]
    private Transform teleporterSelectionContent;
    [SerializeField]
    private AudioClip clickSound;

    private List<GameObject> currentTeleporterButtons = new List<GameObject>();

    private Teleporter correspondingTeleporter;
    private Color standardButtonColor = new Color(1, 0.9176471f, 0.7960784f);

    private bool isMenuInitialized = false;
    private bool isFirstInteraction = true;

    private void Awake()
    {
        clickSound = Resources.Load<AudioClip>("Music/click");
    }

    /// <summary>
    /// Sets up the buttons according to the available data.
    /// </summary>
    /// <param name="teleporter"></param>
    public void SetupUI(Teleporter teleporter)
    {
        correspondingTeleporter = teleporter;

        if (GameSettings.GetGamemode() == Gamemode.TUTORIAL)
        {
            ShowTutorialTeleporterUi();
            return;
        }

        for (int i = 0; i <= GameSettings.GetMaxWorlds(); i++)
        {
            int worldIndex = i;
            List<TeleporterData> dataList = DataManager.Instance.GetUnlockedTeleportersInWorld(worldIndex);
            if (dataList.Count == 0)
            {
                continue;
            }
            if (worldIndex == teleporter.worldID && dataList.Count == 1)
            {
                continue;
            }
            GameObject newToggle = GameObject.Instantiate(prototypeToggle, worldSelectionContent);
            newToggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetWorldName(worldIndex);
            Image image = newToggle.GetComponent<Image>();
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => ToggleEnabledColor(b, image));
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => UpdateTeleporterSelections(worldIndex,dataList));
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => HandleClick(newToggle));
            newToggle.SetActive(true);
            AddAudioSource(newToggle);
        }
        isMenuInitialized = true;
    }

    /// <summary>
    /// Manages the teleporter ui for the tutorial mode.
    /// </summary>
    private void ShowTutorialTeleporterUi()
    {
        List<TeleporterData> dataList = DataManager.Instance.GetUnlockedTeleportersInWorld(0);

        if (dataList.Count <= 1)
        {
            // only one teleporter spot unlocked => find another one
            StartCoroutine(TutorialManager.Instance.LoadNextScreen(0));
        }
        else
        {
            GameObject newToggle = GameObject.Instantiate(prototypeToggle, worldSelectionContent);
            newToggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetWorldName(0);
            Image image = newToggle.GetComponent<Image>();
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => ToggleEnabledColor(b, image));
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => UpdateTeleporterSelections(0, dataList));
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => HandleClick(newToggle));
            newToggle.SetActive(true);
            AddAudioSource(newToggle);
        }
        isMenuInitialized = true;
    }


    /// <summary>
    /// Updates the right side of the menu. This contains the buttons corresponding to the target teleporters.
    /// </summary>
    /// <param name="worldID"></param>
    public void UpdateTeleporterSelections(int worldID, List<TeleporterData> dataList)
    {
        foreach (GameObject button in currentTeleporterButtons)
        {
            Destroy(button);
        }
        currentTeleporterButtons.Clear();
        foreach (TeleporterData data in dataList)
        {
            if (data.teleporterNumber == correspondingTeleporter.teleporterNumber && data.worldID == correspondingTeleporter.worldID && data.dungeonID == correspondingTeleporter.dungeonID)
            {
                continue;
            }
            GameObject newButton = GameObject.Instantiate(prototypeButton.gameObject, teleporterSelectionContent);
            newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.teleporterName;
            newButton.GetComponent<Button>().onClick.AddListener(() => correspondingTeleporter.TeleportPlayerTo(data.position, data.worldID, data.dungeonID));
            newButton.GetComponent<Button>().onClick.AddListener(() => HandleClick(newButton));
            newButton.SetActive(true);
            currentTeleporterButtons.Add(newButton);
            AddAudioSource(newButton);
        }
    }

    /// <summary>
    /// Returns the static world name of the given world id.
    /// </summary>
    /// <param name="worldID"></param>
    /// <returns></returns>
    private string GetWorldName(int worldID)
    {
        string worldName = DataManager.Instance.GetWorldData(worldID).staticName;
        if (worldName.Equals(""))
        {
            worldName = "World " + worldID;
        }
        return worldName;
    }

    public void ToggleEnabledColor(bool enabled, Image image)
    {
        image.color = enabled ? Color.green : standardButtonColor;
    }

    /// <summary>
    /// Adds audio source to the given object.
    /// </summary>
    /// <param name="gameObject"></param>
    private void AddAudioSource(GameObject gameObject)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Handles the game object click event. Plays a click sound if it`s not the first interaction.
    /// </summary>
    /// <param name="gameObject">The game object that was clicked</param>
    private void HandleClick(GameObject gameObject)
    {
        if (isFirstInteraction)
        {
            isFirstInteraction = false;
        }
        else
        {
            PlayClickSound(gameObject);
        }
    }

    /// <summary>
    /// Plays click sound. This function calls when button is clicked.
    /// </summary>
    /// <param name="gameObject"></param>
    private void PlayClickSound(GameObject gameObject)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedButton = GetClickedButton();
            if (clickedButton != null)
            {
                HandleClick(clickedButton);
            }
        }
    }

    private GameObject GetClickedButton()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult result in results)
        {
            if (currentTeleporterButtons.Contains(result.gameObject))
            {
                return result.gameObject;
            }
        }
        return null;
    }
}
