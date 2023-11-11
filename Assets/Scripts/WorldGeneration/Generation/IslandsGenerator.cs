using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsGenerator : LayoutGenerator
{
    private RoomGenerator roomGenerator;

    private int outerBorderSize;
    private int innerBorderSize;
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

        EnsureConnectivity();
    }

    /// <summary>
    ///     This function creates room of the defined type in the given subspace
    /// </summary>
    /// <param name="space">The space to create the room in</param>
    private void CreateRoom(Subspace space)
    {
        CellType[,] roomLayout = new CellType[space.size.x, space.size.y];

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

    /// <summary>
    ///     This function creates a cellular automata room in the given subspace
    /// </summary>
    /// <param name="space">The space to create the room in</param>
    /// <returns>The created room layout</returns>
    private CellType[,] CellularAutomataRoom(Subspace space)
    {
        string roomSeed = GetNextSeed();

        LayoutGenerator layoutGenerator = new CellularAutomataGenerator(roomSeed, space.size, accessability, innerBorderSize);
        layoutGenerator.GenerateLayout();
        return layoutGenerator.GetLayout();
    }

    /// <summary>
    ///     This function creates a dunkard's walk room in the given subspace
    /// </summary>
    /// <param name="space">The space to create the room in</param>
    /// <returns>The created room layout</returns>
    private CellType[,] DrunkardsWalkRoom(Subspace space)
    {
        string roomSeed = GetNextSeed();

        LayoutGenerator layoutGenerator = new DrunkardsWalkGenerator(roomSeed, space.size, accessability, innerBorderSize);
        layoutGenerator.GenerateLayout();
        return layoutGenerator.GetLayout();
    }

    /// <summary>
    ///     This function creates a seed used for the individual room generators
    /// </summary>
    /// <returns></returns>
    private string GetNextSeed()
    {
        int seedPart1 = pseudoRandomNumberGenerator.Next(0, 100);
        int seedPart2 = pseudoRandomNumberGenerator.Next(0, 100000);

        return seedPart1.ToString() + "," + seedPart2.ToString();
    }
}
