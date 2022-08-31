using System;
using UnityEngine;

/// <summary>
///     This class converts json strings to objects.
/// </summary>
public class JsonHelper
{
    /// <summary>
    ///     This function converts a json string to a object of type <c>T</c>.
    /// </summary>
    /// <typeparam name="T">The type to convert the json in</typeparam>
    /// <param name="json">The json string to be converted</param>
    /// <returns></returns>
    public static T[] GetJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    /// <summary>
    ///     This class is a wrapper to help to convert a json string to a object.
    /// </summary>
    /// <typeparam name="T">The type to convert the json in</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}