using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This class is used to retrieve <c>Achievement</c> data from Get Requests.
/// </summary>
public class Achievement
{
    public string achievementTitle;
    public string description;
    public string imageName;
    public int amountRequired;
    public string[] categories;

    public Achievement(string achievementTitle, string description, string[] categories, string imageName, int amountRequired)
    {
        this.achievementTitle = achievementTitle;
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
