using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to allow retrieving a World Map Layout from Get Requests.
/// </summary>
[Serializable]
public class Layout
{
    public ThreeDimensionalArray<string> content;

    #region Constructors
    public Layout() { }

    public Layout(ThreeDimensionalArray<string> content)
    {
        this.content = content;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>Layout</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>Layout</c> object containing the data</returns>
    public static Layout CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Layout>(jsonString);
    }

    /// <summary>
    ///     This function converts a three dimensional string array to a <c>Layout</c> object
    /// </summary>
    /// <param name="array">The three dimensional string array to be converted</param>
    /// <returns>The converted <c>Layout</c> object</returns>
    public static Layout ConvertArrayToLayout(string[,,] array)
    {
        TwoDimensionalArray<string>[] secondDimension = new TwoDimensionalArray<string>[array.GetLength(0)];
        for(int thirdDimensionIndex=0; thirdDimensionIndex < array.GetLength(0); thirdDimensionIndex++)
        {
            OneDimensionalArray<string>[] firstDimension = new OneDimensionalArray<string>[array.GetLength(1)];
            for(int secondDimensionIndex=0; secondDimensionIndex < array.GetLength(1); secondDimensionIndex++)
            {
                string[] elements = new string[array.GetLength(2)];                
                for (int firstDimensionIndex = 0; firstDimensionIndex < array.GetLength(2); firstDimensionIndex++)
                {
                    elements[firstDimensionIndex] = array[thirdDimensionIndex, secondDimensionIndex, firstDimensionIndex];
                }
                OneDimensionalArray<string> oneDimensionalArray = new OneDimensionalArray<string>(elements);
                firstDimension[secondDimensionIndex] = oneDimensionalArray;
            }
            TwoDimensionalArray<string> twoDimensionalArray = new TwoDimensionalArray<string>(firstDimension);
            secondDimension[thirdDimensionIndex] = twoDimensionalArray;
        }

        ThreeDimensionalArray<string> thirdDimension = new ThreeDimensionalArray<string>(secondDimension);

        Layout layout = new Layout(thirdDimension);
        return layout;
    }

    /// <summary>
    ///     This function converts a <c>Layout</c> object to a three dimensional string array
    /// </summary>
    /// <param name="layout">The <c>Layout</c> object to be converted</param>
    /// <returns>The converted three dimensional string arry</returns>
    public static string[,,] ConvertLayoutToArray(Layout layout)
    {
        int thirdDimensionSize = layout.content.secondDimension.Length;
        if(thirdDimensionSize == 0)
        {
            return new string[0, 0, 0];
        }

        int secondDimensionSize = layout.content.secondDimension[0].firstDimension.Length;
        if (secondDimensionSize == 0)
        {
            return new string[0, 0, 0];
        }

        int firstDimensionSize = layout.content.secondDimension[0].firstDimension[0].elements.Length;
        if (firstDimensionSize == 0)
        {
            return new string[0, 0, 0];
        }

        string[,,] array = new string[thirdDimensionSize, secondDimensionSize, firstDimensionSize];

        for(int thirdDimensionIndex=0; thirdDimensionIndex<thirdDimensionSize; thirdDimensionIndex++)
        {
            for (int secondDimensionIndex = 0; secondDimensionIndex < secondDimensionSize; secondDimensionIndex++)
            {
                for (int firstDimensionIndex = 0; firstDimensionIndex < firstDimensionSize; firstDimensionIndex++)
                {
                    array[thirdDimensionIndex, secondDimensionIndex, firstDimensionIndex] = layout.content.secondDimension[thirdDimensionIndex].firstDimension[secondDimensionIndex].elements[firstDimensionIndex];
                }
            }
        }
        return array;
    }
}
