using UnityEngine;

/// <summary>
///     This class manages the interaction of objects.
/// </summary>
public class InteractionOfObject : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
}