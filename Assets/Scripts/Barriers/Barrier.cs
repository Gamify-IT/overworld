using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enum defines, whether a barrier blocks access towards a dungeon or a world
/// </summary>
public enum BarrierType
{
    worldBarrier,
    dungeonBarrier
}

/*
 * This script defines a barrier to block access to other worlds.  
 */
public class Barrier : MonoBehaviour
{
    #region Attributes
    [SerializeField] private BarrierType type;
    [SerializeField] private bool isActive;
    [SerializeField] private int originWorldIndex;
    [SerializeField] private int destinationAreaIndex;
    #endregion

    #region Setup
    /*
     * The Start function is called when the object is initialized and sets up the starting values and state of the object. 
     * The function registers the barrier at the game manager and sets the barrier to be active on default.
     */
    void Awake()
    {
        registerToGameManager();
        updateStatus();
    }

    private void OnDestroy()
    {
        Debug.Log("remove " + type + ": " + originWorldIndex + "->" + destinationAreaIndex);
        GameManagerV2.instance.removeBarrier(type, originWorldIndex, destinationAreaIndex);
    }

    /*
     * This function registers the barrier at the game manager
     */
    private void registerToGameManager()
    {
        Debug.Log("register " + type + ": " + originWorldIndex + "->" + destinationAreaIndex);
        GameManagerV2.instance.addBarrier(this.gameObject, type, originWorldIndex, destinationAreaIndex);
    }
    #endregion

    #region Functionality
    /*
     * This functions configurates the barrier with the given data and updates the object.
     * @param data: the data to be set
     */
    public void setup(BarrierData data)
    {
        Debug.Log(type + ": " + originWorldIndex + "->" + destinationAreaIndex + ": new status: " + data.getIsActive());
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
            Debug.Log(type + ": " + originWorldIndex + "->" + destinationAreaIndex + ": now visible");
            gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(type + ": " + originWorldIndex + "->" + destinationAreaIndex + ": now invisible");
            gameObject.SetActive(false);
        }
    }
    
    //returns barrier object info
    public string getInfo()
    {
        string info = "";
        switch(type)
        {
            case BarrierType.worldBarrier:
                info = "world " + originWorldIndex + "-> world " + destinationAreaIndex + ": active: " + isActive;
                break;
            case BarrierType.dungeonBarrier:
                info = "world " + originWorldIndex + "-> dungeon " + destinationAreaIndex + ": active: " + isActive;
                break;
        }
        return info;
    }
    #endregion

    #region Getter
    public int getWorldOriginIndex()
    {
        return originWorldIndex;
    }

    public int getWorldDestinationIndex()
    {
        return destinationAreaIndex;
    }
    #endregion
}
