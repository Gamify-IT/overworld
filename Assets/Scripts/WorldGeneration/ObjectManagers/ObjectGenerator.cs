using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator
{
    private AreaInformation areaIdentifier;
    private Vector2Int offset;
    private List<string> npcNames;

    public ObjectGenerator(AreaInformation areaIdentifier, Vector2Int offset)
    {
        this.areaIdentifier = areaIdentifier;
        this.offset = offset;

        npcNames = GetNamesList();
    }

    /// <summary>
    ///     This function reads the NPC names from the local json file
    /// </summary>
    /// <returns>A list containing all NPC names</returns>
    private List<string> GetNamesList()
    {
        string path = "GameSettings/NPCNames";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        NPCNames npcNamesArray = NPCNames.CreateFromJSON(json);

        List<string> npcNames = new List<string>();
        foreach (string name in npcNamesArray.names)
        {
            npcNames.Add(name);
        }
        return npcNames;
    }

    /// <summary>
    ///     This function creates <c>MinigameSpotData</c> objects for given positions
    /// </summary>
    /// <param name="positions">A list of all minigame positions</param>
    /// <returns>A list of <c>MinigameSpotData</c> objects with the given positions</returns>
    public List<MinigameSpotData> GenerateMinigameSpots(List<Vector2Int> positions)
    {
        List<MinigameSpotData> minigameSpots = new List<MinigameSpotData>();
        int index = 1;

        foreach (Vector2Int position in positions)
        {
            //shift position
            Vector2 shiftedPosition = position + offset + new Vector2(0.5f, 0.5f);

            //create MinigameSpotData
            MinigameSpotData minigameSpot = new MinigameSpotData(areaIdentifier, index, shiftedPosition);
            minigameSpots.Add(minigameSpot);
            index++;
        }

        return minigameSpots;
    }

    /// <summary>
    ///     This function creates <c>NpcSpotData</c> objects for given positions
    /// </summary>
    /// <param name="positions">A list of all npc positions</param>
    /// <returns>A list of <c>NpcSpotData</c> objects with the given positions</returns>
    public List<NpcSpotData> GenerateNpcSpots(List<Vector2Int> positions)
    {
        List<NpcSpotData> npcSpots = new List<NpcSpotData>();
        int index = 1;

        foreach (Vector2Int position in positions)
        {
            //shift position
            Vector2 shiftedPosition = position + offset + new Vector2(0.5f, 0.5f);

            //choose random name
            int nameIndex = Random.Range(0, npcNames.Count);
            string name = npcNames[nameIndex];

            //chose random design
            int spriteIndex = Random.Range(0, 17);
            string spriteName = "npc_" + spriteIndex;
            string iconName = "NPCHeads_" + spriteIndex;

            //create NpcSpotData
            NpcSpotData npcSpot = new NpcSpotData(areaIdentifier, index, shiftedPosition, name, spriteName, iconName);
            npcSpots.Add(npcSpot);
            index++;
        }

        return npcSpots;
    }

    /// <summary>
    ///     This function creates <c>BookSpotData</c> objects for given positions
    /// </summary>
    /// <param name="positions">A list of all book positions</param>
    /// <returns>A list of <c>BookSpotData</c> objects with the given positions</returns>
    public List<BookSpotData> GenerateBookSpots(List<Vector2Int> positions)
    {
        List<BookSpotData> bookSpots = new List<BookSpotData>();
        int index = 1;

        foreach (Vector2Int position in positions)
        {
            //shift position
            Vector2 shiftedPosition = position + offset + new Vector2(0.5f, 0.5f);

            //create BookSpotData
            BookSpotData bookSpot = new BookSpotData(areaIdentifier, index, shiftedPosition, "Mysterious Book " + index);
            bookSpots.Add(bookSpot);
            index++;
        }

        return bookSpots;
    }

    /// <summary>
    ///     This function creates <c>TeleporterSpotData</c> objects for given positions
    /// </summary>
    /// <param name="positions">A list of all teleporter positions</param>
    /// <returns>A list of <c>TeleporterSpotData</c> objects with the given positions</returns>
    public List<TeleporterSpotData> GenerateTeleporterSpots(List<Vector2Int> positions)
    {
        List<TeleporterSpotData> teleporterSpots = new List<TeleporterSpotData>();
        int index = 1;

        foreach (Vector2Int position in positions)
        {
            //shift position
            Vector2 shiftedPosition = position + offset + new Vector2(0.5f, 0.5f);

            //create TeleporterSpotData
            TeleporterSpotData teleporterSpot = new TeleporterSpotData(areaIdentifier, index, shiftedPosition, "Teleporter " + index);
            teleporterSpots.Add(teleporterSpot);
            index++;
        }

        return teleporterSpots;
    }

    /// <summary>
    ///     This function creates <c>SceneTransitionSpotData</c> objects for given positions
    /// </summary>
    /// <param name="dungeonSpotPositions">A list of all dungeon positions</param>
    /// <returns>A list of <c>SceneTransitionSpotData</c> objects with the given positions</returns>
    public List<SceneTransitionSpotData> GenerateDungeonSpots(List<DungeonSpotPosition> dungeonSpotPositions)
    {
        List<SceneTransitionSpotData> dungeonSpots = new List<SceneTransitionSpotData>();
        int index = 1;

        foreach (DungeonSpotPosition dungeonSpotPosition in dungeonSpotPositions)
        {
            Vector2Int position = dungeonSpotPosition.GetPosition();
            DungeonStyle style = dungeonSpotPosition.GetStyle();

            Vector2 size = new Vector2();
            Vector2 shift = new Vector2();

            switch(style)
            {
                case DungeonStyle.HOUSE:
                    size = new Vector2(1.0f, 1.0f);
                    shift = new Vector2(2.5f, 1f);
                    break;

                case DungeonStyle.TRAPDOOR:
                    size = new Vector2(1.5f, 1.0f);
                    shift = new Vector2(1.0f, 1.0f);
                    break;

                case DungeonStyle.GATE:
                    size = new Vector2(1.5f, 1.0f);
                    shift = new Vector2(2.0f, 1.0f);
                    break;

                case DungeonStyle.CAVE_ENTRANCE:
                    size = new Vector2(1.0f, 1.0f);
                    shift = new Vector2(0.5f, 0.5f);
                    break;
            }

            AreaInformation areaToLoad = new AreaInformation(areaIdentifier.GetWorldIndex(), new Optional<int>());
            if(!areaIdentifier.IsDungeon())
            {
                areaToLoad.SetDungeonIndex(index);
            }

            FacingDirection facingDirection = FacingDirection.south;

            //shift position
            Vector2 shiftedPosition = position + shift + offset;

            //create DungeonSpotData
            SceneTransitionSpotData dungeonSpot = new SceneTransitionSpotData(areaIdentifier, shiftedPosition, size, areaToLoad, facingDirection, style);
            dungeonSpots.Add(dungeonSpot);
            index++;
        }

        return dungeonSpots;
    }

    /// <summary>
    ///     This function creates <c>BarrierSpotData</c> objects for given dungeon positions
    /// </summary>
    /// <param name="dungeonSpotPositions">A list of all dungeon positions</param>
    /// <returns>A list of <c>BarrierSpotData</c> objects with the given positions</returns>
    public List<BarrierSpotData> GenerateBarrierSpots(List<DungeonSpotPosition> dungeonSpotPositions)
    {
        List<BarrierSpotData> barrierSpots = new List<BarrierSpotData>();
        int index = 1;

        foreach (DungeonSpotPosition dungeonSpotPosition in dungeonSpotPositions)
        {
            Vector2Int position = dungeonSpotPosition.GetPosition();
            BarrierStyle style = BarrierStyle.TREE;

            Vector2 shift = new Vector2();

            switch (dungeonSpotPosition.GetStyle())
            {
                case DungeonStyle.HOUSE:
                    style = BarrierStyle.HOUSE;
                    shift = new Vector2(2.5f, 1f);
                    break;

                case DungeonStyle.TRAPDOOR:
                    style = BarrierStyle.TRAPDOOR;
                    shift = new Vector2(1.0f, 1.0f);
                    break;

                case DungeonStyle.GATE:
                    style = BarrierStyle.GATE;
                    shift = new Vector2(2.0f, 1.0f);
                    break;

                case DungeonStyle.CAVE_ENTRANCE:
                    style = BarrierStyle.TREE;
                    shift = new Vector2(0.5f, -0.5f);
                    break;
            }

            BarrierType type;
            int destinationAreaIndex;
            if (areaIdentifier.IsDungeon())
            {
                type = BarrierType.worldBarrier;
                destinationAreaIndex = areaIdentifier.GetWorldIndex();
                
            }
            else
            {
                type = BarrierType.dungeonBarrier;
                destinationAreaIndex = index;
            }

            //shift position
            Vector2 shiftedPosition = position + shift + offset;

            //create BarrierSpotData
            BarrierSpotData barrierSpot = new BarrierSpotData(areaIdentifier, shiftedPosition, type, destinationAreaIndex, style);
            barrierSpots.Add(barrierSpot);
            index++;
        }

        return barrierSpots;
    }

    /// <summary>
    ///     This function creates <c>BarrierSpotData</c> objects for given positions
    /// </summary>
    /// <param name="barrierSpotPositions">A list of all barrier positions</param>
    /// <returns>A list of <c>BarrierSpotData</c> objects with the given positions</returns>
    public List<BarrierSpotData> GenerateBarrierSpots(List<BarrierSpotPosition> barrierSpotPositions)
    {
        List<BarrierSpotData> barrierSpots = new List<BarrierSpotData>();

        foreach (BarrierSpotPosition barrierSpotPosition in barrierSpotPositions)
        {
            Vector2Int position = barrierSpotPosition.GetPosition();
            BarrierStyle style = barrierSpotPosition.GetStyle();
            BarrierType type = BarrierType.worldBarrier; ;
            int destinationAreaIndex = barrierSpotPosition.GetDestinationWorld();

            //shift position
            Vector2 shiftedPosition = position + offset + new Vector2(0.5f, 0.5f);

            //create BarrierSpotData
            BarrierSpotData barrierSpot = new BarrierSpotData(areaIdentifier, shiftedPosition, type, destinationAreaIndex, style);
            barrierSpots.Add(barrierSpot);
        }

        return barrierSpots;
    }
}
