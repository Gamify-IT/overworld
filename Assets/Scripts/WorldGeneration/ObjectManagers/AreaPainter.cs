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
    [SerializeField] private Tile savannaGroundTile;
    [SerializeField] private Tile savannaWallTile;
    [SerializeField] private Tile savannaWaterTile;

    [Space(10)]

    [SerializeField] private Tile beachGroundTile;
    [SerializeField] private Tile beachWaterTile;
    [SerializeField] private Tile beachConnectionTile;

    [Space(10)]

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

    [SerializeField] private Tile forestGroundTile;
    [SerializeField] private Tile forestWallTile;

    //Mapper
    private Dictionary<TileSprite, TileBase> tileMapper = new Dictionary<TileSprite, TileBase>();
    #endregion

    private void Awake()
    {
        //Savanna tiles
        tileMapper.Add(TileSprite.SAVANNA_FLOOR, savannaGroundTile);
        tileMapper.Add(TileSprite.SAVANNA_WALL, savannaWallTile);
        tileMapper.Add(TileSprite.SAVANNA_WATER, savannaWaterTile);

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
                
                switch(layout[x,y])
                {
                    case TileSprite.CAVE_FLOOR:
                    case TileSprite.BEACH_FLOOR:
                    case TileSprite.BEACH_CONNECTION:
                    case TileSprite.FOREST_FLOOR:
                    case TileSprite.SAVANNA_FLOOR:
                        tilemap = ground;
                        break;

                    case TileSprite.CAVE_WALL_BOTTOM_LEFT:
                    case TileSprite.CAVE_WALL_BOTTOM_MID:
                    case TileSprite.CAVE_WALL_BOTTOM_RIGHT:
                    case TileSprite.CAVE_WALL_TOP_LEFT:
                    case TileSprite.CAVE_WALL_TOP_MID:
                    case TileSprite.CAVE_WALL_TOP_RIGHT:
                    case TileSprite.CAVE_VOID_BOTTOM_LEFT:
                    case TileSprite.CAVE_VOID_BOTTOM_MID:
                    case TileSprite.CAVE_VOID_BOTTOM_RIGHT:
                    case TileSprite.CAVE_VOID_MID_LEFT:
                    case TileSprite.CAVE_VOID_MID_MID:
                    case TileSprite.CAVE_VOID_MID_RIGHT:
                    case TileSprite.CAVE_VOID_TOP_LEFT:
                    case TileSprite.CAVE_VOID_TOP_MID:
                    case TileSprite.CAVE_VOID_TOP_RIGHT:
                    case TileSprite.BEACH_WATER:
                    case TileSprite.FOREST_TREE:
                    case TileSprite.SAVANNA_WALL:
                    case TileSprite.SAVANNA_WATER:
                        tilemap = wall;
                        break;

                    default:
                        tilemap = ground;
                        break;
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
