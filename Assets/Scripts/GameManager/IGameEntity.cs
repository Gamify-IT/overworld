using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GameEntity is an object that is a component of a gameobject placed in the world. Its data is provided by the server. The generic parameter refers to its corresponding data class.
/// </summary>
public interface IGameEntity<T> where T : IGameEntityData 
{
    public void Setup(T data);
}
