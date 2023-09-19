using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;

/// <summary>
///     This file stores the game settings.
/// </summary>
public static class GameSettings
{
    #region Attributes

    private static string overworldBackendPath;
    private static Gamemode gamemode;
    private static int maxWorld;
    private static int maxMinigames;
    private static int maxNPCs;
    private static int maxBooks;
    private static int maxDungeons;
    private static int maxTeleporters;

    #endregion

    public static async UniTask<bool> FetchValues()
    {
        string path = "GameSettings/GameSettings";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        GameSettingsDTO dto = GameSettingsDTO.CreateFromJSON(json);
        
        overworldBackendPath = dto.overworldBackendPath;
        maxWorld = dto.maxWorld;
        maxMinigames = dto.maxMinigames;
        maxNPCs = dto.maxNPCs;
        maxBooks = dto.maxBooks;
        maxDungeons = dto.maxDungeons;
        maxTeleporters = dto.maxTeleporters;

        return true;
    }

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