using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public string shopItemID;
    public int cost;
    public string imageName;
    public string category;
    public bool bought;

    public ShopItem(string shopItemID, int cost, string imageName, string category, bool bought)
    {
        this.shopItemID = shopItemID;
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
