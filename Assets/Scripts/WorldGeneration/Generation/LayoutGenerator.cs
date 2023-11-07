using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines an Layout Generator. It is used to inherit for specific generators implementation.
/// </summary>
public abstract class LayoutGenerator
{
    protected string seed;
    protected bool[,] layout;
    protected Vector2Int size;
    protected int accessability;
    protected List<WorldConnection> worldConnections;

    //constructor with seed and world connections
    protected LayoutGenerator(string seed, Vector2Int size, int accessability, List<WorldConnection> worldConnections)
    {
        this.seed = seed;
        layout = new bool[size.x, size.y];
        this.size = size;
        this.accessability = accessability;
        this.worldConnections = worldConnections;
    }

    //constructor with seed and without world connections
    protected LayoutGenerator(string seed, Vector2Int size, int accessability)
    {
        this.seed = seed;
        layout = new bool[size.x, size.y];
        this.size = size;
        this.accessability = accessability;
        this.worldConnections = new List<WorldConnection>();
    }

    /// <summary>
    ///     This funciton generates a layout based on the local variables of the generator
    /// </summary>
    public abstract void GenerateLayout();

    /// <summary>
    ///     This function returns the generated layout
    /// </summary>
    /// <returns>The generated layout</returns>
    public bool[,] GetLayout()
    {
        return layout;
    }
}
