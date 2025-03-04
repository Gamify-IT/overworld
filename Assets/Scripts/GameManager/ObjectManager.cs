using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
///     The <c>ObjectManager</c> manages the communication between the <c>GameManager</c> and <c>GameObjects</c> in the areas. It also sets up these objects with provided data.
/// </summary>
public class ObjectManager : MonoBehaviour
{
    //Singleton
    public static ObjectManager Instance { get; private set; }

    //Game settigs
    private int maxWorld;
    private int maxMinigames;
    private int maxNPCs;
    private int maxBooks;
    private int maxDungeons;
    private int maxTeleporters;

    //Object refernce fields
    private GameObject[,] minigameObjects;
    private GameObject[,] worldBarrierObjects;
    private GameObject[,] dungeonBarrierObjects;
    private GameObject[,] npcObjects;
    private GameObject[,] bookObjects;
    private GameObject[,] teleporterObjects;

    /// <summary>
    ///     This function registers a new barrier at the <c>ObjectManager</c>
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
    ///     This function removes a barrier from the <c>ObjectManager</c>
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
    ///     This function registers a new game entity at the <c>ObjectManager</c>. Allowed game entites are Book, NPC, Teleporter and Minigame.
    ///     The type is given by the generic parameter.
    /// </summary>
    /// <param name="entity">The gameObject to add</param>
    /// <param name="world">The index of the world the game entity is in</param>
    /// <param name="dungeon">The index of the dungeon the game entity is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the game entity in its area</param>
    public void AddGameEntity<T,U>(GameObject entity, int world, int dungeon, int number) where T : IGameEntity<U> where U: IGameEntityData
    {
        if (entity == null)
        {
            return;
        }
        try
        {
            GameObject[,] targetArray = GetArrayForGameEntity<T,U>();
            if (dungeon == 0)
            {
                targetArray[world, number] = entity;
            }
            else
            {
                targetArray[0, number] = entity;
            }
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError("Entity could not be added to the object manager. " + e.StackTrace);
            return;
        }
    }

    /// <summary>
    ///     This function removes a game entity at the <c>ObjectManager</c>. Allowed game entites are Book, NPC, Teleporter and Minigame.
    ///     The type is given by the generic parameter.
    /// </summary>
    /// <param name="entity">The gameObject to add</param>
    /// <param name="world">The index of the world the game entity is in</param>
    /// <param name="dungeon">The index of the dungeon the game entity is in (0 if in no dungeon)</param>
    /// <param name="number">The index of the game entity in its area</param>
    public void RemoveGameEntity<T,U>(int world, int dungeon, int number) where T : IGameEntity<U> where U : IGameEntityData
    {
        try
        {
            GameObject[,] targetArray = GetArrayForGameEntity<T,U>();
            if (dungeon == 0)
            {
                targetArray[world, number] = null;
            }
            else
            {
                targetArray[0, number] = null;
            }
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError("Entity could not be removed from the object manager" + e.StackTrace);
            return;
        }

    }

    /// <summary>
    /// This function returns the object array with components of type T. An exception is thrown if this array is not existing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private GameObject[,] GetArrayForGameEntity<T,U>() where T : IGameEntity<U> where U: IGameEntityData
    {
        GameObject[,] targetArray;
        if (typeof(T) == typeof(Book))
        {
            targetArray = bookObjects;
        }
        else if (typeof(T) == typeof(NPC))
        {
            targetArray = npcObjects;
        }
        else if (typeof(T) == typeof(Teleporter))
        {
            targetArray = teleporterObjects;
        }
        else if (typeof(T) == typeof(Minigame))
        {
            targetArray = minigameObjects;
        }
        else
        {
            throw new ArgumentOutOfRangeException("There exists no GameObject[,] for " + typeof(T).FullName);
        }
        return targetArray;
    }

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
        SetupEntityData<Minigame,MinigameData>(worldIndex, data);
        SetupEntityData<NPC,NPCData>(worldIndex, data);
        SetupEntityData<Book,BookData>(worldIndex, data);
        SetupEntityData<Teleporter,TeleporterData>(worldIndex, data);

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
            if (worldData != null)
            {
                activedByLecturer = worldData.isActive();
            }
            bool unlockedByPlayer = DataManager.Instance.IsWorldUnlocked(barrierDestinationIndex);
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
            bool unlockedByPlayer = DataManager.Instance.IsDungeonUnlocked(worldIndex, barrierDestinationIndex);
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
        if (worldIndex <= 0 || worldIndex > maxWorld)
        {
            return;
        }

        if (dungeonIndex <= 0 || dungeonIndex > maxDungeons)
        {
            return;
        }

        SetupEntityData<Minigame,MinigameData>(0, data);
        SetupEntityData<NPC,NPCData>(0, data);
        SetupEntityData<Book,BookData>(0, data);
        SetupEntityData<Teleporter, TeleporterData>(0, data);
    }

    /// <summary>
    /// This function calls the setup method of all GameEntities given by the generic parameter T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="worldIndex"></param>
    /// <param name="data"></param>
    private void SetupEntityData<T,U>(int worldIndex, IAreaData data) where T : IGameEntity<U> where U : IGameEntityData, new()
    {
        if(data == null)
        {
            Debug.Log("Data is null");
            return;
        }
        GameObject[,] entityArray;
        try
        {
            entityArray = GetArrayForGameEntity<T,U>();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError(e.ToString());
            return;
        }
        for (int i = 1; i < entityArray.GetLength(1); i++)
        {
            GameObject entityGameObject = entityArray[worldIndex, i];
            if (entityGameObject == null)
            {
                continue;
            }
            T entity = entityGameObject.GetComponent<T>();
            if (entity == null)
            {
                continue;
            }
            U entityData = data.GetEntityDataAt<U>(i);
            if (entityData == null)
            {
                entityData = new U();
            }
            entity.Setup(entityData);
        }
    }

    /// <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetupObjectManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    ///     This function initializes the <c>ObjectManager</c>. All arrays are initialized with empty objects.
    /// </summary>
    public void SetupObjectManager()
    {
        Debug.Log("Reading max object counts");

        maxWorld = GameSettings.GetMaxWorlds();
        maxMinigames = GameSettings.GetMaxMinigames();
        maxNPCs = GameSettings.GetMaxNpcs();
        maxBooks = GameSettings.GetMaxBooks();
        maxDungeons = GameSettings.GetMaxDungeons();
        maxTeleporters = GameSettings.GetMaxTeleporters();

        minigameObjects = new GameObject[maxWorld + 1, maxMinigames + 1];
        worldBarrierObjects = new GameObject[maxWorld + 1, maxWorld + 1];
        dungeonBarrierObjects = new GameObject[maxWorld + 1, maxDungeons + 1];
        npcObjects = new GameObject[maxWorld + 1, maxNPCs + 1];
        bookObjects = new GameObject[maxWorld + 1, maxBooks + 1];
        teleporterObjects = new GameObject[maxWorld + 1, maxTeleporters + 1];
    }

}
