using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    [SerializeField] private GameObject keybindingPrefab;
    [SerializeField] private GameObject content;

    private List<Keybinding> keybindings;

    private void Start()
    {
        keybindings = GameManager.Instance.GetKeybindings();
        DisplayKeybinding();
    }

    /// <summary>
    ///     This function displays all keybindings
    /// </summary>
    private void DisplayKeybinding()
    {
        foreach (Keybinding keybinding in keybindings)
        {
            DisplayKeybinding(keybinding);
        }
    }

    /// <summary>
    ///     This function creates a GameObject for the given <c>Keybinding</c>
    /// </summary>
    /// <param name="keybinding">The keybinding a GameObject should be created for</param>
    private void DisplayKeybinding(Keybinding keybinding)
    {
        GameObject keybindingObject = Instantiate(keybindingPrefab, content.transform, false);

        KeyBindingUIElement keyBindingUIElement = keybindingObject.GetComponent<KeyBindingUIElement>();
        if(keyBindingUIElement != null)
        {
            keyBindingUIElement.Setup(keybinding);            
        }
        else
        {
            Destroy(keybindingObject);
        }
    }
}
