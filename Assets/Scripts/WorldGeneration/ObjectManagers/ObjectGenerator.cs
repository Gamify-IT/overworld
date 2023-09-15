using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator
{
    private AreaInformation areaIdentifier;
    private List<string> npcNames;

    public ObjectGenerator(AreaInformation areaIdentifier)
    {
        this.areaIdentifier = areaIdentifier;

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
    public List<MinigameSpotData> GenerateMinigameSpots(List<Vector2> positions)
    {
        List<MinigameSpotData> minigameSpots = new List<MinigameSpotData>();
        int index = 1;

        foreach (Vector2 position in positions)
        {
            MinigameSpotData minigameSpot = new MinigameSpotData(areaIdentifier, index, position);
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
    public List<NpcSpotData> GenerateNpcSpots(List<Vector2> positions)
    {
        List<NpcSpotData> npcSpots = new List<NpcSpotData>();
        int index = 1;

        foreach (Vector2 position in positions)
        {
            //choose random name
            int nameIndex = Random.Range(0, npcNames.Count);
            string name = npcNames[nameIndex];

            //chose random design
            int spriteIndex = Random.Range(0, 17);
            string spriteName = "npc_" + spriteIndex;
            string iconName = "NPCHeads_" + spriteIndex;

            //create NpcSpotData
            NpcSpotData npcSpot = new NpcSpotData(areaIdentifier, index, position, name, spriteName, iconName);
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
    public List<BookSpotData> GenerateBookSpots(List<Vector2> positions)
    {
        List<BookSpotData> bookSpots = new List<BookSpotData>();
        int index = 1;

        foreach (Vector2 position in positions)
        {
            BookSpotData bookSpot = new BookSpotData(areaIdentifier, index, position, "Mysterious Book " + index);
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
    public List<TeleporterSpotData> GenerateTeleporterSpots(List<Vector2> positions)
    {
        List<TeleporterSpotData> teleporterSpots = new List<TeleporterSpotData>();
        int index = 1;

        foreach (Vector2 position in positions)
        {
            TeleporterSpotData teleporterSpot = new TeleporterSpotData(areaIdentifier, index, position, "Teleporter " + index);
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
    public List<SceneTransitionSpotData> GenerateDungeonSpots(List<Vector2> positions)
    {
        List<SceneTransitionSpotData> dungeonSpots = new List<SceneTransitionSpotData>();
        int index = 1;

        foreach (Vector2 position in positions)
        {
            Vector2 size = new Vector2(1.5f, 1.5f);
            AreaInformation areaToLoad = new AreaInformation(areaIdentifier.GetWorldIndex(), new Optional<int>());
            if(!areaIdentifier.IsDungeon())
            {
                areaToLoad.SetDungeonIndex(index);
            }

            FacingDirection facingDirection = FacingDirection.south;

            SceneTransitionSpotData dungeonSpot = new SceneTransitionSpotData(areaIdentifier, position, size, areaToLoad, facingDirection);
            dungeonSpots.Add(dungeonSpot);
            index++;
        }

        return dungeonSpots;
    }
}
