using System;

public class EventSystemExampleEvents
{
    public event Action onEnteringSwitch;
    public void EnteringSwitch()
    {
        onEnteringSwitch();
    }

    public event Action onLeavingSwitch;
    public void LeavingSwitch()
    {
        onLeavingSwitch();
    }
}
