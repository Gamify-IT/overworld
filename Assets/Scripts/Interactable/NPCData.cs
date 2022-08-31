/// <summary>
///     This class defines all needed data for a <c>NPC</c>.
/// </summary>
public class NPCData
{
    public NPCData(string uuid, string[] dialogue, bool hasBeenTalkedTo)
    {
        this.uuid = uuid;
        this.dialogue = dialogue;
        this.hasBeenTalkedTo = hasBeenTalkedTo;
    }

    public NPCData()
    {
        uuid = "";
        string[] text = { "Hi.", "I have nothing to say." };
        dialogue = text;
        hasBeenTalkedTo = true;
    }

    #region Attributes

    private string uuid;
    private string[] dialogue;
    private bool hasBeenTalkedTo;

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
    public string[] GetDialogue()
    {
        return dialogue;
    }

    /// <summary>
    ///     This method is used to set the dialogue of the NPC.
    /// </summary>
    /// <param name="dialogue">the dialogue of the NPC</param>
    public void SetDialogue(string[] dialogue)
    {
        this.dialogue = dialogue;
    }

    /// <summary>
    ///     This function returns the value if a NPC has been talked to.
    /// </summary>
    /// <returns>hasBeenTalkedTo</returns>
    public bool GetHasBeenTalkedTo()
    {
        return hasBeenTalkedTo;
    }

    /// <summary>
    ///     This function sets the hasBeenTalkedTo attribute to the value of 'hasBeenTalkedTo'.
    /// </summary>
    /// <param name="hasBeenTalkedTo">boolean if the NPC has been talk to</param>
    public void SetHasBeenTalkedTo(bool hasBeenTalkedTo)
    {
        this.hasBeenTalkedTo = hasBeenTalkedTo;
    }

    #endregion
}