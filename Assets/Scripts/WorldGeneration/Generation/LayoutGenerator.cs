using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines an Layout Generator. It is used to inherit for specific generators implementation.
/// </summary>
public abstract class LayoutGenerator
{
    protected string seed;
    //protected bool[,] layout;
    protected CellType[,] layout;
    protected Vector2Int size;
    protected int accessability;
    protected List<WorldConnection> worldConnections;
    protected int corridorSize;
    protected int floorRoomThreshold;
    protected int wallRoomThreshold;    

    //constructor with seed and world connections
    protected LayoutGenerator(string seed, Vector2Int size, int accessability, List<WorldConnection> worldConnections)
    {
        this.seed = seed;
        //layout = new bool[size.x, size.y];
        layout = new CellType[size.x, size.y];
        this.size = size;
        this.accessability = accessability;
        this.worldConnections = worldConnections;
        GetSettings();
    }

    //constructor with seed and without world connections
    protected LayoutGenerator(string seed, Vector2Int size, int accessability)
    {
        this.seed = seed;
        //layout = new bool[size.x, size.y];
        layout = new CellType[size.x, size.y];
        this.size = size;
        this.accessability = accessability;
        this.worldConnections = new List<WorldConnection>();
        GetSettings();
    }

    /// <summary>
    ///     This function reads to generator settings from the local file and sets up the variables needed
    /// </summary>
    private void GetSettings()
    {
        string path = "GameSettings/GeneratorSettings";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        GenerationSettings generationSettings = GenerationSettings.CreateFromJSON(json);

        corridorSize = generationSettings.corridorSize;
        floorRoomThreshold = generationSettings.floorRoomThreshold;
        wallRoomThreshold = generationSettings.wallRoomThreshold;
    }

    /// <summary>
    ///     This funciton generates a layout based on the local variables of the generator
    /// </summary>
    public abstract void GenerateLayout();

    /// <summary>
    ///     This function returns the generated layout
    /// </summary>
    /// <returns>The generated layout</returns>
    public CellType[,] GetLayout()
    {
        return layout;
    }

    /// <summary>
    ///     This function removes small wall or floor areas and connects all rooms
    /// </summary>
    protected void EnsureConnectivity()
    {
        //Setup room manager
        RoomManager roomManager = new RoomManager(layout, seed);

        //Remove too small areas
        roomManager.RemoveSmallRooms(CellType.WALL, wallRoomThreshold);
        roomManager.RemoveSmallRooms(CellType.FLOOR, floorRoomThreshold);

        //Add world connections, if present
        if (worldConnections.Count > 0)
        {
            roomManager.AddWorldConnections(worldConnections);
        }

        //Connect rooms
        roomManager.ConnectRooms(corridorSize);

        //Remove small wall areas that might were created
        roomManager.RemoveSmallRooms(CellType.WALL, wallRoomThreshold);

        //Retrieve updated layout
        layout = roomManager.GetLayout();
    }
}
