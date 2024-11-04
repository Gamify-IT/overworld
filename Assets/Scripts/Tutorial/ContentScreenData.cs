/// <summary>
/// 
/// </summary>
[System.Serializable]
public class ContentScreenData
{
    public string header;
    public string content;
    public string buttonLabel;

    public ContentScreenData() { }

    public ContentScreenData(string header, string content, string buttonLabel)
    {
        this.header = header;
        this.content = content;
        this.buttonLabel = buttonLabel;
    }

    #region getter 
    public string GetHeader()
    {
        return header;
    }
    public string GetContent() 
    { 
        return content;
    }
    public string GetButtonLabel()
    {
        return buttonLabel;
    }
    #endregion
}