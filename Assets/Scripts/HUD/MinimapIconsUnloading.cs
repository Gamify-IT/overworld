using UnityEngine;

/*
 * This script manages, which icons on the minimap must be active and which must not
 * At every connection between two worlds, there are icons showing an image representing the other world,
 * to help player navigate between different worlds. Once the player has passed the icon on the minimap,
 * it disappears, because if it wouldn't, it would be misleading. 
 */
public class MinimapIconsUnloading : MonoBehaviour
{
    public string nameOfCurrentScene;

    /*
     * This function is triggered once the player enteres a world or the area of the minimap icon.
     * It activates all minimap icons in that world. 
     * If the collider is a minimap icon, it disappears from the minimap. 
     */
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            GameObject[] minimapIcons = GameObject.FindGameObjectsWithTag("MinimapIcon");

            foreach (GameObject minimapIcon in minimapIcons)
            {
                if (!minimapIcon.name.Contains(nameOfCurrentScene) && !nameOfCurrentScene.Equals(""))
                {
                    minimapIcon.GetComponent<SpriteRenderer>().enabled = true;
                    ;
                }
            }

            if (nameOfCurrentScene.Equals(""))
            {
                this.transform.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    /*
     * This function is triggered once the player leaves a world or the area of the minimap icon.
     * It deactivates all minimap icons in that world. 
     * If the collider is a minimap icon, it disappears from the minimap. 
     */
    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            GameObject[] minimapIcons = GameObject.FindGameObjectsWithTag("MinimapIcon");

            foreach (GameObject minimapIcon in minimapIcons)
            {
                if (minimapIcon.name.Contains(nameOfCurrentScene) && !nameOfCurrentScene.Equals(""))
                {
                    minimapIcon.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            if (nameOfCurrentScene.Equals(""))
            {
                this.transform.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}