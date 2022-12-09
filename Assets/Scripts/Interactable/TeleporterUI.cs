using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class is used to save the available buttons of the TeleporterUI
/// </summary>
public class TeleporterUI : MonoBehaviour
{
    [SerializeField]
    private Button selectionButton;
    [SerializeField]
    private Transform worldSelectionContent;
    [SerializeField]
    private Transform teleporterSelectionContent;

    private GameObject[] currentTeleporterButtons;
    private Dictionary<int, List<Teleporter>> targetTeleporters;

    private Teleporter correspondingTeleporter;
    public void SetupUI(Teleporter teleporter)
    {
        correspondingTeleporter = teleporter;
        targetTeleporters = teleporter.getTargetTeleporters();
        List<int> destinationWorlds = new List<int>(targetTeleporters.Keys);
        foreach (int worldID in destinationWorlds)
        {
            GameObject newButton = GameObject.Instantiate(selectionButton.gameObject, worldSelectionContent);
            newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "World " + worldID.ToString();
            newButton.GetComponent<Button>().onClick.AddListener(() => UpdateTeleporterSelections(worldID));
            newButton.SetActive(true);
        }
    }

    public void UpdateTeleporterSelections(int worldID)
    {
        foreach (GameObject button in currentTeleporterButtons)
        {
            Destroy(button);
        }
        foreach (Teleporter teleporter in targetTeleporters[worldID])
        {
            GameObject newButton = GameObject.Instantiate(selectionButton.gameObject, teleporterSelectionContent);
            newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = teleporter.teleporterName;
            newButton.GetComponent<Button>().onClick.AddListener(() => correspondingTeleporter.TeleportPlayerTo(teleporter.transform.position,teleporter.worldID,teleporter.dungeonID));
            newButton.SetActive(true);
        }
    }
}
