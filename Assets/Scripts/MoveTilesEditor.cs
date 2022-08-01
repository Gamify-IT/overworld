using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
public class MoveTilesEditor : EditorWindow
{
    [MenuItem("Window/Custom/MoveTiles")]
    public static void ShowWindow()
    {
        GetWindow<MoveTilesEditor>("MoveTiles");
    }

    public Tilemap tilemap;
    public TileBase tileBase;
    public Vector3 translation;

    void OnGUI()
    {
        tilemap = (Tilemap) (EditorGUILayout.ObjectField(tilemap, typeof(Tilemap), true));
        tileBase = (TileBase) (EditorGUILayout.ObjectField(tileBase, typeof(TileBase), true));
        translation = EditorGUILayout.Vector3Field("translation: ", translation);

        if (GUILayout.Button("Execute")) {
            Vector3Int size = tilemap.size;
            Vector3Int origin = tilemap.origin;

            for (int x = origin.x; x < origin.x + size.x; x++) {
                for (int y = origin.y; y < origin.y + size.y; y++) {
                    TileBase tileBase = tilemap.GetTile(new Vector3Int(x, y));
                    if (tileBase == this.tileBase) {
                        Debug.Log("found tile at " + x + ", " + y);
                        tilemap.SetTransformMatrix(new Vector3Int(x, y), Matrix4x4.TRS(translation, Quaternion.Euler(0f, 0f, 0f), Vector3.one));
                    }
                }
            }
        }
    }
}
#endif