/// <summary>
///     This enum contains the precise sprite for a tile for each area style, environmental object and dungeon design
/// </summary>
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
    CAVE_VOID,
    CAVE_FLOOR_DECORATION,

    //Beach tiles
    BEACH_FLOOR_TOP_LEFT,
    BEACH_FLOOR_TOP_MID,
    BEACH_FLOOR_TOP_RIGHT,
    BEACH_FLOOR_MID_LEFT,
    BEACH_FLOOR_MID_MID,
    BEACH_FLOOR_MID_RIGHT,
    BEACH_FLOOR_BOTTOM_LEFT,
    BEACH_FLOOR_BOTTOM_MID,
    BEACH_FLOOR_BOTTOM_RIGHT,
    BEACH_FLOOR_INNER_CORNER_TOP_LEFT,
    BEACH_FLOOR_INNER_CORNER_TOP_RIGHT,
    BEACH_FLOOR_INNER_CORNER_BOTTOM_LEFT,
    BEACH_FLOOR_INNER_CORNER_BOTTOM_RIGHT,
    BEACH_WATER,
    BEACH_WATER_WAVE,
    BEACH_CONNECTION_TOP_LEFT,
    BEACH_CONNECTION_TOP_MID,
    BEACH_CONNECTION_TOP_RIGHT,
    BEACH_CONNECTION_MID_LEFT,
    BEACH_CONNECTION_MID_MID,
    BEACH_CONNECTION_MID_RIGHT,
    BEACH_CONNECTION_BOTTOM_LEFT,
    BEACH_CONNECTION_BOTTOM_MID,
    BEACH_CONNECTION_BOTTOM_RIGHT,
    BEACH_CONNECTION_INNER_CORNER_TOP_LEFT,
    BEACH_CONNECTION_INNER_CORNER_TOP_RIGHT,
    BEACH_CONNECTION_INNER_CORNER_BOTTOM_LEFT,
    BEACH_CONNECTION_INNER_CORNER_BOTTOM_RIGHT,
    BEACH_WATER_DECORATION,

    //Forest tiles
    FOREST_GRASS,
    FOREST_TREE,
    FOREST_FLOOR_DECORATION,

    //Savanna tiles
    SAVANNA_FLOOR_BOTTOM_LEFT_WALL,
    SAVANNA_FLOOR_BOTTOM_LEFT_WATER,
    SAVANNA_FLOOR_BOTTOM_MID,
    SAVANNA_FLOOR_BOTTOM_RIGHT_WALL,
    SAVANNA_FLOOR_BOTTOM_RIGHT_WATER,
    SAVANNA_FLOOR_MID_LEFT_WALL,
    SAVANNA_FLOOR_MID_LEFT_WATER,
    SAVANNA_FLOOR_MID_INNER_CORNER_LEFT,
    SAVANNA_FLOOR_MID_MID,
    SAVANNA_FLOOR_MID_INNER_CORNER_RIGHT,
    SAVANNA_FLOOR_MID_RIGHT_WALL,
    SAVANNA_FLOOR_MID_RIGHT_WATER,
    SAVANNA_FLOOR_TOP_LEFT,
    SAVANNA_FLOOR_TOP_MID,
    SAVANNA_FLOOR_TOP_RIGHT,
    SAVANNA_WALL_BOTTOM_LEFT,
    SAVANNA_WALL_BOTTOM_MID,
    SAVANNA_WALL_BOTTOM_RIGHT,
    SAVANNA_WALL_TOP_LEFT_WALL,
    SAVANNA_WALL_TOP_LEFT_WATER,
    SAVANNA_WALL_TOP_MID,
    SAVANNA_WALL_TOP_RIGHT_WALL,
    SAVANNA_WALL_TOP_RIGHT_WATER,
    SAVANNA_WATER,
    SAVANNA_WATER_WAVE,
    SAVANNA_FLOOR_DECORATION,
    SAVANNA_WATER_DECORATION,

    //Objects
    STONE_SMALL,
    STONE_BIG,
    BUSH,
    SAVANNA_BUSH,
    TREE,
    SAVANNA_TREE,
    TREE_STUMP,
    FENCE,
    HOUSE_SMALL,
    HOUSE_BIG,
    BARREL,
    LOG,
    GRAVE,

    //Dungeon Entrance
    CAVE_ENTRANCE,
    GATE,
    TRAPDOOR,
    HOUSE
}