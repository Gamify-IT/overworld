
/// <summary>
///     This function contains the information required to identify an area
/// </summary>
public class AreaInformation
{
    #region Attributes
    private int worldIndex;
    private Optional<int> dungeonIndex;
    #endregion

    public AreaInformation(int worldIndex, Optional<int> dungeonIndex)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
    }

    #region GetterAndSetter
    public void SetWorldIndex(int worldIndex)
    {
        this.worldIndex = worldIndex;
    }

    public int GetWorldIndex()
    {
        return worldIndex;
    }

    public void SetDungeonIndex(int dungeonIndex)
    {
        this.dungeonIndex.SetValue(dungeonIndex);
    }

    public bool IsDungeon()
    {
        return dungeonIndex.IsPresent();
    }

    public int GetDungeonIndex()
    {
        return dungeonIndex.Value();
    }
    #endregion
}
