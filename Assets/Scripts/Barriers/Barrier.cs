using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// This enum defines, whether a barrier blocks access towards a dungeon or a world
/// </summary>
public enum BarrierType
{
    worldBarrier,
    dungeonBarrier
}

/// <summary>
/// A <c>Barrier</c> is used to block of access to worlds and dungeons.
/// </summary>
public class Barrier : MonoBehaviour
{
    #region Attributes
    [SerializeField] private BarrierType type;
    [SerializeField] private bool isActive;
    [SerializeField] private int originWorldIndex;
    [SerializeField] private int destinationAreaIndex;
    #endregion

    #region Setup
    /// <summary>
    /// The Awake function is called when the object is initialized and sets up the starting values and state of the object.
    /// The function registers the barrier at the game manager and sets the barrier to be active on default.
    /// </summary>
    void Awake()
    {
        registerToGameManager();
        updateStatus();
    }

    /// <summary>
    /// The OnDestroy function is called when the object is deleted.
    /// The function removes the barrier from the game manager.
    /// </summary>
    private void OnDestroy()
    {
        Debug.Log("remove " + type + ": " + originWorldIndex + "->" + destinationAreaIndex);
        GameManager.instance.removeBarrier(type, originWorldIndex, destinationAreaIndex);
    }

    /// <summary>
    /// This function registers the barrier at the game manager.
    /// </summary>
    private void registerToGameManager()
    {
        Debug.Log("register " + type + ": " + originWorldIndex + "->" + destinationAreaIndex);
        GameManager.instance.addBarrier(this.gameObject, type, originWorldIndex, destinationAreaIndex);
    }
    #endregion

    #region Functionality
    /// <summary>
    /// This function sets up the barrier object with the provided data
    /// </summary>
    /// <param name="data">The data to be set</param>
    public void setup(BarrierData data)
    {
        Debug.Log(type + ": " + originWorldIndex + "->" + destinationAreaIndex + ": new status: " + data.getIsActive());
        isActive = data.getIsActive();
        updateStatus();
    }

    /// <summary>
    /// This function updates whether the barrier is active or not, based on the stored data
    /// </summary>
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

    /// <summary>
    /// This function returns information about the barrier object
    /// </summary>
    /// <returns>A string containing all necessary information</returns>
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

    /// <summary>
    /// This function is called when the player gets near the barrier object.
    /// The function opens the info screen and showcases information about the barrier.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string infoText = GameManager.instance.getBarrierInfoText(type, originWorldIndex, destinationAreaIndex);
            openInfoPanel(infoText);
        }
    }

    /// <summary>
    /// This function is called when the player moves away from the barrier object.
    /// The function closes the info screen.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            closeInfoPanel();
        }
    }

    /// <summary>
    /// This function opens the info screen and displays the given text.
    /// </summary>
    /// <param name="infoText">The text to display</param>
    /// <returns></returns>
    private async UniTask openInfoPanel(string infoText)
    {
        await SceneManager.LoadSceneAsync("InfoScreen", LoadSceneMode.Additive);
        if(InfoManager.instance == null)
        {
            return;
        }
        InfoManager.instance.displayInfo(infoText);
    }

    /// <summary>
    /// This function closes the info screen.
    /// </summary>
    /// <returns></returns>
    private async UniTask closeInfoPanel()
    {
        if(InfoManager.instance != null)
        {
            InfoManager.instance.closeButtonPressed();
        }
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
