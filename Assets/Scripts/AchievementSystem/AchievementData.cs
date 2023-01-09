using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This class is used to store all relevant information about achievements in the overworld frontend
/// </summary>
public class AchievementData
{
    private static string imageFolder = "AchievementImages";
    private static string defaultImageName = "defaultImage";

    private readonly string title;
    private readonly string description;
    private readonly List<string> categories;
    private readonly string imageName;
    private readonly Sprite image;
    private readonly int amountRequired;
    private int progress;
    private bool completed;

    public AchievementData(string title, string description, List<string> categories, string imageName, int amountRequired, int progress, bool completed) 
    {
        this.title = title;
        this.description = description;
        this.categories = categories;
        this.imageName = imageName;
        this.image = GetImage(imageName);
        this.amountRequired = amountRequired;
        this.progress = progress;
        this.completed = completed;
    }

    /// <summary>
    ///     This function converts a <c>AchievementStatistic</c> to an <c>AchievementUIElement</c>
    /// </summary>
    /// <param name="statistic">The <c>AchievementStatistic</c> to convert</param>
    /// <returns>The converted <c>AchievementUIElement</c> object</returns>
    public static AchievementData ConvertFromAchievementStatistic(AchievementStatistic statistic)
    {
        string title = statistic.achievement.title;
        string description = statistic.achievement.description;
        List<string> categories = statistic.achievement.categories;
        string imageName = statistic.achievement.imageName;
        int amountRequired = statistic.achievement.amountRequired;
        int progress = statistic.progress;
        bool completed = statistic.completed;

        AchievementData data = new AchievementData(title, description, categories, imageName, amountRequired, progress, completed);
        return data;
    }

    /// <summary>
    ///     This function updates the progress and sets the completed flag if needed
    /// </summary>
    /// <param name="newProgress">The new progress</param>
    /// <returns>True if the achievement is just now completed, false otherwise</returns>
    public bool UpdateProgress(int newProgress)
    {
        progress = newProgress;
        if(newProgress >= amountRequired && !completed)
        {
            completed = true;
            return true;
        }
        return false;
    }

    /// <summary>
    ///     This function returns a sprite for an achievement 
    /// </summary>
    /// <param name="imageName">The name of the sprite to return</param>
    /// <returns>The sprite with the given name, if present or the default image otherwise</returns>
    private Sprite GetImage(string imageName)
    {
        var sprite = Resources.Load<Sprite>(imageFolder + "/" + imageName);
        if(sprite == null)
        {
            Debug.Log("Load default image");
            sprite = Resources.Load<Sprite>(imageFolder + "/" + defaultImageName);
        }
        //Debug.Log(sprite.ToString());
        return sprite;
    }

    #region Getter
    public string GetTitle()
    {
        return title;
    }

    public string GetDescription()
    {
        return description;
    }

    public List<string> GetCategories()
    {
        return categories;
    }

    public string GetImageName()
    {
        return imageName;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public int GetAmountRequired()
    {
        return amountRequired;
    }

    public int GetProgress()
    {
        return progress;
    }

    public bool IsCompleted()
    {
        return completed;
    }
    #endregion
}
