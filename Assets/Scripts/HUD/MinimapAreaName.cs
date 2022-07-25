using UnityEngine;

/* 
 * This script manages that the minimap area name string is set correctily.
 * Each area has a collider and if the player enters the area and therefor also the collider,
 * this scripts updates the static string attribute in the 'MinimapScript' which is represented at the minimap.
 */
public class MinimapAreaName : MonoBehaviour
{
    public string areaName;

    /*
     * This function updates the static string representing the name of the area the player is currently in
     */
    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            MinimapScript.areaName = areaName;
        }
    }
}