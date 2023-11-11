using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaPainter : MonoBehaviour
{
    #region Attributes
    [Header("Layers")]
    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap groundDecorations;
    [SerializeField] private Tilemap wall;
    [SerializeField] private Tilemap wallDecorations;
    [SerializeField] private Tilemap objects;

    [Space(10)]

    [Header("Tiles")]

    //Savanna tiles

    //Floor tiles
    [SerializeField] private Tile savannaFloorBottomLeftWall;
    [SerializeField] private Tile savannaFloorBottomLeftWater;
    [SerializeField] private Tile savannaFloorBottomMid;
    [SerializeField] private Tile savannaFloorBottomRightWall;
    [SerializeField] private Tile savannaFloorBottomRightWater;
    [SerializeField] private Tile savannaFloorMidLeftWall;
    [SerializeField] private Tile savannaFloorMidLeftWater;
    [SerializeField] private Tile savannaFloorMidInnerCornerLeft;
    [SerializeField] private Tile savannaFloorMidMid;
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

    [Space(10)]

    //Beach tiles

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

    [Space(10)]

    //Cave tiles

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
    [SerializeField] private Tile caveVoidBottomLeft;
    [SerializeField] private Tile caveVoidBottomMid;
    [SerializeField] private Tile caveVoidBottomRight;
    [SerializeField] private Tile caveVoidMidLeft;
    [SerializeField] private Tile caveVoidMidMid;
    [SerializeField] private Tile caveVoidMidRight;
    [SerializeField] private Tile caveVoidTopLeft;
    [SerializeField] private Tile caveVoidTopMid;
    [SerializeField] private Tile caveVoidTopRight;

    [Space(10)]

    //Forest tiles
    [SerializeField] private Tile forestFloor;
    [SerializeField] private RuleTile forestTree;

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

        //Cave tiles
        tileMapper.Add(TileSprite.CAVE_FLOOR, caveGround);

        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_LEFT, caveWallBottomLeft);
        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_MID, caveWallBottomMid);
        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_RIGHT, caveWallBottomRight);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_LEFT, caveWallTopLeft);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_MID, caveWallTopMid);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_RIGHT, caveWallTopRight);

        tileMapper.Add(TileSprite.CAVE_VOID, caveVoid);

        //Forest tiles
        tileMapper.Add(TileSprite.FOREST_GRASS, forestFloor);
        tileMapper.Add(TileSprite.FOREST_TREE, forestTree);
    }

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
                    ground.SetTile(position, floorTile);
                }

                //paint floor decoration
                if (layout[x, y, 1] != TileSprite.UNDEFINED)
                {
                    TileBase floorDecorationTile = tileMapper[layout[x, y, 1]];
                    groundDecorations.SetTile(position, floorDecorationTile);
                }

                //paint wall
                if (layout[x, y, 2] != TileSprite.UNDEFINED)
                {
                    TileBase wallTile = tileMapper[layout[x, y, 2]];
                    wall.SetTile(position, wallTile);
                }

                //paint wall decoration
                if (layout[x, y, 3] != TileSprite.UNDEFINED)
                {
                    TileBase wallDecorationTile = tileMapper[layout[x, y, 3]];
                    wallDecorations.SetTile(position, wallDecorationTile);
                }

                //paint objects
                if (layout[x, y, 4] != TileSprite.UNDEFINED)
                {
                    TileBase objectsTile = tileMapper[layout[x, y, 4]];
                    objects.SetTile(position, objectsTile);
                }
            }
        }
    }

    /// <summary>
    ///     This function removes all tiles of the tilemaps
    /// </summary>
    private void ClearTilemaps()
    {
        ground.ClearAllTiles();
        groundDecorations.ClearAllTiles();
        wall.ClearAllTiles();
        wallDecorations.ClearAllTiles();
        objects.ClearAllTiles();
    }
}
