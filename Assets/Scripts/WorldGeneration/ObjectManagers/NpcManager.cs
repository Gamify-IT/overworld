using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the creation and setup of npc spot at world loading
/// </summary>
public class NpcManager : MonoBehaviour
{
    [SerializeField] private GameObject npcSpotPrefab;
    [SerializeField] private GameObject placeholderPrefab;
    [SerializeField] private GameObject minimapIcons;
    [SerializeField] private List<Sprite> sprites;

    /// <summary>
    ///     This function sets up npc objects for the data given
    /// </summary>
    /// <param name="npcSpots">The data needed for the npcs</param>
    public void Setup(List<NpcSpotData> npcSpots)
    {
        ClearNpcSpots();
        MinimapIconManager minimapIconManager = minimapIcons.GetComponent<MinimapIconManager>();
        if (minimapIconManager != null)
        {
            minimapIconManager.ClearMinimapIconsOfType(MinimapIconType.NPC);
        }
        else
        {
            Debug.LogError("Error creating npc minimap icon - MinimapIconManager not found");
        }
        foreach (NpcSpotData npcSpotData in npcSpots)
        {
            CreateNpcSpot(npcSpotData);
        }
    }

    /// <summary>
    ///     This function sets up placeholder npc objects for the data given
    /// </summary>
    /// <param name="npcSpots">The data needed for the npcs</param>
    public void SetupPlaceholder(List<NpcSpotData> npcSpots)
    {
        ClearNpcSpots();
        foreach (NpcSpotData npcSpotData in npcSpots)
        {
            CreatePlaceholderNpcSpot(npcSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing npc objects of the given area
    /// </summary>
    private void ClearNpcSpots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates an npc spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the npc spot</param>
    private void CreateNpcSpot(NpcSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject npcSpot = Instantiate(npcSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;
        
        SpriteRenderer spriteRenderer = npcSpot.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            Optional<Sprite> sprite = GetSprite(data.GetSpriteName());
            if (sprite.IsPresent())
            {
                spriteRenderer.sprite = sprite.Value();
            }
            else
            {
                Debug.LogError("Could not find sprite: " + data.GetSpriteName());
            }
        }
        else
        {
            Debug.LogError("Error creating npc - SpriteRenderer not found");
        }

        NPC npc = npcSpot.GetComponent<NPC>();
        if(npc != null)
        {
            npc.SetWorldIndex(data.GetArea().GetWorldIndex());
            if (data.GetArea().IsDungeon())
            {
                npc.SetDungeonIndex(data.GetArea().GetDungeonIndex());                
            }
            else
            {
                npc.SetDungeonIndex(0);
            }
            npc.SetIndex(data.GetIndex());
            npc.SetName(data.GetName());
            Optional<Sprite> icon = GetSprite(data.GetIconName());
            if(icon.IsPresent())
            {
                npc.SetSprite(icon.Value());
            }
            else
            {
                Debug.LogError("Could not find sprite: " + data.GetIconName());
            }
        }
        else
        {
            Debug.LogError("Error creating npc - Script not found");
        }

        MinimapIconManager minimapIconManager = minimapIcons.GetComponent<MinimapIconManager>();
        if (minimapIconManager != null)
        {
            minimapIconManager.AddMinimapIcon(MinimapIconType.NPC, position);
        }
        else
        {
            Debug.LogError("Error creating npc minimap icon - MinimapIconManager not found");
        }
    }

    /// <summary>
    ///     This function returns the sprite with the given name
    /// </summary>
    /// <param name="spriteName">The sprite to look for</param>
    /// <returns>An <c>Optional</c> containing the sprite, if present</returns>
    private Optional<Sprite> GetSprite(string spriteName)
    {
        Optional<Sprite> sprite = new Optional<Sprite>();
        Sprite[] spritesArray = sprites.ToArray();
        for (int i=0; i<spritesArray.Length; i++)
        {
            if(spritesArray[i].name.Equals(spriteName))
            {
                sprite.SetValue(spritesArray[i]);
                break;
            }
        }
        return sprite;
    }

    /// <summary>
    ///     This function creates a placeholder npc spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the npc spot</param>
    private void CreatePlaceholderNpcSpot(NpcSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject placeholderSpot = Instantiate(placeholderPrefab, position, Quaternion.identity, this.transform) as GameObject;
        PlaceholderObject placeholder = placeholderSpot.GetComponent<PlaceholderObject>();
        if (placeholder != null)
        {
            placeholder.Setup(PlaceholderType.NPC);
        }
        else
        {
            Debug.LogError("Error creating placeholder npc spot");
        }
    }
}
