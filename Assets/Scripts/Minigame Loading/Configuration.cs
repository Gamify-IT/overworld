using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Configuration
{
    public long id;
    public String staticWorldId;
    public String configurationString;
    public String minigameType;

    public Configuration(String staticWorldId, String configurationString, String minigameType)
    {
        this.staticWorldId = staticWorldId;
        this.configurationString = configurationString;
        this.minigameType = minigameType;
    }

    public Configuration()
    {

    }

    public static Configuration CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Configuration>(jsonString);
    }
}

