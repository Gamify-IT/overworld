using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinimapIconType
{
    UNSET,
    WORLD,
    DUNGEON,
    NPC
}

public class MinimapIconManager : MonoBehaviour
{
    [SerializeField] private GameObject minimapIconPrefab;

    /// <summary>
    ///     This function removes all existing minimap icon objects
    /// </summary>
    public void ClearMinimapIcons()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function removes all minimap icons of the given type
    /// </summary>
    /// <param name="type">The type of minimap icons to remove</param>
    public void ClearMinimapIconsOfType(MinimapIconType type)
    {
        foreach (Transform child in transform)
        {
            MinimapIcon minimapIcon = child.GetComponent<MinimapIcon>();
            if (minimapIcon != null && minimapIcon.GetMinimapIconType() == type)
            {
                Destroy(child.gameObject);
            }            
        }
    }

    /// <summary>
    ///     This function creates a minimap icon and sets it up
    /// </summary>
    /// <param name="type">The type of the minimap icon</param>
    /// <param name="position">The position of the minimap icon</param>
    public void AddMinimapIcon(MinimapIconType type, Vector3 position)
    {
        GameObject minimapIconObject = Instantiate(minimapIconPrefab, position, Quaternion.identity, this.transform) as GameObject;

        MinimapIcon minimapIcon = minimapIconObject.GetComponent<MinimapIcon>();

        if(minimapIcon != null)
        {
            minimapIcon.SetMinimapIconType(type);
        }
        else
        {
            Debug.LogError("Error creating minimap icon - MinimapIcon not found");
        }                   
    }
}
