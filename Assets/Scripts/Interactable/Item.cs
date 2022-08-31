using UnityEngine;

/// <summary>
///     This class manages the items that player can pick up.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public GameObject gameObjectToDestroy;

    // Update is called once per frame
    private void Test()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    /// <summary>
    ///     This functions destroy the item on interaction.
    /// </summary>
    public void Interact()
    {
        Destroy(gameObjectToDestroy);
    }
}