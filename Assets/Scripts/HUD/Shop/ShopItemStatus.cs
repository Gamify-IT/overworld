using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemStatus 
{
    public string id;
    public ShopItem shopItem;
    public bool bought;

    public ShopItemStatus(string id, ShopItem shopItem, bool bought)
    {
        this.id = id;
        this.shopItem = shopItem;
        this.bought = bought;
             
    }

    public ShopItemStatus() { }

    public static ShopItemStatus CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ShopItemStatus>(jsonString);
    }

}
