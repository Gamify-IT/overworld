/// <summary>
///     This class defines all needed data for a <c>NPC</c>.
/// </summary>
public class NPCData:IGameEntityData
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

    /// <summary>
    ///     This function converts a NPCDTO to NPCData 
    /// </summary>
    /// <param name="dto">The NPCDTO to convert</param>
    /// <returns>The converted NPCData</returns>
    public static NPCData ConvertDtoToData(NPCDTO dto)
    {
        string uuid = dto.id;
        string[] dialogue = dto.text.ToArray();
        bool hasBeenTalkedTo = false;
        
        if (dialogue.Length == 0)
        {
            string[] dummyText = { "I could tell you something useful...", "...But i don't remember." };
            dialogue = dummyText;
        }

        NPCData data = new NPCData(uuid, dialogue, hasBeenTalkedTo);
        return data;
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