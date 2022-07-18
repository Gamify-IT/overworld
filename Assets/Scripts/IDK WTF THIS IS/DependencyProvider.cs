using UnityEngine;

public class DependencyProvider : MonoBehaviour
{
    public static DependencyProvider instance { get { { return FindObjectOfType<DependencyProvider>(); } } }

    private EventSystemExampleDependencies _eventSystemExampleDependencies = new EventSystemExampleDependencies();
    public EventSystemExampleDependencies eventSystemExampleDependencies { get { return _eventSystemExampleDependencies; } }
}
