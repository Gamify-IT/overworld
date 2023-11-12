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

        //TODO: read from json list
        npcNames = new List<string>();
        npcNames.Add("Bob");
        npcNames.Add("Martin");
        npcNames.Add("Fred");
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
    /// <param name="positions">A list of all dungeon positions</param>
    /// <returns>A list of <c>SceneTransitionSpotData</c> objects with the given positions</returns>
    public List<SceneTransitionSpotData> GenerateDungeonSpots(List<Vector2Int> positions)
    {
        List<SceneTransitionSpotData> dungeonSpots = new List<SceneTransitionSpotData>();
        int index = 1;

        foreach (Vector2Int position in positions)
        {
            Vector2 size = new Vector2(1.5f, 1.5f);
            AreaInformation areaToLoad = new AreaInformation(areaIdentifier.GetWorldIndex(), new Optional<int>());
            if(!areaIdentifier.IsDungeon())
            {
                areaToLoad.SetDungeonIndex(index);
            }

            FacingDirection facingDirection = FacingDirection.south;

            //shift position
            Vector2 shiftedPosition = position + offset + new Vector2(0.5f, 0.5f);

            //create MinigameSpotData
            SceneTransitionSpotData dungeonSpot = new SceneTransitionSpotData(areaIdentifier, shiftedPosition, size, areaToLoad, facingDirection);
            dungeonSpots.Add(dungeonSpot);
            index++;
        }

        return dungeonSpots;
    }
}
