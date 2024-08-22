using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ShopItem 
{
    public int cost;
    public string imageName;
    public ShopItemCategory category;
    public bool bought;
    public ShopItemTitle title;

    public ShopItem(ShopItemTitle title, int cost, string imageName, ShopItemCategory category, bool bought)
    {
        this.title = title;
        this.cost = cost;
        this.imageName = imageName;
        this.category = category;
        this.bought = bought;
    }

    public ShopItem() { }

    public static ShopItem CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ShopItem>(jsonString);
    }
    
}
