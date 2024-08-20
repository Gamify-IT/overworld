using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve <c>Achievement</c> data from Get Requests.
/// </summary>
[Serializable]
public class KeybindingDTO
{
    public string id;
    public string binding;
    public string key;
    public int volumeLevel;

    public KeybindingDTO(string id, string binding, string key, int volumeLevel)
    {
        this.id = id;
        this.binding = binding;
        this.key = key;
        this.volumeLevel = volumeLevel;
    }

    public KeybindingDTO() { }

    /// <summary>
    ///     This function converts a json string to a <c>KeybindingDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>KeybindingDTO</c> object containing the data</returns>
    public static KeybindingDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<KeybindingDTO>(jsonString);
    }
}
