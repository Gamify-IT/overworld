using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This enum defines, whether a barrier blocks access towards a dungeon or a world
/// </summary>
public enum BarrierType
{
    worldBarrier,
    dungeonBarrier
}

/// <summary>
///     A <c>Barrier</c> is used to block of access to worlds and dungeons.
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
    ///     The OnDestroy function is called when the object is deleted.
    ///     The function removes the barrier from the game manager.
    /// </summary>
    private void OnDestroy()
    {
        if (GameSettings.GetGamemode() == Gamemode.PLAY)
        {
            Debug.Log("remove " + type + ": " + originWorldIndex + "->" + destinationAreaIndex);
            ObjectManager.Instance.RemoveBarrier(type, originWorldIndex, destinationAreaIndex);
        }            
    }

    /// <summary>
    ///     This function initializes the <c>Barrier</c> object
    /// </summary>
    /// <param name="worldOriginIndex">The world the <c>Barrier</c> is in</param>
    /// <param name="areaDestinationIndex">The index of the area the <c>Barrier</c> is blocking access to</param>
    /// <param name="type"></param>
    public void Initialize(int originWorldIndex, int destinationAreaIndex, BarrierType type)
    {
        this.originWorldIndex = originWorldIndex;
        this.destinationAreaIndex = destinationAreaIndex;
        this.type = type;

        RegisterToGameManager();
    }

    /// <summary>
    ///     This function registers the barrier at the game manager.
    /// </summary>
    private void RegisterToGameManager()
    {
        Debug.Log("register " + type + ": " + originWorldIndex + "->" + destinationAreaIndex);
        ObjectManager.Instance.AddBarrier(gameObject, type, originWorldIndex, destinationAreaIndex);
    }

    #endregion

    #region Functionality

    /// <summary>
    ///     This function sets up the barrier object with the provided data
    /// </summary>
    /// <param name="data">The data to be set</param>
    public void Setup(BarrierData data)
    {
        Debug.Log(type + ": " + originWorldIndex + "->" + destinationAreaIndex + ": new status: " + data.IsActive());
        isActive = data.IsActive();
        UpdateStatus();
    }

    /// <summary>
    ///     This function updates whether the barrier is active or not, based on the stored data
    /// </summary>
    private void UpdateStatus()
    {
        if (isActive)
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
    ///     This function returns information about the barrier object
    /// </summary>
    /// <returns>A string containing all necessary information</returns>
    public string GetInfo()
    {
        string info = "";
        switch (type)
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
    ///     This function is called when the player gets near the barrier object.
    ///     The function opens the info screen and showcases information about the barrier.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string infoText = GameManager.Instance.GetBarrierInfoText(type, originWorldIndex, destinationAreaIndex);
            CloseInfoPanel(); //so if a info panel is open it will be closed before the new one opens
            OpenInfoPanel(infoText);
        }
    }

    /// <summary>
    ///     This function is called when the player moves away from the barrier object.
    ///     The function closes the info screen.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CloseInfoPanel();
        }
    }

    /// <summary>
    ///     This function opens the info screen and displays the given text.
    /// </summary>
    /// <param name="infoText">The text to display</param>
    /// <returns></returns>
    private async UniTask OpenInfoPanel(string infoText)
    {
        await SceneManager.LoadSceneAsync("InfoScreen", LoadSceneMode.Additive);
        if (InfoManager.Instance == null)
        {
            return;
        }

        string headerText = "";
        switch (type)
        {
            case BarrierType.worldBarrier:
                headerText = "World " + destinationAreaIndex;
                break;

            case BarrierType.dungeonBarrier:
                headerText = "Dungeon " + originWorldIndex + "-" + destinationAreaIndex;
                break;
        }

        InfoManager.Instance.DisplayInfo(headerText, infoText);
    }

    /// <summary>
    ///     This function closes the info screen.
    /// </summary>
    /// <returns></returns>
    private void CloseInfoPanel()
    {
        if (InfoManager.Instance != null)
        {
            InfoManager.Instance.CloseButtonPressed();
        }
    }

    #endregion

    #region Getter

    public int GetWorldOriginIndex()
    {
        return originWorldIndex;
    }

    public void SetWorldOriginIndex(int worldOriginIndex)
    {
        originWorldIndex = worldOriginIndex;
    }

    public int GetWorldDestinationIndex()
    {
        return destinationAreaIndex;
    }

    public void SetDestionationAreaIndex(int destinationAreaIndex)
    {
        this.destinationAreaIndex = destinationAreaIndex;
    }

    public void  SetBarrierType(BarrierType type)
    {
        this.type = type;
    }

    #endregion
}