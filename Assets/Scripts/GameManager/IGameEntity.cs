using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GameEntity is an object that is a component of a gameobject placed in the world. Its data is provided by the server.
/// </summary>
public interface IGameEntity
{
    public void Setup(IGameEntityData data);
}
