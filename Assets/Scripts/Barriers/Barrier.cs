using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script defines a barrier to block access to other worlds.  
 */
public class Barrier : MonoBehaviour
{
    #region Attributes
    [SerializeField] private bool isActive;
    [SerializeField] private int worldIndexOrigin;
    [SerializeField] private int worldIndexDestination;
    #endregion

    #region Setup
    /*
     * The Start function is called when the object is initialized and sets up the starting values and state of the object. 
     * The function registers the barrier at the game manager and sets the barrier to be active on default.
     */
    void Start()
    {
        registerToGameManager();
        updateStatus();
    }

    /*
     * This function registers the barrier at the game manager
     */
    private void registerToGameManager()
    {
        Debug.Log("register Barrier " + worldIndexOrigin + " -> " + worldIndexDestination);
        GameManager.instance.addBarrier(this.gameObject, worldIndexOrigin, worldIndexDestination);
    }
    #endregion

    #region Functionality
    /*
     * This functions configurates the barrier with the given data and updates the object.
     * @param data: the data to be set
     */
    public void setup(BarrierData data)
    {
        isActive = data.getIsActive();
        updateStatus();
    }

    /*
     * This function updates the object status.
     */
    private void updateStatus()
    {
        if(isActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}
