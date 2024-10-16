using UnityEngine;

/// <summary>
///     This class is used to load the configurations of the different generators from a local json file
/// </summary>
[System.Serializable]
public class GenerationSettings
{
    //General
    public int borderSize;
    public int minAreaSize;
    public int maxAreaSize;
    public int worldConnectionWidth;
    public int corridorSize;
    public int floorRoomThreshold;
    public int wallRoomThreshold;

    //Cellular Automata
    public int iterationsCA;
    public int floorThresholdCA;

    //Drunkards Walk
    public int radiusDW;

    //Islands
    public int innerBorderSize;
    public int connectionSize;
    public int minRoomHeight;
    public int minRoomWidth;

    public GenerationSettings() { }

    /// <summary>
    ///     This function converts a json string to a <c>GenerationSettings</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>GenerationSettings</c> object containing the data</returns>
    public static GenerationSettings CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<GenerationSettings>(jsonString);
    }
}
