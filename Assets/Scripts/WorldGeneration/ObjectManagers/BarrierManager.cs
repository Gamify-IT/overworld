using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This classes manages the creation of barrier objects and placeholder icons
/// </summary>
public class BarrierManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject barrierSpotPrefab;
    [SerializeField] private GameObject placeholderPrefab;

    [Space(10)]

    [Header("Barrier Sprites")]
    [SerializeField] private Sprite house;
    [SerializeField] private Sprite trapdoor;
    [SerializeField] private Sprite gate;
    [SerializeField] private Sprite tree;
    [SerializeField] private Sprite ironBarDoor;
    [SerializeField] private Sprite stone;

    private Dictionary<BarrierStyle, Sprite> spriteMapper = new Dictionary<BarrierStyle, Sprite>();

    private void Awake()
    {
        spriteMapper.Add(BarrierStyle.HOUSE, house);
        spriteMapper.Add(BarrierStyle.TRAPDOOR, trapdoor);
        spriteMapper.Add(BarrierStyle.GATE, gate);
        spriteMapper.Add(BarrierStyle.TREE, tree);
        spriteMapper.Add(BarrierStyle.IRON_BAR_DOOR, ironBarDoor);
        spriteMapper.Add(BarrierStyle.STONE, stone);
    }

    /// <summary>
    ///     This function sets up barrier objects for the data given
    /// </summary>
    /// <param name="barrierSpots">The data needed for the barriers</param>
    public void Setup(List<BarrierSpotData> barrierSpots)
    {
        ClearBarrierSpots();
        foreach (BarrierSpotData barrierSpotData in barrierSpots)
        {
            CreateBarrierSpot(barrierSpotData);
        }
    }

    /// <summary>
    ///     This function sets up placeholder barrier objects for the data given
    /// </summary>
    /// <param name="barrierSpots">The data needed for the barriers</param>
    public void SetupPlaceholder(List<BarrierSpotData> barrierSpots)
    {
        ClearBarrierSpots();
        foreach (BarrierSpotData barrierSpotData in barrierSpots)
        {
            CreatePlaceholderBarrierSpot(barrierSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing barrier objects
    /// </summary>
    private void ClearBarrierSpots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates an barrier spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the barrier spot</param>
    private void CreateBarrierSpot(BarrierSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject barrierSpot = Instantiate(barrierSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;
        Barrier barrier = barrierSpot.GetComponent<Barrier>();
        if (barrier != null)
        {
            Sprite sprite = spriteMapper[data.GetBarrierStyle()];
            barrier.Initialize(data.GetArea().GetWorldIndex(), data.GetDestinationAreaIndex(), data.GetBarrierType(), sprite);
        }
        else
        {
            Debug.LogError("Error creating barrier - script not found");
        }
    }

    /// <summary>
    ///     This function creates a placeholder barrier spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the barrier spot</param>
    private void CreatePlaceholderBarrierSpot(BarrierSpotData data)
    {
        if(data.GetBarrierType() == BarrierType.dungeonBarrier)
        {
            return;
        }

        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject placeholderSpot = Instantiate(placeholderPrefab, position, Quaternion.identity, this.transform) as GameObject;
        PlaceholderObject placeholder = placeholderSpot.GetComponent<PlaceholderObject>();
        if (placeholder != null)
        {
            placeholder.Setup(PlaceholderType.BARRIER, 0);
        }
        else
        {
            Debug.LogError("Error creating placeholder barrier spot");
        }
    }
}
