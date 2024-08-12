using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public string shopItemID;
    public int cost;
    public string imageName;


    public ShopItem(string shopItemID, int cost, string imageName)
    {
        this.shopItemID = shopItemID;
        this.cost = cost;
        this.imageName = imageName;
    }

    public ShopItem() { }

    public static ShopItem CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ShopItem>(jsonString);
    }
    
}
