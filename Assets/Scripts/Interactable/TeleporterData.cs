using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeleporterData : IGameEntityData
{
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

    public TeleporterData(string name, int worldID, int dungeonID,  int number, Vector2 position, bool isUnlocked)
    {
        this.teleporterName = name;
        this.worldID = worldID;
        this.dungeonID = dungeonID;
        this.teleporterNumber = number;
        this.position = position;
        this.isUnlocked = isUnlocked;
    }
}
