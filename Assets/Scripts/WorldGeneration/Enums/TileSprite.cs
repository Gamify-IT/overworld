using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileSprite
{
    UNDEFINED,

    //Cave tiles
    CAVE_FLOOR,
    CAVE_WALL_BOTTOM_LEFT,
    CAVE_WALL_BOTTOM_MID,
    CAVE_WALL_BOTTOM_RIGHT,
    CAVE_WALL_TOP_LEFT,
    CAVE_WALL_TOP_MID,
    CAVE_WALL_TOP_RIGHT,
    CAVE_VOID_BOTTOM_LEFT,
    CAVE_VOID_BOTTOM_MID,
    CAVE_VOID_BOTTOM_RIGHT,
    CAVE_VOID_MID_LEFT,
    CAVE_VOID_MID_MID,
    CAVE_VOID_MID_RIGHT,
    CAVE_VOID_TOP_LEFT,
    CAVE_VOID_TOP_MID,
    CAVE_VOID_TOP_RIGHT,

    //Beach tiles
    BEACH_FLOOR,
    BEACH_WATER,
    BEACH_CONNECTION,

    //Forest tiles
    FOREST_FLOOR,
    FOREST_TREE,

    //Savanna tiles
    SAVANNA_FLOOR,
    SAVANNA_WALL,
    SAVANNA_WATER
}
