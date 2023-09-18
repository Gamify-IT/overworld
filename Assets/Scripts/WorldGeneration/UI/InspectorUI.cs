using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUI : MonoBehaviour
{
    #region Attributes
    InspectorUIManager uiManager;

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

    public void Setup(InspectorUIManager uiManager)
    {
        this.uiManager = uiManager;

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
}
