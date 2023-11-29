using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SeedLogger
{
    private static string basePath = "Assets/Resources/AreaLogs/Area";

    public static void LogArea(CustomAreaMapData areaData)
    {
        CustomAreaMapDTO areaDTO = CustomAreaMapDTO.ConvertDataToDto(areaData);
        string json = JsonUtility.ToJson(areaDTO, true);

        int index = GetNextIndex();
        Debug.Log("Logging with index: " + index);
        string path = basePath + index + ".json";

        using (FileStream fs = File.Create(path))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private static int GetNextIndex()
    {
        int index = 0;

        while(true)
        {
            if(!File.Exists(basePath + index + ".json"))
            {
                return index;
            }
            index++;
        }
    }
}
