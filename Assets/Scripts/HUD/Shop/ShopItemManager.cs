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
        ownData = DataManager.Instance.GetOwnStatisticData();
        shopItemData = DataManager.Instance.GetShopItems();
        UpdateUI();

        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(() => insurancePanel.SetActive(false));

        insuranceCloseButton.onClick.AddListener(() => ClosePanels());
        successCloseButton.onClick.AddListener(() => ClosePanels());
    }

    void UpdateUI()
    {
        DisplayShopItems(shopItemData);
        UpdateCreditText();
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
            shopItemUIElement.Setup(title, image, price);

            Button buyButton = shopItemObject.GetComponentInChildren<Button>();
            if (buyButton != null)
            {
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

    private void OnYesButtonClicked()
    {
        Debug.Log($"Attempting to buy item: {currentItemTitle} for {currentItemPrice} coins.");

        if (ownData.GetCredit() >= currentItemPrice)
        {
            int oldCredit = ownData.GetCredit();
            int newCredit = oldCredit - currentItemPrice;

            Debug.Log($"Old Credit: {oldCredit}, New Credit: {newCredit}");

            ownData.SetCredit(newCredit);

            int updatedCredit = ownData.GetCredit();
            Debug.Log($"Credit after update: {updatedCredit}");

            UpdateCreditText();  

            successPanel.SetActive(true);
            successText.text = $"Nice! You just bought the {currentItemTitle} for {currentItemPrice} coins!";
        }
        else
        {
            insuranceText.text = "Sorry, you don't have enough credits!";
        }

        insurancePanel.SetActive(false);
    }

    private void UpdateCreditText()
    {
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
