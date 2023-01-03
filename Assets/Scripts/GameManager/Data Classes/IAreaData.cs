using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This interface abstracts data classes for worlds and dungeons
/// </summary>
public interface IAreaData
{
    /// <summary>
    /// Returns the data object at given index inside the array of GameEntityData of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    public T GetEntityDataAt<T>(int index) where T : IGameEntityData;
}
