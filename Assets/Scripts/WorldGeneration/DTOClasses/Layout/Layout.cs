using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

/// <summary>
///     This class is used to allow retrieving a World Map Layout from Get Requests.
/// </summary>
[Serializable]
public class Layout
{
    public int sizeX;
    public int sizeY;
    public int layers;
    public string tiles;

    #region Constructors
    public Layout() { }

    public Layout(int sizeX, int sizeY, int layers, string tiles)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.layers = layers;
        this.tiles = tiles;
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>Layout</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>Layout</c> object containing the data</returns>
    public static Layout CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Layout>(jsonString);
    }

    /// <summary>
    ///     This function converts a three dimensional string array to a <c>Layout</c> object
    /// </summary>
    /// <param name="array">The three dimensional string array to be converted</param>
    /// <returns>The converted <c>Layout</c> object</returns>
    public static Layout ConvertArrayToLayout(string[,,] array)
    {
        int sizeX = array.GetLength(0);
        int sizeY = array.GetLength(1);
        int layers = array.GetLength(2);
        
        StringBuilder stringBuilder = new StringBuilder();
        
        for(int x=0; x<sizeX; x++)
        {
            for(int y=0; y<sizeY; y++)
            {
                for(int l=0; l<layers; l++)
                {
                    stringBuilder.Append(array[x, y, l] + ";");
                }
            }
        }
        //remove last ;
        stringBuilder.Remove(stringBuilder.ToString().Length - 1, 1);
        string tiles = stringBuilder.ToString();
        

        Layout layout = new Layout(sizeX, sizeY, layers, tiles);
        return layout;
    }

    /// <summary>
    ///     This function converts a <c>Layout</c> object to a three dimensional string array
    /// </summary>
    /// <param name="layout">The <c>Layout</c> object to be converted</param>
    /// <returns>The converted three dimensional string arry</returns>
    public static string[,,] ConvertLayoutToArray(Layout layout)
    {
        if(layout == null)
        {
            return new string[0, 0, 0];
        }

        if (layout.tiles == null)
        {
            return new string[0, 0, 0];
        }

        if (!layout.tiles.Contains(";"))
        {
            return new string[0, 0, 0];
        }

        string[,,] tiles = new string[layout.sizeX, layout.sizeY, layout.layers];

        string[] sprites = layout.tiles.Split(";");
        int posX = 0;
        int posY = 0;
        int layer = 0;

        Debug.Log("Sprites size: " + sprites.Length);

        for(int i=0; i<sprites.Length; i++)
        {
            //check current indizes
            if(posX >= layout.sizeX || posY >= layout.sizeY || layer >= layout.layers)
            {
                Debug.LogError("Error converting layout to array");
            }

            //store tile in array
            tiles[posX, posY, layer] = sprites[i];

            //get next array position
            layer++;
            if(layer >= layout.layers)
            {
                layer = 0;
                posY++;
                if(posY >= layout.sizeY)
                {
                    posY = 0;
                    posX++;
                }
            }
        }

        return tiles;
    }
}
