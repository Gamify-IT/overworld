using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemManager : MonoBehaviour
{
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private GameObject content;

    [SerializeField] private GameObject insurancePanel;
    [SerializeField] private TMP_Text insuranceText;

    [SerializeField] private GameObject successPanel;
    [SerializeField] private TMP_Text successText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [SerializeField] private Button insuranceCloseButton;
    [SerializeField] private Button successCloseButton;

    [SerializeField] private TMP_Text creditText;

    private string currentItemTitle;
    private int currentItemPrice;

    private PlayerStatisticData ownData;

    private List<ShopItemData> shopItemData;

    void Start()
    {
        shopItemData = DataManager.Instance.GetShopItems();
        UpdateUI();

        yesButton.onClick.AddListener(YesButtonClicked);
        noButton.onClick.AddListener(() => insurancePanel.SetActive(false));

        insuranceCloseButton.onClick.AddListener(() => ClosePanels());
        successCloseButton.onClick.AddListener(() => ClosePanels());
    }

    void UpdateUI()
    {
        ClearShopItems(); 
        DisplayShopItems(shopItemData);
    }

    private void ClearShopItems()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);  
        }
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
            int price = shopItem.GetCost();
            bool bought = shopItem.IsBought();
            bool showCoin = !bought;
            shopItemUIElement.Setup(title, image, bought, showCoin);

            Button buyButton = shopItemObject.GetComponentInChildren<Button>();
            TMP_Text buyButtonText = buyButton.GetComponentInChildren<TMP_Text>();

            Image panelImage = shopItemObject.GetComponent<Image>();


            if (bought)
            {
                panelImage.color = new Color(1f, 1f, 1f, 0.5f);

                if (buyButtonText != null)
                {
                    buyButtonText.text = "<i>Already Bought</i>";  
                }

                buyButton.onClick.AddListener(() => {
                    successPanel.SetActive(true);
                    successText.text = "You already bought this item.";
                });

            }
            else
            {
                if (buyButtonText != null)
                {
                    buyButtonText.text = $"Buy for {price} coins";
                }
                buyButton.interactable = true;

                buyButton.onClick.AddListener(() => OpenInsurancePanel(title, price));
            }
        }
        else
        {
            Destroy(shopItemObject);
        }
    }

    private void OpenInsurancePanel(string title, int price)
    {
        insurancePanel.SetActive(true);
        insuranceText.text = $"Are you sure you want to buy the article {title} for {price} coins?";
        currentItemTitle = title;
        currentItemPrice = price;
    }

    private void YesButtonClicked()
    {
        ownData = DataManager.Instance.GetOwnStatisticData();

        Debug.Log($"Attempting to buy item: {currentItemTitle} for {currentItemPrice} coins.");

        if (ownData.GetCredit() >= currentItemPrice)
        {
            int oldCredit = ownData.GetCredit();
            int newCredit = oldCredit - currentItemPrice;

            Debug.Log($"Old Credit: {oldCredit}, New Credit: {newCredit}");

            ownData.SetCredit(newCredit);

            int updatedCredit = ownData.GetCredit();
            Debug.Log($"Credit after update: {updatedCredit}");

            successPanel.SetActive(true);
            successText.text = $"Nice! You just bought the {currentItemTitle} for {currentItemPrice} coins!";
            if (System.Enum.TryParse(currentItemTitle, out ShopItemTitle itemTitle))
            {
                DataManager.Instance.UpdateShopItemStatus(itemTitle, true);
            }
            else
            {
                Debug.LogError($"Failed to parse item title: {currentItemTitle}");
                insuranceText.text = "Error: Invalid item title!";
            }

            UpdateCreditText();
            UpdateUI();
        }
        else
        {
            successPanel.SetActive(true);
            successText.text = "Oh no, you don't have enough credit! Let's gain some more rewards and then turn back!";
        }

        insurancePanel.SetActive(false);
    }

    private void UpdateCreditText()
    {
        ownData = DataManager.Instance.GetOwnStatisticData();

        int credit = ownData.GetCredit();
        creditText.text = $"{credit}";
        Debug.Log($"Updated Credit Text: {credit}");
    }

    private void ClosePanels()
    {
        insurancePanel.SetActive(false);
        successPanel.SetActive(false);
    }
}
