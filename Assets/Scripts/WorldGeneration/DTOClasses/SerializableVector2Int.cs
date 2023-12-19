using UnityEngine;

/// <summary>
///     This class is used to transfer an two dimensional vector of ints from and to the overworld backend
/// </summary>
[System.Serializable]
public class SerializableVector2Int
{
    public int x;
    public int y;

    public SerializableVector2Int() { }

    public SerializableVector2Int(int x, int y) 
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    ///     This function converts a json string to a <c>SerializableVector2Int</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>SerializableVector2Int</c> object containing the data</returns>
    public static SerializableVector2Int CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<SerializableVector2Int>(jsonString);
    }
}
