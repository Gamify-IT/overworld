using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private List<GameObject> currentTeleporterButtons = new List<GameObject>();
    private Dictionary<int, List<Teleporter>> registeredTeleporters;

    private Teleporter correspondingTeleporter;


    /// <summary>
    /// Sets up the buttons according to the available data.
    /// </summary>
    /// <param name="teleporter"></param>
    public void SetupUI(Teleporter teleporter)
    {
        correspondingTeleporter = teleporter;
        registeredTeleporters = DataManager.Instance.registeredTeleporters;
        List<int> destinationWorlds = new List<int>(registeredTeleporters.Keys);
        if (registeredTeleporters[teleporter.worldID].Count == 1)
        {
            destinationWorlds.Remove(teleporter.worldID);
        }
        foreach (int worldID in destinationWorlds)
        {
            GameObject newToggle = GameObject.Instantiate(prototypeToggle, worldSelectionContent);
            newToggle.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "World " + worldID.ToString();
            Image image = newToggle.GetComponent<Image>();
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => ToggleEnabledColor(b, image));
            newToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => UpdateTeleporterSelections(worldID));
            newToggle.SetActive(true);
        }
    }


    /// <summary>
    /// Updates the right side of the menu. This contains the buttons corresponding to the target teleporters.
    /// </summary>
    /// <param name="worldID"></param>
    public void UpdateTeleporterSelections(int worldID)
    {
        foreach (GameObject button in currentTeleporterButtons)
        {
            Destroy(button);
        }
        currentTeleporterButtons.Clear();
        foreach (Teleporter teleporter in registeredTeleporters[worldID])
        {
            if (worldID == correspondingTeleporter.worldID && teleporter.Equals(correspondingTeleporter))
            {
                continue;
            }
            GameObject newButton = GameObject.Instantiate(prototypeButton.gameObject, teleporterSelectionContent);
            newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = teleporter.teleporterName;
            newButton.GetComponent<Button>().onClick.AddListener(() => correspondingTeleporter.TeleportPlayerTo(teleporter.transform.position,teleporter.worldID,teleporter.dungeonID));
            newButton.SetActive(true);
            currentTeleporterButtons.Add(newButton);
        }
    }

    public void ToggleEnabledColor(bool enabled, Image image)
    {
        image.color = enabled ? Color.green : Color.white;
    }
}
