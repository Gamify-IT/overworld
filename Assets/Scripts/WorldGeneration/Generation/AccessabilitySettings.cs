using UnityEngine;

/// <summary>
///     This class is used to get the configuration for the accessible setting in the generation process
/// </summary>
[System.Serializable]
public class AccessabilitySettings
{
    //Cellular Automata
    public int verySmallCA;
    public int smallCA;
    public int normalCA;
    public int bigCA;
    public int veryBigCA;

    //Drunkards Walk
    public int verySmallDW;
    public int smallDW;
    public int normalDW;
    public int bigDW;
    public int veryBigDW;

    //Islands - Cellular Automata
    public int verySmallIslandCA;
    public int smallIslandCA;
    public int normalIslandCA;
    public int bigIslandCA;
    public int veryBigIslandCA;

    //Islands - Drunkards Walk
    public int verySmallIslandDW;
    public int smallIslandDW;
    public int normalIslandDW;
    public int bigIslandDW;
    public int veryBigIslandDW;

    public AccessabilitySettings() { }

    /// <summary>
    ///     This function converts a json string to a <c>AccessabilitySettings</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>AccessabilitySettings</c> object containing the data</returns>
    public static AccessabilitySettings CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AccessabilitySettings>(jsonString);
    }
}
