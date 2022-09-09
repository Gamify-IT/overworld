/// <summary>
///     This class defines all needed data for a <c>Book</c>.
/// </summary>
public class BookData
{
    public BookData(string uuid, string[] bookText)
    {
        this.uuid = uuid;
        this.bookText = bookText;
    }

    public BookData()
    {
        uuid = "";
        string[] text = { "This is just an empty Book. No one has written anything here." };
        bookText = text;
    }

    #region Attributes

    private string uuid;
    private string[] bookText;

    #endregion

    #region GetterAndSetter

    /// <summary>
    ///     This method returns the UUID of the NPC.
    /// </summary>
    /// <returns>uuid</returns>
    public string GetUuid()
    {
        return uuid;
    }

    /// <summary>
    ///     This method sets the UUID of the NPC.
    /// </summary>
    /// <param name="uuid">uuid of the NPC</param>
    public void SetUuid(string uuid)
    {
        this.uuid = uuid;
    }

    /// <summary>
    ///     This method returns the dialogue of the NPC.
    /// </summary>
    /// <returns>dialogue</returns>
    public string[] GetBookText()
    {
        return bookText;
    }

    /// <summary>
    ///     This method is used to set the dialogue of the NPC.
    /// </summary>
    /// <param name="dialogue">the dialogue of the NPC</param>
    public void SetBookText(string[] dialogue)
    {
        this.bookText = dialogue;
    }

    #endregion
}