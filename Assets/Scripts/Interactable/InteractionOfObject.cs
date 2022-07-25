using UnityEngine;

/*
 * This script manages to status of collider script on the same object. 
 */
public class InteractionOfObject : MonoBehaviour
{
    /*
     * The Update function is called every frame. It updates all values according to the changes happened since the last frame. 
     * It sets the collider of the object to be a trigger. 
     */
    void Update()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
}