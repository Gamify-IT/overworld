using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionOfObject : MonoBehaviour
{
    public GameObject gameObject;
    // Update is called once per frame
    void Update()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
	}
}
