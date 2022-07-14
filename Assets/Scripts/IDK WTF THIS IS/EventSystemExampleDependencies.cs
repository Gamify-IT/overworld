using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemExampleDependencies
{
    private EventSystemExampleEvents _events = new EventSystemExampleEvents();
    public EventSystemExampleEvents events { get { return _events; } }
}
