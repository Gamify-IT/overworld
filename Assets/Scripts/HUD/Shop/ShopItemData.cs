using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is used to store all relevant information about shop items in the overworld frontend
/// </summary>
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

    /// <summary>
    ///     This function converts a <c>ShopItem</c> to a <c>ShopItemData</c>
    /// </summary>
    /// <param name="item">The <c>ShopItem</c> to convert</param>
    /// <returns>The converted <c>ShopItemData</c> object</returns>
    public static ShopItemData ConvertFromShopItem(ShopItem item)
    {
        int cost = item.cost;
        string title = item.shopItemID;
        string imageName = item.imageName;
        bool bought = item.bought;
        string category = item.category;
        

       ShopItemData data = new ShopItemData(title, cost, bought, imageName, category);
        return data;
    }

    /// <summary>
    ///     This function converts a <c>ShopItemData</c> to an <c>ShopItem</c>
    /// </summary>
    /// <param name="shopItemData">The <c>ShopItemData</c> to convert</param>
    /// <returns>The converted <c>ShopItem</c> object</returns>
    public static ShopItem ConvertToShopItem(ShopItemData shopItemData)
    {
        int cost = shopItemData.GetCost();
        bool bought = shopItemData.IsBought();
        string shopItemID = shopItemData.GetTitle();
        string imageName = shopItemData.GetImageName();
        string category = shopItemData.GetCategory();

       ShopItem shopItem = new ShopItem(shopItemID, cost, imageName, category, bought);


        return shopItem;
    }

    /// <summary>
    ///     This function updates the bought status and sets the completed flag if needed
    /// </summary>
    /// <param name="newProgress">The bought status</param>
    /// <returns>True if the bought status is true, false otherwise</returns>
    public bool UpdateProgress(bool newProgress)
    {
        updated = true;
        bought = newProgress;
        return newProgress;
    }


    /// <summary>
    ///     This function returns a sprite for a shop item
    /// </summary>
    /// <param name="imageName">The name of the sprite to return</param>
    /// <returns>The sprite with the given name, if present or the default image otherwise</returns>
    private Sprite GetImage(string imageName)
    {
        var sprite = Resources.Load<Sprite>(imageFolder + "/" + imageName);
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>(imageFolder + "/" + defaultImageName);
        }
        return sprite;
    }

    #region Getter
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

    #endregion
}
