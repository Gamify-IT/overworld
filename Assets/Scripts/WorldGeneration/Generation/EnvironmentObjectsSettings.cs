using UnityEngine;

/// <summary>
///     This class is used to load the configuration of the <c>EnvironmentalObjectGenerator</c> from a local json file
/// </summary>
[System.Serializable]
public class EnvironmentObjectsSettings
{
    public float maxObjectPercentage;
    public int maxIterationsPerObject;
    public int minDistance;
    public float spawnChance;
    public int spawnDistance;
    public float bigStoneExpandChance;
    public float bushExpandChance;
    public float treeExpandChance;
    public float fenceExpandChance;
    public int maxLogSize;
    public float logExpandChance;

    public EnvironmentObjectsSettings() { }

    /// <summary>
    ///     This function converts a json string to a <c>EnvironmentObjectsSettings</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>EnvironmentObjectsSettings</c> object containing the data</returns>
    public static EnvironmentObjectsSettings CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<EnvironmentObjectsSettings>(jsonString);
    }
}
