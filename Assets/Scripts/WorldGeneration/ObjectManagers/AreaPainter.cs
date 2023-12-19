using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
///     This class is used to paint a layout by placing the provided sprites at the respective tilemap
/// </summary>
public class AreaPainter : MonoBehaviour
{
    #region Attributes
    [Header("Layers")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap groundDecorationsTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap wallDecorationsTilemap;
    [SerializeField] private Tilemap objectsTilemap;
    [SerializeField] private List<Tilemap> additionalLayers;

    [Space(10)]

    [Header("Savanna Tiles")]

    //Floor tiles
    [SerializeField] private Tile savannaFloorBottomLeftWall;
    [SerializeField] private Tile savannaFloorBottomLeftWater;
    [SerializeField] private Tile savannaFloorBottomMid;
    [SerializeField] private Tile savannaFloorBottomRightWall;
    [SerializeField] private Tile savannaFloorBottomRightWater;
    [SerializeField] private Tile savannaFloorMidLeftWall;
    [SerializeField] private Tile savannaFloorMidLeftWater;
    [SerializeField] private Tile savannaFloorMidInnerCornerLeft;
    [SerializeField] private RuleTile savannaFloorMidMid;
    [SerializeField] private Tile savannaFloorMidInnerCornerRight;
    [SerializeField] private Tile savannaFloorMidRightWall;
    [SerializeField] private Tile savannaFloorMidRightWater;
    [SerializeField] private Tile savannaFloorTopLeft;
    [SerializeField] private Tile savannaFloorTopMid;
    [SerializeField] private Tile savannaFloorTopRight;

    //Wall tiles
    [SerializeField] private Tile savannaWallBottomLeft;
    [SerializeField] private Tile savannaWallBottomMid;
    [SerializeField] private Tile savannaWallBottomRight;
    [SerializeField] private Tile savannaWallTopLeft;
    [SerializeField] private Tile savannaWallTopLeftWater;
    [SerializeField] private Tile savannaWallTopMid;
    [SerializeField] private Tile savannaWallTopRight;
    [SerializeField] private Tile savannaWallTopRightWater;

    //Water tiles
    [SerializeField] private Tile savannaWater;
    [SerializeField] private RuleTile savannaWaterWave;

    //Decoration
    [SerializeField] private RuleTile savannaFloorDecoration;
    [SerializeField] private RuleTile savannaWaterDecoration;

    [Space(10)]

    [Header("Beach Tiles")]

    //Floor tiles
    [SerializeField] private Tile beachGroundTopLeft;
    [SerializeField] private Tile beachGroundTopMid;
    [SerializeField] private Tile beachGroundTopRight;
    [SerializeField] private Tile beachGroundMidLeft;
    [SerializeField] private Tile beachGroundMidMid;
    [SerializeField] private Tile beachGroundMidRight;
    [SerializeField] private Tile beachGroundBottomLeft;
    [SerializeField] private Tile beachGroundBottomMid;
    [SerializeField] private Tile beachGroundBottomRight;
    [SerializeField] private Tile beachGroundInnerCornerTopLeft;
    [SerializeField] private Tile beachGroundInnerCornerTopRight;
    [SerializeField] private Tile beachGroundInnerCornerBottomLeft;
    [SerializeField] private Tile beachGroundInnerCornerBottomRight;

    //Water tile
    [SerializeField] private Tile beachWaterTile;
    [SerializeField] private RuleTile beachWaterWave;

    //Connection tiles
    [SerializeField] private Tile beachConnectionTopLeft;
    [SerializeField] private Tile beachConnectionTopMid;
    [SerializeField] private Tile beachConnectionTopRight;
    [SerializeField] private Tile beachConnectionMidLeft;
    [SerializeField] private Tile beachConnectionMidMid;
    [SerializeField] private Tile beachConnectionMidRight;
    [SerializeField] private Tile beachConnectionBottomLeft;
    [SerializeField] private Tile beachConnectionBottomMid;
    [SerializeField] private Tile beachConnectionBottomRight;
    [SerializeField] private Tile beachConnectionInnerCornerTopLeft;
    [SerializeField] private Tile beachConnectionInnerCornerTopRight;
    [SerializeField] private Tile beachConnectionInnerCornerBottomLeft;
    [SerializeField] private Tile beachConnectionInnerCornerBottomRight;

    //Decoration
    [SerializeField] private RuleTile beachWaterDecoration;

    [Space(10)]

    [Header("Cave Tiles")]

    //Floor tiles
    [SerializeField] private Tile caveGround;

    //Wall tiles
    [SerializeField] private Tile caveWallBottomLeft;
    [SerializeField] private Tile caveWallBottomMid;
    [SerializeField] private Tile caveWallBottomRight;
    [SerializeField] private Tile caveWallTopLeft;
    [SerializeField] private Tile caveWallTopMid;
    [SerializeField] private Tile caveWallTopRight;

    //Void tiles
    [SerializeField] private RuleTile caveVoid;

    //Decoration
    [SerializeField] private RuleTile caveFloorDecoration;

    [Space(10)]

    [Header("Forest Tiles")]

    [SerializeField] private RuleTile forestFloor;
    [SerializeField] private RuleTile forestTree;

    //Decoration
    [SerializeField] private RuleTile forestFloorDecoration;

    [Space(10)]

    [Header("Object Tiles")]

    [SerializeField] private RuleTile stoneSmall;
    [SerializeField] private RuleTile stoneBig;
    [SerializeField] private RuleTile bush;
    [SerializeField] private RuleTile savannaBush;
    [SerializeField] private RuleTile tree;
    [SerializeField] private RuleTile savannaTree;
    [SerializeField] private RuleTile treeStump;
    [SerializeField] private RuleTile fence;
    [SerializeField] private RuleTile houseSmall;
    [SerializeField] private RuleTile houseBig;
    [SerializeField] private RuleTile barrel;
    [SerializeField] private RuleTile log;
    [SerializeField] private RuleTile grave;

    [Space(10)]

    [Header("Dungeon Entrance Tiles")]
    [SerializeField] private RuleTile caveEntrance;
    [SerializeField] private RuleTile gate;
    [SerializeField] private RuleTile trapdoor;
    [SerializeField] private RuleTile house;

    //Mapper
    private Dictionary<TileSprite, TileBase> tileMapper = new Dictionary<TileSprite, TileBase>();
    #endregion

    private void Awake()
    {
        //Savanna tiles
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_BOTTOM_LEFT_WALL, savannaFloorBottomLeftWall);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_BOTTOM_LEFT_WATER, savannaFloorBottomLeftWater);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_BOTTOM_MID, savannaFloorBottomMid);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_BOTTOM_RIGHT_WALL, savannaFloorBottomRightWall);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_BOTTOM_RIGHT_WATER, savannaFloorBottomRightWater);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_LEFT_WALL, savannaFloorMidLeftWall);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_LEFT_WATER, savannaFloorMidLeftWater);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_INNER_CORNER_LEFT, savannaFloorMidInnerCornerLeft);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_MID, savannaFloorMidMid);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_INNER_CORNER_RIGHT, savannaFloorMidInnerCornerRight);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_RIGHT_WALL, savannaFloorMidRightWall);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_MID_RIGHT_WATER, savannaFloorMidRightWater);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_TOP_LEFT, savannaFloorTopLeft);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_TOP_MID, savannaFloorTopMid);
        tileMapper.Add(TileSprite.SAVANNA_FLOOR_TOP_RIGHT, savannaFloorTopRight);

        tileMapper.Add(TileSprite.SAVANNA_WALL_BOTTOM_LEFT, savannaWallBottomLeft);
        tileMapper.Add(TileSprite.SAVANNA_WALL_BOTTOM_MID, savannaWallBottomMid);
        tileMapper.Add(TileSprite.SAVANNA_WALL_BOTTOM_RIGHT, savannaWallBottomRight);
        tileMapper.Add(TileSprite.SAVANNA_WALL_TOP_LEFT_WALL, savannaWallTopLeft);
        tileMapper.Add(TileSprite.SAVANNA_WALL_TOP_LEFT_WATER, savannaWallTopLeftWater);
        tileMapper.Add(TileSprite.SAVANNA_WALL_TOP_MID, savannaWallTopMid);
        tileMapper.Add(TileSprite.SAVANNA_WALL_TOP_RIGHT_WALL, savannaWallTopRight);
        tileMapper.Add(TileSprite.SAVANNA_WALL_TOP_RIGHT_WATER, savannaWallTopRightWater);

        tileMapper.Add(TileSprite.SAVANNA_WATER, savannaWater);
        tileMapper.Add(TileSprite.SAVANNA_WATER_WAVE, savannaWaterWave);

        tileMapper.Add(TileSprite.SAVANNA_FLOOR_DECORATION, savannaFloorDecoration);
        tileMapper.Add(TileSprite.SAVANNA_WATER_DECORATION, savannaWaterDecoration);

        //Beach tiles
        tileMapper.Add(TileSprite.BEACH_FLOOR_BOTTOM_LEFT, beachGroundBottomLeft);
        tileMapper.Add(TileSprite.BEACH_FLOOR_BOTTOM_MID, beachGroundBottomMid);
        tileMapper.Add(TileSprite.BEACH_FLOOR_BOTTOM_RIGHT, beachGroundBottomRight);
        tileMapper.Add(TileSprite.BEACH_FLOOR_MID_LEFT, beachGroundMidLeft);
        tileMapper.Add(TileSprite.BEACH_FLOOR_MID_MID, beachGroundMidMid);
        tileMapper.Add(TileSprite.BEACH_FLOOR_MID_RIGHT, beachGroundMidRight);
        tileMapper.Add(TileSprite.BEACH_FLOOR_TOP_LEFT, beachGroundTopLeft);
        tileMapper.Add(TileSprite.BEACH_FLOOR_TOP_MID, beachGroundTopMid);
        tileMapper.Add(TileSprite.BEACH_FLOOR_TOP_RIGHT, beachGroundTopRight);
        tileMapper.Add(TileSprite.BEACH_FLOOR_INNER_CORNER_BOTTOM_LEFT, beachGroundInnerCornerBottomLeft);
        tileMapper.Add(TileSprite.BEACH_FLOOR_INNER_CORNER_BOTTOM_RIGHT, beachGroundInnerCornerBottomRight);
        tileMapper.Add(TileSprite.BEACH_FLOOR_INNER_CORNER_TOP_LEFT, beachGroundInnerCornerTopLeft);
        tileMapper.Add(TileSprite.BEACH_FLOOR_INNER_CORNER_TOP_RIGHT, beachGroundInnerCornerTopRight);

        tileMapper.Add(TileSprite.BEACH_WATER, beachWaterTile);
        tileMapper.Add(TileSprite.BEACH_WATER_WAVE, beachWaterWave);

        tileMapper.Add(TileSprite.BEACH_CONNECTION_BOTTOM_LEFT, beachConnectionBottomLeft);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_BOTTOM_MID, beachConnectionBottomMid);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_BOTTOM_RIGHT, beachConnectionBottomRight);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_MID_LEFT, beachConnectionMidLeft);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_MID_MID, beachConnectionMidMid);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_MID_RIGHT, beachConnectionMidRight);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_TOP_LEFT, beachConnectionTopLeft);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_TOP_MID, beachConnectionTopMid);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_TOP_RIGHT, beachConnectionTopRight);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_INNER_CORNER_BOTTOM_LEFT, beachConnectionInnerCornerBottomLeft);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_INNER_CORNER_BOTTOM_RIGHT, beachConnectionInnerCornerBottomRight);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_INNER_CORNER_TOP_LEFT, beachConnectionInnerCornerTopLeft);
        tileMapper.Add(TileSprite.BEACH_CONNECTION_INNER_CORNER_TOP_RIGHT, beachConnectionInnerCornerTopRight);

        tileMapper.Add(TileSprite.BEACH_WATER_DECORATION, beachWaterDecoration);

        //Cave tiles
        tileMapper.Add(TileSprite.CAVE_FLOOR, caveGround);

        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_LEFT, caveWallBottomLeft);
        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_MID, caveWallBottomMid);
        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_RIGHT, caveWallBottomRight);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_LEFT, caveWallTopLeft);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_MID, caveWallTopMid);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_RIGHT, caveWallTopRight);

        tileMapper.Add(TileSprite.CAVE_VOID, caveVoid);

        tileMapper.Add(TileSprite.CAVE_FLOOR_DECORATION, caveFloorDecoration);

        //Forest tiles
        tileMapper.Add(TileSprite.FOREST_GRASS, forestFloor);
        tileMapper.Add(TileSprite.FOREST_TREE, forestTree);
        tileMapper.Add(TileSprite.FOREST_FLOOR_DECORATION, forestFloorDecoration);

        //Object tiles
        tileMapper.Add(TileSprite.STONE_SMALL, stoneSmall);
        tileMapper.Add(TileSprite.STONE_BIG, stoneBig);
        tileMapper.Add(TileSprite.BUSH, bush);
        tileMapper.Add(TileSprite.SAVANNA_BUSH, savannaBush);
        tileMapper.Add(TileSprite.TREE, tree);
        tileMapper.Add(TileSprite.SAVANNA_TREE, savannaTree);
        tileMapper.Add(TileSprite.TREE_STUMP, treeStump);
        tileMapper.Add(TileSprite.FENCE, fence);
        tileMapper.Add(TileSprite.HOUSE_SMALL, houseSmall);
        tileMapper.Add(TileSprite.HOUSE_BIG, houseBig);
        tileMapper.Add(TileSprite.BARREL, barrel);
        tileMapper.Add(TileSprite.LOG, log);
        tileMapper.Add(TileSprite.GRAVE, grave);

        //dungeon entrance tiles
        tileMapper.Add(TileSprite.CAVE_ENTRANCE, caveEntrance);
        tileMapper.Add(TileSprite.GATE, gate);
        tileMapper.Add(TileSprite.TRAPDOOR, trapdoor);
        tileMapper.Add(TileSprite.HOUSE, house);
    }

    /// <summary>
    ///     This function paints the given layout at the tilemaps, with the given offset
    /// </summary>
    /// <param name="layout">The layout to paint</param>
    /// <param name="offset">The offset in the tilemap</param>
    public void Paint(TileSprite[,,] layout, Vector2Int offset)
    {
        ClearTilemaps();

        Vector2Int size = new Vector2Int(layout.GetLength(0), layout.GetLength(1));
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                Vector3Int position = new Vector3Int(x + offset.x, y + offset.y, 0);

                //paint floor
                if(layout[x,y,0] != TileSprite.UNDEFINED)
                {
                    TileBase floorTile = tileMapper[layout[x, y, 0]];
                    groundTilemap.SetTile(position, floorTile);
                }

                //paint floor decoration
                if (layout[x, y, 1] != TileSprite.UNDEFINED)
                {
                    TileBase floorDecorationTile = tileMapper[layout[x, y, 1]];
                    groundDecorationsTilemap.SetTile(position, floorDecorationTile);
                }

                //paint wall
                if (layout[x, y, 2] != TileSprite.UNDEFINED)
                {
                    TileBase wallTile = tileMapper[layout[x, y, 2]];
                    wallTilemap.SetTile(position, wallTile);
                }

                //paint wall decoration
                if (layout[x, y, 3] != TileSprite.UNDEFINED)
                {
                    TileBase wallDecorationTile = tileMapper[layout[x, y, 3]];
                    wallDecorationsTilemap.SetTile(position, wallDecorationTile);
                }

                //paint objects
                if (layout[x, y, 4] != TileSprite.UNDEFINED)
                {
                    TileBase objectsTile = tileMapper[layout[x, y, 4]];
                    objectsTilemap.SetTile(position, objectsTile);
                }
            }
        }
    }

    /// <summary>
    ///     This function removes all tiles of the tilemaps
    /// </summary>
    private void ClearTilemaps()
    {
        groundTilemap.ClearAllTiles();
        groundDecorationsTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        wallDecorationsTilemap.ClearAllTiles();
        objectsTilemap.ClearAllTiles();

        foreach(Tilemap tilemap in additionalLayers)
        {
            tilemap.ClearAllTiles();
        }
    }
}
