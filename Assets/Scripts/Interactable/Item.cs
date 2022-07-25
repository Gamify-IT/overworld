using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
/*
 * This scripts manages a destroyable item. 
 * If an interaction with the item is triggered, it gets destroyed. 
 */
public class Item : MonoBehaviour
{
    public GameObject gameObjectToDestroy;

    /* 
     * This function manages to status of collider script on the same object. 
     * It sets the collider to be a trigger. 
     */
    private void Test()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    /*
     * This functions manages, what happenes when an interaction with the item is triggered.
     * If that happenes, the item gets destroyed. 
     */
    public void Interact()
    {
        Destroy(gameObjectToDestroy);
    }
}