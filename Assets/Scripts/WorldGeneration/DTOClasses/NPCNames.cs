using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
