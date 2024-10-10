using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to retrieve <c>ShopItem<c> data from Get Requests.
/// </summary>
[Serializable]
public class ShopItem
{
    public int cost;
    public string imageName;
    public string category;
    public bool bought;
    public string shopItemID;

    public ShopItem(string shopItemID, int cost, string imageName, string category, bool bought)
    {
        this.shopItemID = shopItemID;
        this.cost = cost;
        this.imageName = imageName;
        this.category = category;
        this.bought = bought;
    }

    public ShopItem() { }

    /// <summary>
    ///     This function converts a json string to a <c>ShopItem<c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>Achievement</c> object containing the data</returns>
    public static ShopItem CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ShopItem>(jsonString);
    }

}
