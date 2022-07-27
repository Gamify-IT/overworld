using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This scripts defines the barrier data that needs to be transfered from the game manager to a barrier object.
 */
public class BarrierData
{
    #region Attributes
    private bool isActive;
    #endregion

    public BarrierData(bool isActive)
    {
        this.isActive = isActive;
    }

    #region GetterAndSetter
    public bool getIsActive()
    {
        return isActive;
    }

    public void setIsActive(bool isActive)
    {
        this.isActive = isActive;
    }
    #endregion
}
