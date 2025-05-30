using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class MinigameTaskDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>MinigameTaskDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>MinigameTaskDTO</c> object containing the data</returns>
    public static MinigameTaskDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MinigameTaskDTO>(jsonString);
    }

    #region Attributes

    public string id;
    public AreaLocationDTO area;
    public int index;
    public string game;
    public string configurationId;
    public string description;

    #endregion

    #region Constructors

    public MinigameTaskDTO(string id, AreaLocationDTO area, int index, string game, string configurationId)
    {
        this.id = id;
        this.area = area;
        this.index = index;
        this.game = game;
        this.configurationId = configurationId;
    }

    public MinigameTaskDTO()
    {
    }

    #endregion
}