using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    #region Singleton

    public static ObjectManager Instance { get; private set; }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            SetupObjectManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Attributes

    private GameObject[,] minigameObjects;
    private GameObject[,] worldBarrierObjects;
    private GameObject[,] dungeonBarrierObjects;
    private GameObject[,] npcObjects;
    private GameObject[,] bookObjects;

    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxBooks;
    private int maxDungeons;

    #endregion

    #region Setup

    /// <summary>
    ///     This function initializes the <c>ObjectManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    private void SetupObjectManager()
    {
        maxWorld = GameSettings.GetMaxWorlds();
        maxMinigames = GameSettings.GetMaxMinigames();
        maxNPCs = GameSettings.GetMaxNpCs();
        maxBooks = GameSettings.GetMaxBooks();
        maxDungeons = GameSettings.GetMaxDungeons();

        minigameObjects = new GameObject[maxWorld + 1, maxMinigames + 1];
        worldBarrierObjects = new GameObject[maxWorld + 1, maxWorld + 1];
        dungeonBarrierObjects = new GameObject[maxWorld + 1, maxDungeons + 1];
        npcObjects = new GameObject[maxWorld + 1, maxNPCs + 1];
        bookObjects = new GameObject[maxWorld + 1, maxBooks + 1];
    }

    /// <summary>
    ///     This function registers a new minigame at the <c>GameManager</c>
    /// </summary>
    /// <param name="minigame">The minigame gameObject</param>
    /// <param name="world">The index of the world the minigame is in</param>
    /// <param name="dungeon">The index of the dungeon the minigame is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the minigame in its area</param>
    public void AddMinigame(GameObject minigame, int world, int dungeon, int number)
    {
        if (minigame != null)
        {
            if (dungeon == 0)
            {
                minigameObjects[world, number] = minigame;
            }
            else
            {
                minigameObjects[0, number] = minigame;
            }
        }
    }

    /// <summary>
    ///     This function removes a minigame from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the minigame is in</param>
    /// <param name="dungeon">The index of the dungeon the minigame is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the minigame in its area</param>
    public void RemoveMinigame(int world, int dungeon, int number)
    {
        if (dungeon == 0)
        {
            minigameObjects[world, number] = null;
        }
        else
        {
            minigameObjects[0, number] = null;
        }
    }

    /// <summary>
    ///     This function registers a new barrier at the <c>GameManager</c>
    /// </summary>
    /// <param name="barrier">The barrier gameObject</param>
    /// <param name="originWorldIndex">The index of the world which exit the barrier is blocking</param>
    /// <param name="destinationAreaIndex">The index of the world which entry the barrier is blocking</param>
    public void AddBarrier(GameObject barrier, BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        if (barrier != null)
        {
            switch (type)
            {
                case BarrierType.worldBarrier:
                    worldBarrierObjects[originWorldIndex, destinationAreaIndex] = barrier;
                    break;
                case BarrierType.dungeonBarrier:
                    dungeonBarrierObjects[originWorldIndex, destinationAreaIndex] = barrier;
                    break;
            }
        }
    }

    /// <summary>
    ///     This function removes a barrier from the <c>GameManager</c>
    /// </summary>
    /// <param name="worldIndexOrigin">The index of the world which exit the barrier is blocking</param>
    /// <param name="worldIndexDestination">The index of the world which entry the barrier is blocking</param>
    public void RemoveBarrier(BarrierType type, int originWorldIndex, int destinationAreaIndex)
    {
        switch (type)
        {
            case BarrierType.worldBarrier:
                worldBarrierObjects[originWorldIndex, destinationAreaIndex] = null;
                break;
            case BarrierType.dungeonBarrier:
                dungeonBarrierObjects[originWorldIndex, destinationAreaIndex] = null;
                break;
        }
    }

    /// <summary>
    ///     This function registers a new npc at the <c>GameManager</c>
    /// </summary>
    /// <param name="npc">The npc gameObject</param>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void AddNpc(GameObject npc, int world, int dungeon, int number)
    {
        if (npc != null)
        {
            if (dungeon == 0)
            {
                npcObjects[world, number] = npc;
            }
            else
            {
                npcObjects[0, number] = npc;
            }
        }
    }

    /// <summary>
    ///     This function removes a npc from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the npc is in</param>
    /// <param name="dungeon">The index of the dungeon the npc is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the npc in its area</param>
    public void RemoveNpc(int world, int dungeon, int number)
    {
        if (dungeon == 0)
        {
            npcObjects[world, number] = null;
            string[] emptyArray = { "" };
        }
        else
        {
            npcObjects[0, number] = null;
            string[] emptyArray = { "" };
        }
    }

    /// <summary>
    ///     This function registers a new book at the <c>GameManager</c>
    /// </summary>
    /// <param name="book">The npc gameObject</param>
    /// <param name="world">The index of the world the book is in</param>
    /// <param name="dungeon">The index of the dungeon the book is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the book in its area</param>
    public void AddBook(GameObject book, int world, int dungeon, int number)
    {
        if (book != null)
        {
            if (dungeon == 0)
            {
                bookObjects[world, number] = book;
            }
            else
            {
                bookObjects[0, number] = book;
            }
        }
    }

    /// <summary>
    ///     This function removes a book from the <c>GameManager</c>
    /// </summary>
    /// <param name="world">The index of the world the book is in</param>
    /// <param name="dungeon">The index of the dungeon the book is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the book in its area</param>
    public void RemoveBook(int world, int dungeon, int number)
    {
        if (dungeon == 0)
        {
            bookObjects[world, number] = null;
            string[] emptyArray = { "" };
        }
        else
        {
            bookObjects[0, number] = null;
            string[] emptyArray = { "" };
        }
    }

    #endregion

    #region SettingData

    /// <summary>
    ///     This functions sets the data for a given world.
    /// </summary>
    /// <param name="worldIndex">The index of the world</param>
    /// <param name="data">The data to be set</param>
    public void SetWorldData(int worldIndex, WorldData data)
    {
        if (worldIndex <= 0 || worldIndex > maxWorld)
        {
            return;
        }

        for (int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData minigameData = data.getMinigameData(minigameIndex);
            if (minigameData == null)
            {
                minigameData = new MinigameData();
            }

            GameObject minigameObject = minigameObjects[worldIndex, minigameIndex];
            if (minigameObject == null)
            {
                continue;
            }

            Minigame minigame = minigameObject.GetComponent<Minigame>();
            if (minigame == null)
            {
                continue;
            }

            minigame.Setup(minigameData);
        }

        for (int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            NPCData npcData = data.getNPCData(npcIndex);
            if (npcData == null)
            {
                npcData = new NPCData();
            }

            GameObject npcObject = npcObjects[worldIndex, npcIndex];
            if (npcObject == null)
            {
                continue;
            }

            NPC npc = npcObject.GetComponent<NPC>();
            if (npc == null)
            {
                continue;
            }

            npc.Setup(npcData);
        }

        for (int bookIndex = 1; bookIndex <= maxBooks; bookIndex++)
        {
            BookData bookData = data.getBookData(bookIndex);
            if (bookData == null)
            {
                bookData = new BookData();
            }

            GameObject bookObject = bookObjects[worldIndex, bookIndex];
            if (bookObject == null)
            {
                continue;
            }

            Book book = bookObject.GetComponent<Book>();
            if (book == null)
            {
                continue;
            }

            book.Setup(bookData);
        }

        for (int barrierDestinationIndex = 1; barrierDestinationIndex <= maxWorld; barrierDestinationIndex++)
        {
            GameObject barrierObject = worldBarrierObjects[worldIndex, barrierDestinationIndex];
            if (barrierObject == null)
            {
                continue;
            }

            Barrier barrier = barrierObject.GetComponent<Barrier>();
            if (barrier == null)
            {
                continue;
            }

            bool activedByLecturer = false;
            WorldData worldData = DataManager.Instance.GetWorldData(barrierDestinationIndex);
            if(worldData != null)
            {
                activedByLecturer = worldData.isActive();
            }
            bool unlockedByPlayer = DataManager.Instance.PlayerHasWorldUnlocked(barrierDestinationIndex);
            bool worldExplorable = activedByLecturer & unlockedByPlayer;
            BarrierData barrierData = new BarrierData(!worldExplorable);
            barrier.Setup(barrierData);
        }

        for (int barrierDestinationIndex = 1; barrierDestinationIndex <= maxDungeons; barrierDestinationIndex++)
        {
            GameObject barrierObject = dungeonBarrierObjects[worldIndex, barrierDestinationIndex];
            if (barrierObject == null)
            {
                continue;
            }

            Barrier barrier = barrierObject.GetComponent<Barrier>();
            if (barrier == null)
            {
                continue;
            }

            bool activedByLecturer = false;
            DungeonData dungeonData = DataManager.Instance.GetDungeonData(worldIndex, barrierDestinationIndex);
            if (dungeonData != null)
            {
                activedByLecturer = dungeonData.IsActive();
            }
            bool unlockedByPlayer = DataManager.Instance.PlayerHasDungeonUnlocked(worldIndex, barrierDestinationIndex);
            bool dungeonExplorable = activedByLecturer & unlockedByPlayer;
            BarrierData barrierData = new BarrierData(!dungeonExplorable);
            barrier.Setup(barrierData);
        }
    }

    /// <summary>
    ///     This functions sets the data for a given dungeon.
    /// </summary>
    /// <param name="worldIndex">The index of the world the dungeon is in</param>
    /// <param name="dungeonIndex">The index of the dungeon</param>
    /// <param name="data">The data to be set</param>
    public void SetDungeonData(int worldIndex, int dungeonIndex, DungeonData data)
    {
        if(worldIndex <= 0 || worldIndex > maxWorld)
        {
            return;
        }

        if(dungeonIndex <= 0 ||dungeonIndex > maxDungeons)
        {
            return;
        }

        for (int minigameIndex = 1; minigameIndex <= maxMinigames; minigameIndex++)
        {
            MinigameData minigameData = data.GetMinigameData(minigameIndex);
            if (minigameData == null)
            {
                minigameData = new MinigameData();
            }

            GameObject minigameObject = minigameObjects[0, minigameIndex];
            if (minigameObject == null)
            {
                continue;
            }

            Minigame minigame = minigameObject.GetComponent<Minigame>();
            if (minigame == null)
            {
                continue;
            }

            minigame.Setup(minigameData);
        }

        for (int npcIndex = 1; npcIndex <= maxNPCs; npcIndex++)
        {
            NPCData npcData = data.GetNpcData(npcIndex);
            if (npcData == null)
            {
                npcData = new NPCData();
            }

            GameObject npcObject = npcObjects[0, npcIndex];
            if (npcObject == null)
            {
                continue;
            }

            NPC npc = npcObject.GetComponent<NPC>();
            if (npc == null)
            {
                continue;
            }

            npc.Setup(npcData);
        }

        for (int bookIndex = 1; bookIndex <= maxBooks; bookIndex++)
        {
            BookData bookData = data.GetBookData(bookIndex);
            if (bookData == null)
            {
                bookData = new BookData();
            }

            GameObject bookObject = bookObjects[0, bookIndex];
            if (bookObject == null)
            {
                continue;
            }

            Book book = bookObject.GetComponent<Book>();
            if (book == null)
            {
                continue;
            }

            book.Setup(bookData);
        }
    }

    #endregion
}
