/// <summary>
///     This class defines all needed data for a <c>Book</c>.
/// </summary>
public class BookData
{
    public BookData(string uuid, string[] dialogue, bool hasBeenTalkedTo)
    {
        this.uuid = uuid;
        this.dialogue = dialogue;
    }

    public BookData()
    {
        uuid = "";
        string[] text = { "Hi.", "I have nothing to say." };
        dialogue = text;
    }

    #region Attributes

    private string uuid;
    private string[] dialogue;

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
        return dialogue;
    }

    /// <summary>
    ///     This method is used to set the dialogue of the NPC.
    /// </summary>
    /// <param name="dialogue">the dialogue of the NPC</param>
    public void SetBookText(string[] dialogue)
    {
        this.dialogue = dialogue;
    }

    #endregion
}