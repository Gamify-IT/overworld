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
    [SerializeField] private Tile beachWallTile;

    [SerializeField] private Tile caveGroundTile;
    [SerializeField] private Tile caveWallTile;

    [SerializeField] private Tile forestGroundTile;
    [SerializeField] private Tile forestWallTile;

    //Mapper
    private Dictionary<string, Tile> tileMapper = new Dictionary<string, Tile>();
    #endregion

    private void Awake()
    {
        tileMapper.Add("Overworld-Savanna_0", savannaGroundTile);
        tileMapper.Add("Overworld-Savanna_453", savannaWallTile);
        tileMapper.Add("Overworld_156", beachGroundTile);
        tileMapper.Add("Overworld_276", beachWallTile);
        tileMapper.Add("cave_0", caveGroundTile);
        tileMapper.Add("cave_12", caveWallTile);
        tileMapper.Add("Overworld_0", forestGroundTile);
        tileMapper.Add("Overworld_574", forestWallTile);
    }

    public void Paint(string[,,] layout, Vector2Int offset)
    {
        Vector2Int size = new Vector2Int(layout.GetLength(0), layout.GetLength(1));
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                Vector3Int position = new Vector3Int(x + offset.x, y + offset.y, 0);

                //paint ground layer
                if (tileMapper.ContainsKey(layout[x, y, 0]))
                {
                    Tile tile = tileMapper[layout[x, y, 0]];
                    ground.SetTile(position, tile);
                }
                else
                {
                    ground.SetTile(position, null);
                }

                //paint ground decoration layer
                if (tileMapper.ContainsKey(layout[x, y, 1]))
                {
                    Tile tile = tileMapper[layout[x, y, 1]];
                    groundDecorations.SetTile(position, tile);
                }
                else
                {
                    groundDecorations.SetTile(position, null);
                }

                //paint wall layer
                if (tileMapper.ContainsKey(layout[x, y, 2]))
                {
                    Tile tile = tileMapper[layout[x, y, 2]];
                    wall.SetTile(position, tile);
                }
                else
                {
                    wall.SetTile(position, null);
                }

                //paint wall decoration layer
                if (tileMapper.ContainsKey(layout[x, y, 3]))
                {
                    Tile tile = tileMapper[layout[x, y, 3]];
                    wallDecorations.SetTile(position, tile);
                }
                else
                {
                    wallDecorations.SetTile(position, null);
                }

                //paint objects layer
                if (tileMapper.ContainsKey(layout[x, y, 4]))
                {
                    Tile tile = tileMapper[layout[x, y, 4]];
                    objects.SetTile(position, tile);
                }
                else
                {
                    objects.SetTile(position, null);
                }
            }
        }
    }
}
