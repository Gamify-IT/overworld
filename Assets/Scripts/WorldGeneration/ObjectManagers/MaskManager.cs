using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the masking of worlds
/// </summary>
public class MaskManager : MonoBehaviour
{
    [SerializeField] private GameObject maskPrefab;

    /// <summary>
    ///     This function creates masks for the given worlds
    /// </summary>
    /// <param name="worlds">A List of all worlds</param>
    public void Setup(List<WorldMapData> worlds)
    {
        foreach(WorldMapData world in worlds)
        {
            float positionX = 0.5f * world.GetSize().x + world.GetOffset().x;
            float positionY = 0.5f * world.GetSize().y + world.GetOffset().y;
            Vector3 position = new Vector3(positionX, positionY, 0);
            GameObject mask = Instantiate(maskPrefab, position, Quaternion.identity, this.transform) as GameObject;

            WorldMask worldMask = mask.GetComponent<WorldMask>();
            if (worldMask != null)
            {
                worldMask.SetArea(world.GetArea());
            }
            else
            {
                Debug.LogError("Error creating mask - WorldMask not found");
            }

            SpriteRenderer sprite = mask.GetComponent<SpriteRenderer>();
            if(sprite != null)
            {
                sprite.size = new Vector2(world.GetSize().x, world.GetSize().y);
            }
            else
            {
                Debug.LogError("Error creating mask - SpriteRenderer not found");
            }
        }
    }

    /// <summary>
    ///     This function removes the mask for the given world
    /// </summary>
    /// <param name="world">The world to be removed</param>
    public void RemoveMask(AreaInformation world)
    {
        foreach (Transform child in transform)
        {
            WorldMask worldMask = child.GetComponent<WorldMask>();
            if (worldMask != null)
            {
                if (worldMask.GetArea().GetWorldIndex() == world.GetWorldIndex())
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    ///     This function removes all masks
    /// </summary>
    public void RemoveAllMasks()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function activates the mask for the given world
    /// </summary>
    /// <param name="world">The world which mask should be activated</param>
    public void ActivateMask(AreaInformation world)
    {
        foreach (Transform child in transform)
        {
            WorldMask worldMask = child.GetComponent<WorldMask>();
            if (worldMask != null)
            {
                if (worldMask.GetArea().GetWorldIndex() == world.GetWorldIndex())
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    ///     This function deactivates the mask for the given world
    /// </summary>
    /// <param name="world">The world which mask should be deactivated</param>
    public void DeactivateMask(AreaInformation world)
    {
        foreach (Transform child in transform)
        {
            WorldMask worldMask = child.GetComponent<WorldMask>();
            if (worldMask != null)
            {
                if(worldMask.GetArea().GetWorldIndex() == world.GetWorldIndex())
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }
}
