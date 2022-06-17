using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour

    {
    
    public Transform detectionPoint;

    private const float detectionRadius = 0.5f;

    public LayerMask detectionLayer;

    public GameObject detectedObject;
    void Update()
    {
        if(DetectObject())
        {
            if(InteractInput())
            {
                Debug.Log("Interact");
                detectedObject.GetComponent<Item>().Interact();
            }
        }
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject()
    {
        Collider2D obj =  Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }
}
