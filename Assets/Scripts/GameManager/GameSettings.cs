using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    private static int maxWorld = 4;
    private static int maxMinigames = 12;
    private static int maxNPCs = 10;
    private static int maxDungeons = 4;

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
