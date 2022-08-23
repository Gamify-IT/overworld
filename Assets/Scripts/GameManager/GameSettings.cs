using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    private static int maxWorld = 5;
    private static int maxMinigames = 13;
    private static int maxNPCs = 11;
    private static int maxDungeons = 5;

    public static int getMaxWorlds()
    {
        return maxWorld;
    }

    public static int getMaxMinigames()
    {
        return maxMinigames;
    }

    public static int getMaxNPCs()
    {
        return maxNPCs;
    }

    public static int getMaxDungeons()
    {
        return maxDungeons;
    }
}
