using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to allow serializing an one dimensional array.
/// </summary>
[Serializable]
public class OneDimensionalArray<T>
{
    public T[] elements;

    #region Constructors
    public OneDimensionalArray() { }

    public OneDimensionalArray(T[] elements)
    {
        this.elements = elements;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>OneDimensionalArray</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>OneDimensionalArray</c> object containing the data</returns>
    public static OneDimensionalArray<T> CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<OneDimensionalArray<T>>(jsonString);
    }
}
