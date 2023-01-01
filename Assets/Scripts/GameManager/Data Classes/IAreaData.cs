using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This interface abstracts data classes for worlds and dungeons
/// </summary>
public interface IAreaData
{
    public IGameEntityData GetEntityDataAt<T>(int index) where T : IGameEntity;
    public void InitializeEmptyDataAt<T>(int index) where T : IGameEntity;
}
