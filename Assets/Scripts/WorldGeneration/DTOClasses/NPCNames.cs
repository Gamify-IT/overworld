using UnityEngine;

/// <summary>
///     This function is used to load a list of possible NPC names from a local json file
/// </summary>
[System.Serializable]
public class NPCNames 
{
    public string[] names;

    /// <summary>
    ///     This function converts a json string to a <c>NPCNames</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>NPCNames</c> object containing the data</returns>
    public static NPCNames CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NPCNames>(jsonString);
    }
}
