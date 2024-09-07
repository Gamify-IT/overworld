using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     This class is used to store all relevant information about achievements in the overworld frontend
/// </summary>
public class AchievementData
{
    private static string imageFolder = "AchievementImages";
    private static string defaultImageName = "defaultImage";

    private readonly string id;
    private readonly string title;
    private readonly string description;
    private readonly List<string> categories;
    private readonly string imageName;
    private readonly Sprite image;
    private readonly int amountRequired;
    private int progress;
    private bool completed;
    private bool updated;
    private List<(int,int,int)> interactedObjects;

    public AchievementData(string id, string title, string description, List<string> categories, string imageName, int amountRequired, int progress, bool completed, List<(int,int,int)> interactedObjects) 
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.categories = categories;
        this.imageName = imageName;
        this.image = GetImage(imageName);
        this.amountRequired = amountRequired;
        this.progress = progress;
        this.completed = completed;
        updated = false;
        this.interactedObjects = interactedObjects;
    }

    /// <summary>
    ///     This function converts a <c>AchievementStatistic</c> to an <c>AchievementData</c>
    /// </summary>
    /// <param name="statistic">The <c>AchievementStatistic</c> to convert</param>
    /// <returns>The converted <c>AchievementData</c> object</returns>
    public static AchievementData ConvertFromAchievementStatistic(AchievementStatistic statistic)
    {
        string id = statistic.id;
        string title = statistic.achievement.achievementTitle;
        string description = statistic.achievement.description;
        List<string> categories = new List<string>();
        foreach(string category in statistic.achievement.categories)
        {
            categories.Add(category);
        }
        string imageName = statistic.achievement.imageName;
        int amountRequired = statistic.achievement.amountRequired;
        int progress = statistic.progress;
        bool completed = statistic.completed;
        List<(int, int, int)> interactedObjects = AchievementStatistic.ConvertFromListIntTuple(statistic.interactedObjects);

        AchievementData data = new AchievementData(id, title, description, categories, imageName, amountRequired, progress, completed, interactedObjects);
        return data;
    }

    /// <summary>
    ///     This function converts a <c>AchievementData</c> to an <c>AchievementStatistic</c>
    /// </summary>
    /// <param name="achievementData">The <c>AchievementData</c> to convert</param>
    /// <returns>The converted <c>AchievementStatistic</c> object</returns>
    public static AchievementStatistic ConvertToAchievmentStatistic(AchievementData achievementData)
    {
        string title = achievementData.GetTitle();
        string description = achievementData.GetDescription();
        string[] categories = achievementData.GetCategories().ToArray();
        string imageName = achievementData.GetImageName();
        int amountRequired = achievementData.GetAmountRequired();

        string id = achievementData.GetId();
        int progress = achievementData.GetProgress();
        bool completed = achievementData.IsCompleted();

        List<IntTuple> interactedObjects = AchievementStatistic.ConvertToListIntTuple(achievementData.GetInteractedObjects());

        Achievement achievement = new Achievement(title, description, categories, imageName, amountRequired);

        AchievementStatistic achievementStatistic = new AchievementStatistic(id, achievement, progress, completed, interactedObjects);

        return achievementStatistic;
    }

    /// <summary>
    ///     This function updates the progress and sets the completed flag if needed
    /// </summary>
    /// <param name="newProgress">The new progress</param>
    /// <returns>True if the achievement is just now completed, false otherwise</returns>
    public bool UpdateProgress(int newProgress, List<(int, int, int)> newInteractedObjects)
    {
        if(completed)
        {
            return false;
        }
        updated = true;
        progress = newProgress;
        interactedObjects = newInteractedObjects;
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
            //Debug.Log("Load default image");
            sprite = Resources.Load<Sprite>(imageFolder + "/" + defaultImageName);
        }
        //Debug.Log(sprite.ToString());
        return sprite;
    }

    #region Getter

    public string GetId()
    {
        return id;
    }

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

    public bool isUpdated()
    {
        return updated;
    }

    public List<(int,int,int)> GetInteractedObjects()
    {
        return interactedObjects;
    }
    #endregion
}
