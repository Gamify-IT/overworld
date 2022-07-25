using UnityEngine;

/*
 * This script manages interactable objects. 
 * If the player is close enough to another interactable object and presses the 'E' button, the object starts the interaction. 
 */
public class Interaction : MonoBehaviour
{
    public Transform detectionPoint;
    private const float detectionRadius = 0.5f;
    public LayerMask detectionLayer;
    public GameObject detectedObject;

    /*
     * The Update function is called every frame. It updates all values according to the changes happened since the last frame. 
     * It checks if a object is close enough and the player wants to interact with it.
     * If so, it starts the interaction.
     */
    void Update()
    {
        if (DetectObject())
        {
            if (InteractInput())
            {
                detectedObject.GetComponent<Item>().Interact();
            }
        }
    }

    /*
     * This function return whether the interaction button 'E' is pressed or not. 
     * @return: true, if button is pressed, false otherwise
     */
    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    /*
     * This function checks, if another object is close enough the interact with it.
     * If so, it stores a reference to the other objec. 
     * @return: true, if an object was found, false otherwise
     */
    bool DetectObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
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