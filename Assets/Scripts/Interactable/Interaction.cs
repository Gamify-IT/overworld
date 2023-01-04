using UnityEngine;

public class Interaction : MonoBehaviour

{
    public Transform detectionPoint;
    private const float detectionRadius = 0.5f;
    public LayerMask detectionLayer;
    public GameObject detectedObject;

    //KeyCodes
    private KeyCode interact;

    private void Start()
    {
        interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;
    }

    /// <summary>
    /// The <c>Update</c> function is called once every frame.
    /// This function checks, if a interactable object is found and the player wants to interact with it, and if so, starts the interaction.
    /// </summary>
    void Update()
    {
        if (DetectObject() && !PauseMenu.menuOpen && InteractInput())
        {
            Debug.Log("Interact");
            detectedObject.GetComponent<Item>().Interact();
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    /// <summary>
    /// This function checks, if the <c>E</c> button is pressed or not
    /// </summary>
    /// <returns>True, if <c>E</c> button pressed, false otherwise</returns>
    bool InteractInput()
    {
        return Input.GetKeyDown(interact);
    }

    /// <summary>
    /// This function checks, if a object is nearby.
    /// </summary>
    /// <returns>True, if a object was found, false otherwise</returns>
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

    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.INTERACT)
        {
            interact = GameManager.Instance.GetKeyCode(Binding.INTERACT);
        }
    }
}