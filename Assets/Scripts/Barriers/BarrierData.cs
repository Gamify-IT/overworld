/// <summary>
///     This class defines all needed data for a
///     <c>
///         <Barrier/ c>.
/// </summary>
public class BarrierData
{
    #region Attributes

    private bool isActive;

    #endregion

    #region Constructors

    public BarrierData(bool isActive)
    {
        this.isActive = isActive;
    }

    public BarrierData()
    {
        isActive = true;
    }

    #endregion

    #region GetterAndSetter

    /// <summary>
    ///     This function returns the "isActive" attribute.
    /// </summary>
    /// <returns>The "isActive" attribute</returns>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
    ///     This function sets the "isActive" attribute.
    /// </summary>
    /// <param name="isActive">the new "isActive" attribute</param>
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    #endregion
}