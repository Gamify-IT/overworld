using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaPainter : MonoBehaviour
{
    #region Attributes
    //Layers
    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap groundDecorations;
    [SerializeField] private Tilemap wall;
    [SerializeField] private Tilemap wallDecorations;
    [SerializeField] private Tilemap objects;

    //Tiles
    [SerializeField] private Tile savannaGroundTile;
    [SerializeField] private Tile savannaWallTile;

    [SerializeField] private Tile beachGroundTile;
    [SerializeField] private RuleTile beachWallTile;

    [SerializeField] private Tile caveGroundTile;
    [SerializeField] private Tile caveWallTile;

    [SerializeField] private Tile forestGroundTile;
    [SerializeField] private Tile forestWallTile;

    //Mapper
    private Dictionary<TileType, TileBase> tileMapper = new Dictionary<TileType, TileBase>();
    #endregion

    private void Awake()
    {
        tileMapper.Add(TileType.SAVANNA_FLOOR, savannaGroundTile);
        tileMapper.Add(TileType.SAVANNA_WALL, savannaWallTile);
        tileMapper.Add(TileType.BEACH_FLOOR, beachGroundTile);
        tileMapper.Add(TileType.BEACH_WATER, beachWallTile);
        tileMapper.Add(TileType.CAVE_FLOOR, caveGroundTile);
        tileMapper.Add(TileType.CAVE_WALL, caveWallTile);
        tileMapper.Add(TileType.FOREST_FLOOR, forestGroundTile);
        tileMapper.Add(TileType.FOREST_TREE, forestWallTile);
    }

    public void Paint(TileType[,] layout, Vector2Int offset)
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
                    case TileType.CAVE_FLOOR:
                    case TileType.BEACH_FLOOR:
                    case TileType.BEACH_CONNECTION:
                    case TileType.FOREST_FLOOR:
                    case TileType.SAVANNA_FLOOR:
                        tilemap = ground;
                        break;

                    case TileType.CAVE_WALL:
                    case TileType.CAVE_VOID:
                    case TileType.BEACH_WATER:
                    case TileType.FOREST_TREE:
                    case TileType.SAVANNA_WALL:
                    case TileType.SAVANNA_WATER:
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
