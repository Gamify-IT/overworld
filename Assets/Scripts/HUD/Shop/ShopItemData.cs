using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections.Generic;

public class ShopItemData 
{
    private static string imageFolder = "ShopItemImages";
    private static string defaultImageName = "defaultImage";

    private readonly int cost;
    private  bool bought;
    private readonly string imageName;
    private readonly Sprite image;
    private bool updated;
    private readonly ShopItemCategory category;
    private readonly ShopItemTitle shopItemID;


    public ShopItemData(ShopItemTitle shopItemID, int cost, bool bought, string imageName, ShopItemCategory category)
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
        ShopItemTitle title = status.title;
        string imageName = status.imageName;
        bool bought = status.bought;
        ShopItemCategory category = status.category;
        

       ShopItemData data = new ShopItemData(title, cost, bought, imageName, category);
        return data;
    }

    public static ShopItem ConvertToShopItem(ShopItemData shopItemData)
    {
        int cost = shopItemData.GetCost();
        bool bought = shopItemData.IsBought();
        ShopItemTitle title = shopItemData.GetTitle();
        string imageName = shopItemData.GetImageName();
        ShopItemCategory category = shopItemData.GetCategory();

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

    

    public ShopItemTitle GetTitle()
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

    public ShopItemCategory GetCategory()
    {
        return category;
    }
}
