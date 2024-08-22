using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopItemData 
{
    private static string imageFolder = "ShopItemImages";
    private static string defaultImageName = "defaultImage";

    private readonly int cost;
    private  bool bought;
    private readonly string imageName;
    private readonly Sprite image;
    private bool updated;
    private  readonly string category;
    private  readonly string shopItemID;


    public ShopItemData(string shopItemID, int cost, bool bought, string imageName, string category)
    {
        this.shopItemID = shopItemID;
        this.cost = cost;
        this.bought = bought;
        this.imageName = imageName;
        this.image = GetImage(imageName);
        updated = false;
        this.category = category;

    }


    public static ShopItemData ConvertFromShopItem(ShopItem status)
    {
        int cost = status.cost;
        string title = status.title;
        string imageName = status.imageName;
        bool bought = status.bought;
        string category = status.category;
        

       ShopItemData data = new ShopItemData(title, cost, bought, imageName, category);
        return data;
    }

    public static ShopItem ConvertToShopItem(ShopItemData shopItemData)
    {
        int cost = shopItemData.GetCost();
        bool bought = shopItemData.IsBought();
        string title = shopItemData.GetTitle();
        string imageName = shopItemData.GetImageName();
        string category = shopItemData.GetCategory();

       ShopItem shopItem = new ShopItem(title, cost, imageName, category, bought);


        return shopItem;
    }


    public bool UpdateProgress(bool newProgress)
    {
        updated = true;
        bought = newProgress;
        if (newProgress = true)
        {
            
            return true;
        }
        return false;
    }



    private Sprite GetImage(string imageName)
    {
        var sprite = Resources.Load<Sprite>(imageFolder + "/" + imageName);
        if (sprite == null)
        {
            Debug.Log("Load default image");
            sprite = Resources.Load<Sprite>(imageFolder + "/" + defaultImageName);
        }
        //Debug.Log(sprite.ToString());
        return sprite;
    }

    public string GetImageName()
    {
        return imageName;
    }

    public Sprite GetImage()
    {
        return image;
    }

    

    public string GetTitle()
    {
        return shopItemID;
    }

    public int GetCost()
    {
        return cost;
    }

    public bool IsBought()
    {
        return bought;
    }

    public bool isUpdated()
    {
        return updated;
    }

    public string GetCategory()
    {
        return category;
    }
}
