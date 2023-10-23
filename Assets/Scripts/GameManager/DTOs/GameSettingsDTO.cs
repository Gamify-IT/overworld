using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettingsDTO
{
    public string overworldBackendPath;
    public int maxWorld;
    public int maxMinigames;
    public int maxNPCs;
    public int maxBooks;
    public int maxDungeons;
    public int maxTeleporters;

    public GameSettingsDTO() { }

    /// <summary>
    ///     This function converts a json string to a <c>GameSettingsDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>GameSettingsDTO</c> object containing the data</returns>
    public static GameSettingsDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<GameSettingsDTO>(jsonString);
    }
}
