using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private CellType roomType;
    private List<Vector2Int> tiles;
    private List<Vector2Int> borderTiles;
    
    public Room(CellType roomType)
    {
        this.roomType = roomType;
        tiles = new List<Vector2Int>();
        borderTiles = new List<Vector2Int>();
    }

    public void SetTiles(List<Vector2Int> tiles)
    {
        this.tiles = tiles;
    }

    public void SetBorderTiles(List<Vector2Int> borderTiles)
    {
        this.borderTiles = borderTiles;
    }

    public List<Vector2Int> GetTiles()
    {
        return tiles;
    }

    public List<Vector2Int> GetBorderTiles()
    {
        return borderTiles;
    }

    public CellType GetRoomType()
    {
        return roomType;
    }
}
