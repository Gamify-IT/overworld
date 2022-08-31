/// <summary>
///     This file stores the game settings.
/// </summary>
public static class GameSettings
{
    #region Attributes

    private static readonly int maxWorld = 4;
    private static readonly int maxMinigames = 12;
    private static readonly int maxNPCs = 10;
    private static readonly int maxDungeons = 4;

    #endregion

    #region GetterAndSetter

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
    ///     This function returns the maximum amount of dungeons in a world.
    /// </summary>
    /// <returns>The maximum amount of dungeons in a world</returns>
    public static int GetMaxDungeons()
    {
        return maxDungeons;
    }

    #endregion
}