using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to allow serializing a two dimensional array.
/// </summary>
[Serializable]
public class TwoDimensionalArray<T>
{
    public OneDimensionalArray<T>[] firstDimension;

    #region Constructors
    public TwoDimensionalArray() { }

    public TwoDimensionalArray(OneDimensionalArray<T>[] firstDimension)
    {
        this.firstDimension = firstDimension;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>TwoDimensionalArray</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>TwoDimensionalArray</c> object containing the data</returns>
    public static TwoDimensionalArray<T> CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TwoDimensionalArray<T>>(jsonString);
    }
}
