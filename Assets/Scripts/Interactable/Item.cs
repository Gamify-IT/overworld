using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Item : MonoBehaviour
{
    public GameObject gameObjectToDestroy;
    // Update is called once per frame
    private void Test()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact()
    {
        Destroy(gameObjectToDestroy);
    }
}
