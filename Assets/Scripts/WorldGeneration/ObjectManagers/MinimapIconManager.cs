using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinimapIconType
{
    WORLD,
    DUNGEON,
    NPC
}

public class MinimapIconManager : MonoBehaviour
{
    [SerializeField] private GameObject minimapIconPrefab;

    [SerializeField] private Sprite worldSprite;
    [SerializeField] private Sprite dungeonSprite;
    [SerializeField] private Sprite npcSprite;

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
    ///     This function creates a minimap icon and sets it up
    /// </summary>
    /// <param name="type">The type of the minimap icon</param>
    /// <param name="position">The position of the minimap icon</param>
    public void AddMinimapIcon(MinimapIconType type, Vector3 position)
    {
        GameObject minimapIcon = Instantiate(minimapIconPrefab, position, Quaternion.identity, this.transform) as GameObject;

        SpriteRenderer spriteRenderer = minimapIcon.GetComponent<SpriteRenderer>();

        if(spriteRenderer != null)
        {
            switch (type)
            {
                case MinimapIconType.WORLD:
                    spriteRenderer.sprite = worldSprite;
                    break;

                case MinimapIconType.DUNGEON:
                    spriteRenderer.sprite = dungeonSprite;
                    break;

                case MinimapIconType.NPC:
                    spriteRenderer.sprite = npcSprite;
                    break;
            }
        }
        else
        {
            Debug.LogError("Error creating minimap icon - SpriteRenderer not found");
        }                   
    }
}
