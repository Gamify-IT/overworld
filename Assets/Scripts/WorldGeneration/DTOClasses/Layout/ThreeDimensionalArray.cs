using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to allow serializing a three dimensional array.
/// </summary>
[Serializable]
public class ThreeDimensionalArray<T>
{
    public TwoDimensionalArray<T>[] secondDimension;

    #region Constructors
    public ThreeDimensionalArray() { }

    public ThreeDimensionalArray(TwoDimensionalArray<T>[] secondDimension)
    {
        this.secondDimension = secondDimension;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>ThreeDimensionalArray</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>ThreeDimensionalArray</c> object containing the data</returns>
    public static ThreeDimensionalArray<T> CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ThreeDimensionalArray<T>>(jsonString);
    }
}
