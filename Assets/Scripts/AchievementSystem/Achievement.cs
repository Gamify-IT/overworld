using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This class is used to retrieve <c>Achievement</c> data from Get Requests.
/// </summary>
public class Achievement
{
    public string title;
    public string description;
    public List<string> categories;
    public string imageName;
    public int amountRequired;

    public Achievement(string title, string description, List<string> categories, string imageName, int amountRequired)
    {
        this.title = title;
        this.description = description;
        this.categories = categories;
        this.imageName = imageName;
        this.amountRequired = amountRequired;        
    }

    public Achievement() { }

    /// <summary>
    ///     This function converts a json string to a <c>Achievement</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>Achievement</c> object containing the data</returns>
    public static Achievement CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Achievement>(jsonString);
    }
}
