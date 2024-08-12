using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemManager : MonoBehaviour
{

    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private GameObject content;

    private List<ShopItemData> shopItemData;



    // Start is called before the first frame update
    void Start()
    {
        shopItemData = DataManager.Instance.GetShopItems();
        UpdateUI();
    }

    // Update is called once per frame
    void UpdateUI()
    {
        DisplayShopItems(shopItemData);
    }

    private void DisplayShopItems(List<ShopItemData> shopItemsToDisplay)
    {
        foreach (ShopItemData shopItem in shopItemsToDisplay)
        {
            DisplayShopItem(shopItem);
        }
    }

    private void DisplayShopItem(ShopItemData shopItem)
    {
        GameObject shopItemObject = Instantiate(shopItemPrefab, content.transform, false);

        ShopItemUIElement shopItemUIElement = shopItemObject.GetComponent<ShopItemUIElement>();
        if (shopItemUIElement != null)
        {
            string title = shopItem.GetTitle();            
            Sprite image = shopItem.GetImage();
            shopItemUIElement.Setup(title, image);
        }
        else
        {
            Destroy(shopItemObject);
        }
    }
}
