using UnityEngine;

public class TutorialDungeon : MonoBehaviour
{
    private static bool isTriggered = false;
    private Collider2D trigger;

    private void Start()
    {
        trigger = GetComponent<Collider2D>();
        InitializeTrigger();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            trigger.enabled = false;
            isTriggered = true;
        }
    }

    /// <summary>
    ///     Activates the trigger in the dungeon only for the first time.
    /// </summary>
    private void InitializeTrigger()
    {
        if (!isTriggered)
        {
            trigger.enabled = true;
        }
    }
}
