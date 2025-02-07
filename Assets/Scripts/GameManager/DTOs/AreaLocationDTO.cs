using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class AreaLocationDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>AreaLocationDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>AreaLocationDTO</c> object containing the data</returns>
    public static AreaLocationDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AreaLocationDTO>(jsonString);
    }

    #region Attributes

    public int worldIndex;
    public int dungeonIndex;

    #endregion

    #region Constructors

    public AreaLocationDTO(int worldIndex, int dungeonIndex)
    {
        this.worldIndex = worldIndex;
        this.dungeonIndex = dungeonIndex;
    }

    public AreaLocationDTO()
    {
    }

    #endregion

    public override bool Equals(object obj)
    {
        if (obj is AreaLocationDTO other)
        {
            return this.worldIndex == other.worldIndex && this.dungeonIndex == other.dungeonIndex;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(worldIndex, dungeonIndex);
    }

}