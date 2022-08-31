using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This file stores the game settings.
/// </summary>
public static class GameSettings
{
    #region Attributes
    private static int maxWorld = 4;
    private static int maxMinigames = 12;
    private static int maxNPCs = 10;
    private static int maxDungeons = 4;
    #endregion

    #region GetterAndSetter
    /// <summary>
    /// This function returns the maximum amount of worlds.
    /// </summary>
    /// <returns>The maximum amount of worlds</returns>
    public static int getMaxWorlds()
    {
        return maxWorld;
    }

    /// <summary>
    /// This function returns the maximum amount of minigames in an area.
    /// </summary>
    /// <returns>The maximum amount of minigames in an area</returns>
    public static int getMaxMinigames()
    {
        return maxMinigames;
    }

    /// <summary>
    /// This function returns the maximum amount of NPCs in an area.
    /// </summary>
    /// <returns>The maximum amount of NPCs in an area</returns>
    public static int getMaxNPCs()
    {
        return maxNPCs;
    }

    /// <summary>
    /// This function returns the maximum amount of dungeons in a world.
    /// </summary>
    /// <returns>The maximum amount of dungeons in a world</returns>
    public static int getMaxDungeons()
    {
        return maxDungeons;
    }
    #endregion
}
