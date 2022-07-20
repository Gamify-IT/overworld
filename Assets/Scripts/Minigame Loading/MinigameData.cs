using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameData
{
    private MinigameStatus status;

    public MinigameData(MinigameStatus status)
    {
        this.status = status;
    }

    public MinigameStatus getStatus()
    {
        return status;
    }

    public void setStatus(MinigameStatus status)
    {
        this.status = status;
    }
}
