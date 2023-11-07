using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsGenerator : LayoutGenerator
{
    private RoomGenerator roomGenerator;

    private int outerBorderSize;
    private int innerBorderSize;
    private int corridorSize;
    private int floorRoomThreshold;
    private int wallRoomThreshold;
    private int minRoomWidth;
    private int minRoomHeight;

    private System.Random pseudoRandomNumberGenerator;

    #region Constructors

    public IslandsGenerator(
        string seed,
        Vector2Int size,
        int accessability,
        List<WorldConnection> worldConnections,
        RoomGenerator roomGenerator)
        : base(seed, size, accessability, worldConnections) 
    {
        GetSettings();
        this.roomGenerator = roomGenerator;
        pseudoRandomNumberGenerator = new System.Random(seed.GetHashCode());
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

        outerBorderSize = generationSettings.borderSize;
        innerBorderSize = generationSettings.innerBorderSize;
        corridorSize = generationSettings.connectionSize;
        floorRoomThreshold = generationSettings.floorRoomThreshold;
        wallRoomThreshold = generationSettings.wallRoomThreshold;
        minRoomWidth = generationSettings.minRoomWidth;
        minRoomHeight = generationSettings.minRoomHeight;
    }

    #endregion

    public override void GenerateLayout()
    {
        Debug.Log("Island layout generator with rooms: " + roomGenerator.ToString() + "\n" +
            "Accessability: " + accessability + "\n" +
            "Seed: " + seed);
        //Islands implementation

        SpacePartitioner spacePartitioner = new BinarySpacePartitioner(seed, minRoomWidth, minRoomHeight);

        Vector2Int spaceOffset = new Vector2Int(outerBorderSize, outerBorderSize);
        Vector2Int spaceSize = new Vector2Int(size.x - 2 * outerBorderSize, size.y - 2 * outerBorderSize);
        Subspace space = new Subspace(spaceOffset, spaceSize);

        List<Subspace> subspaces = spacePartitioner.Partition(space);

        foreach(Subspace subspace in subspaces)
        {
            CreateRoom(subspace);
        }

        //Setup room manager
        RoomManager roomManager = new RoomManager(layout);

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

    private void CreateRoom(Subspace space)
    {
        bool[,] roomLayout = new bool[space.size.x, space.size.y];

        switch(roomGenerator)
        {
            case RoomGenerator.CELLULAR_AUTOMATA:
                roomLayout = CellularAutomataRoom(space);
                break;

            case RoomGenerator.DRUNKARDS_WALK:
                roomLayout = DrunkardsWalkRoom(space);
                break;
        }

        for(int x = 0; x < space.size.x; x++)
        {
            for (int y = 0; y < space.size.y; y++)
            {
                layout[x + space.offset.x, y + space.offset.y] = roomLayout[x, y];
            }
        }
    }

    private bool[,] CellularAutomataRoom(Subspace space)
    {
        string roomSeed = GetNextSeed();

        LayoutGenerator layoutGenerator = new CellularAutomataGenerator(roomSeed, space.size, accessability, innerBorderSize);
        layoutGenerator.GenerateLayout();
        return layoutGenerator.GetLayout();
    }

    private bool[,] DrunkardsWalkRoom(Subspace space)
    {
        string roomSeed = GetNextSeed();

        LayoutGenerator layoutGenerator = new DrunkardsWalkGenerator(roomSeed, space.size, accessability, innerBorderSize);
        layoutGenerator.GenerateLayout();
        return layoutGenerator.GetLayout();
    }

    private string GetNextSeed()
    {
        int seedPart1 = pseudoRandomNumberGenerator.Next(0, 100);
        int seedPart2 = pseudoRandomNumberGenerator.Next(0, 100000);

        return seedPart1.ToString() + "," + seedPart2.ToString();
    }
}
