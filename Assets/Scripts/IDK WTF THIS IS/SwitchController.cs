using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public Sprite unpressedSprite;
    public Sprite pressedSprite;
    private EventSystemExampleEvents events;
    private void Start()
    {
        events = DependencyProvider.instance.eventSystemExampleDependencies.events;

        events.onEnteringSwitch += OnEnteringSwitch;
        events.onLeavingSwitch += OnLeavingSwitch;
        Debug.Log("Var");
    }
    private void OnDestroy()
    {

        events.onEnteringSwitch -= OnEnteringSwitch;
        events.onLeavingSwitch -= OnLeavingSwitch;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Foo");
        if (other.transform.tag == "Player")
        {
            Debug.Log("OnCollisionEnter");
            events.EnteringSwitch();
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("OnCollisionExit");
            events.LeavingSwitch();
        }
    }
    private void OnEnteringSwitch()
    {
        GetComponent<SpriteRenderer>().sprite = pressedSprite;
    }
    private void OnLeavingSwitch()
    {
        GetComponent<SpriteRenderer>().sprite = unpressedSprite;
    }
}