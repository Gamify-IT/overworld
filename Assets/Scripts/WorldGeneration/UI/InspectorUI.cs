using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InspectorUI : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    #region Attributes
    InspectorUIManager uiManager;

    //Area
    [SerializeField] private TextMeshProUGUI areaText;

    //Panels
    [SerializeField] private GameObject inspectorPanel;
    [SerializeField] private GameObject smallInspectorPanel;

    //UI
    [SerializeField] private Toggle minigamesToggle;
    [SerializeField] private Toggle npcsToggle;
    [SerializeField] private Toggle booksToggle;
    [SerializeField] private Toggle teleportersToggle;
    [SerializeField] private Toggle dungeonsToggle;
    [SerializeField] private Toggle barriersToggle;
    #endregion

    public void Setup(InspectorUIManager uiManager, AreaInformation area)
    {
        this.uiManager = uiManager;

        if (area.IsDungeon())
        {
            areaText.text = "DUNGEON " + area.GetWorldIndex() + "-" + area.GetDungeonIndex();
        }
        else
        {
            areaText.text = "WORLD " + area.GetWorldIndex();
        }

        minigamesToggle.isOn = true;
        npcsToggle.isOn = true;
        booksToggle.isOn = true;
        teleportersToggle.isOn = true;
        dungeonsToggle.isOn = true;
    }

    #region Object Toggling
    public void ToggleMinigames()
    {
        uiManager.SetMinigames(minigamesToggle.isOn);
    }

    public void ToggleNpcs()
    {
        uiManager.SetNpcs(npcsToggle.isOn);
    }

    public void ToogleBooks()
    {
        uiManager.SetBooks(booksToggle.isOn);
    }

    public void ToggleTeleporters()
    {
        uiManager.SetTeleporters(teleportersToggle.isOn);
    }

    public void ToggleDungeons()
    {
        uiManager.SetDungeons(dungeonsToggle.isOn);
    }

    public void ToggleBarriers()
    {
        uiManager.SetBarriers(barriersToggle.isOn);
    }
    #endregion

    #region Minimize / Maximize
    public void MinimizeButtonPressed()
    {
        inspectorPanel.SetActive(false);
        smallInspectorPanel.SetActive(true);
        uiManager.ActivateCameraMovement();
    }

    public void MaximizeButtonPressed()
    {
        smallInspectorPanel.SetActive(false);
        inspectorPanel.SetActive(true);
        uiManager.DeactivateCameraMovement();
    }
    #endregion

    public void QuitButtonPressed()
    {
        CloseOverworld();
    }
}
