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
    [SerializeField] private Tile groundTile;
    [SerializeField] private Tile wallTile;

    //Mapper
    private Dictionary<string, Tile> tileMapper;
    #endregion

    private void Start()
    {
        tileMapper = new Dictionary<string, Tile>();
        tileMapper.Add("Overworld-Savanna_0", groundTile);
        tileMapper.Add("Overworld-Savanna_453", wallTile);
    }

    public void Paint(string[,,] layout, Vector2Int size, Vector2Int offset)
    {
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                Vector3Int position = new Vector3Int(x + offset.x, y + offset.y, 0);

                //paint ground layer
                //Debug.Log("Tile an Position (" + position.x + ", " + position.y + ", 0): " + layout[x, y, 0]);
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
                //Debug.Log("Tile an Position (" + position.x + ", " + position.y + ", 1): " + layout[x, y, 1]);
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
                //Debug.Log("Tile an Position (" + position.x + ", " + position.y + ", 2): " + layout[x, y, 2]);
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
                //Debug.Log("Tile an Position (" + position.x + ", " + position.y + ", 3): " + layout[x, y, 3]);
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
                //Debug.Log("Tile an Position (" + position.x + ", " + position.y + ", 4): " + layout[x, y, 4]);
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
