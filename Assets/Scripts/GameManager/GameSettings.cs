/// <summary>
///     This file stores the game settings.
/// </summary>
public static class GameSettings
{
    #region Attributes

    private static readonly string overworldBackendPath = "/overworld/api/v1";
    private static Gamemode gamemode;
    private static readonly int maxWorld = 4;
    private static readonly int maxMinigames = 12;
    private static readonly int maxNPCs = 10;
    private static readonly int maxBooks = 5;
    private static readonly int maxDungeons = 4;
    private static readonly int maxTeleporters = 10;

    #endregion

    #region GetterAndSetter

    /// <summary>
    ///     This function returns the path of the overworld backend
    /// </summary>
    /// <returns>The overworld backend path</returns>
    public static string GetOverworldBackendPath()
    {
        return overworldBackendPath;
    }

    public static void SetGamemode(Gamemode newGamemode)
    {
        gamemode = newGamemode;
    }

    public static Gamemode GetGamemode()
    {
        return gamemode;
    }

    /// <summary>
    ///     This function returns the maximum amount of worlds.
    /// </summary>
    /// <returns>The maximum amount of worlds</returns>
    public static int GetMaxWorlds()
    {
        return maxWorld;
    }

    /// <summary>
    ///     This function returns the maximum amount of minigames in an area.
    /// </summary>
    /// <returns>The maximum amount of minigames in an area</returns>
    public static int GetMaxMinigames()
    {
        return maxMinigames;
    }

    /// <summary>
    ///     This function returns the maximum amount of NPCs in an area.
    /// </summary>
    /// <returns>The maximum amount of NPCs in an area</returns>
    public static int GetMaxNpCs()
    {
        return maxNPCs;
    }
    /// <summary>
    ///     This function returns the maximum amount of NPCs in an area.
    /// </summary>
    /// <returns>The maximum amount of NPCs in an area</returns>
    public static int GetMaxBooks()
    {
        return maxBooks;
    }
    /// <summary>
    ///     This function returns the maximum amount of dungeons in a world.
    /// </summary>
    /// <returns>The maximum amount of dungeons in a world</returns>
    public static int GetMaxDungeons()
    {
        return maxDungeons;
    }

    /// <summary>
    ///     This function returns the maximum amount of teleporters in a world (including its dungeons).
    /// </summary>
    /// <returns>The maximum amount of dungeons in a world</returns>
    public static int GetMaxTeleporters()
    {
        return maxTeleporters;
    }

    #endregion
}