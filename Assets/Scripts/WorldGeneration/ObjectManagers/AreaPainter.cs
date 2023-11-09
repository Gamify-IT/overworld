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
    [SerializeField] private Tile beachGroundTile;
    [SerializeField] private Tile beachWaterTile;
    [SerializeField] private Tile beachConnectionTile;

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
    [SerializeField] private Tile forestGroundTile;
    [SerializeField] private Tile forestWallTile;

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
        tileMapper.Add(TileSprite.BEACH_FLOOR, beachGroundTile);
        tileMapper.Add(TileSprite.BEACH_WATER, beachWaterTile);
        tileMapper.Add(TileSprite.BEACH_CONNECTION, beachConnectionTile);

        //Cave tiles
        tileMapper.Add(TileSprite.CAVE_FLOOR, caveGround);

        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_LEFT, caveWallBottomLeft);
        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_MID, caveWallBottomMid);
        tileMapper.Add(TileSprite.CAVE_WALL_BOTTOM_RIGHT, caveWallBottomRight);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_LEFT, caveWallTopLeft);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_MID, caveWallTopMid);
        tileMapper.Add(TileSprite.CAVE_WALL_TOP_RIGHT, caveWallTopRight);

        tileMapper.Add(TileSprite.CAVE_VOID_BOTTOM_LEFT, caveVoidBottomLeft);
        tileMapper.Add(TileSprite.CAVE_VOID_BOTTOM_MID, caveVoidBottomMid);
        tileMapper.Add(TileSprite.CAVE_VOID_BOTTOM_RIGHT, caveVoidBottomRight);
        tileMapper.Add(TileSprite.CAVE_VOID_MID_LEFT, caveVoidMidLeft);
        tileMapper.Add(TileSprite.CAVE_VOID_MID_MID, caveVoidMidMid);
        tileMapper.Add(TileSprite.CAVE_VOID_MID_RIGHT, caveVoidMidRight);
        tileMapper.Add(TileSprite.CAVE_VOID_TOP_LEFT, caveVoidTopLeft);
        tileMapper.Add(TileSprite.CAVE_VOID_TOP_MID, caveVoidTopMid);
        tileMapper.Add(TileSprite.CAVE_VOID_TOP_RIGHT, caveVoidTopRight);

        //Forest tiles
        tileMapper.Add(TileSprite.FOREST_FLOOR, forestGroundTile);
        tileMapper.Add(TileSprite.FOREST_TREE, forestWallTile);
    }

    public void Paint(TileSprite[,] layout, Vector2Int offset)
    {
        ClearTilemaps();

        Vector2Int size = new Vector2Int(layout.GetLength(0), layout.GetLength(1));
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                Vector3Int position = new Vector3Int(x + offset.x, y + offset.y, 0);

                Tilemap tilemap;

                string sprite = layout[x, y].ToString();

                if(sprite.Contains("FLOOR") || sprite.Contains("CONNECTION"))
                {
                    tilemap = ground;
                }
                else
                {
                    tilemap = wall;
                }

                TileBase tile = tileMapper[layout[x, y]];
                tilemap.SetTile(position, tile);
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
