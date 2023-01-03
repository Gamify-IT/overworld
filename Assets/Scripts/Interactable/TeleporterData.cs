using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeleporterData : IGameEntityData
{
    public string uuid { get; set; }
    public string teleporterName { get; set; }
    public int worldID { get; set; }
    public int dungeonID { get; set; }
    public int teleporterNumber { get; set; }
    public Vector2 position { get; set; }
    public bool isUnlocked { get; set; }

    public TeleporterData()
    {
        this.isUnlocked = false;
        this.teleporterName = "TP";
        this.position = Vector2.zero;
    }

    public TeleporterData(string uuid, string name, int worldID, int dungeonID,  int number, Vector2 position, bool isUnlocked)
    {
        this.uuid = uuid;
        this.teleporterName = name;
        this.worldID = worldID;
        this.dungeonID = dungeonID;
        this.teleporterNumber = number;
        this.position = position;
        this.isUnlocked = isUnlocked;
    }

    public static TeleporterData ConvertDtoToData(TeleporterDTO dto)
    {
        string uuid = dto.id;
        string name = dto.name;
        bool unlocked = false;
        Vector2 position = new Vector2(dto.position.x, dto.position.y);
        int worldID = dto.area.worldIndex;
        int dungeonID = dto.area.dungeonIndex;
        int number = dto.index;

        if (name.Length == 0)
        {
            name = "Some Teleporter";
        }
        TeleporterData data = new TeleporterData(uuid, name, worldID, dungeonID, number, position, unlocked);
        return data;
    }
}
