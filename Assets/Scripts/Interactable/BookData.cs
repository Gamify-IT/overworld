/// <summary>
///     This class defines all needed data for a <c>Book</c>.
/// </summary>
public class BookData
{
    public BookData(string uuid, string bookText)
    {
        this.uuid = uuid;
        this.bookText = bookText;
    }

    public BookData()
    {
        uuid = "";
        string text = "This is just an empty Book. No one has written anything here.";
        bookText = text;
    }

    #region Attributes

    private string uuid;
    private string bookText;

    #endregion

    #region GetterAndSetter

    /// <summary>
    ///     This method returns the UUID of the Book.
    /// </summary>
    /// <returns>uuid</returns>
    public string GetUuid()
    {
        return uuid;
    }

    /// <summary>
    ///     This method sets the UUID of the Book
    /// </summary>
    /// <param name="uuid">uuid of the Book</param>
    public void SetUuid(string uuid)
    {
        this.uuid = uuid;
    }

    /// <summary>
    ///     This method returns the dialogue of the Book.
    /// </summary>
    /// <returns>bookText</returns>
    public string GetBookText()
    {
        return bookText;
    }

    /// <summary>
    ///     This method is used to set the dialogue of the Book.
    /// </summary>
    /// <param name="bookText">the content of the Book</param>
    public void SetBookText(string bookText)
    {
        this.bookText = bookText;
    }

    #endregion
}