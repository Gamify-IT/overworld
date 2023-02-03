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

    private Teleporter correspondingTeleporter;
    private Color standardButtonColor = new Color(1, 0.9176471f, 0.7960784f);


    /// <summary>
    /// Sets up the buttons according to the available data.
    /// </summary>
    /// <param name="teleporter"></param>
    public void SetupUI(Teleporter teleporter)
    {
        correspondingTeleporter = teleporter;

        for (int i = 1; i < GameSettings.GetMaxWorlds(); i++)
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
            newToggle.SetActive(true);
        }
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
            newButton.SetActive(true);
            currentTeleporterButtons.Add(newButton);
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
}
