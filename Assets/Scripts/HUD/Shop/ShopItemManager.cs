using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    [SerializeField] private Button OUTFITButton;
    [SerializeField] private Button ACCESSORIESEButton;
    [SerializeField] private Button showAllButton;
    [SerializeField] private Button inventoryButton;

    [SerializeField] private AudioSource audioSource;  
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip notEnoughCreditSound;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip clickSound;


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
        UpdateCreditText();

        yesButton.onClick.AddListener(YesButtonClicked);
        noButton.onClick.AddListener(() => insurancePanel.SetActive(false));

        insuranceCloseButton.onClick.AddListener(() => ClosePanels());
        successCloseButton.onClick.AddListener(() => ClosePanels());

        OUTFITButton.onClick.AddListener(OnOutfitButtonClicked);
        ACCESSORIESEButton.onClick.AddListener(OnAccessoriesButtonClicked);
        showAllButton.onClick.AddListener(OnShowAllItemsClicked);
        inventoryButton.onClick.AddListener(ShowPurchasedItems);

        if (audioSource != null)
        {
            audioSource.volume = 0.3f;
        }

    }

    void UpdateUI()
    {
        ClearShopItems();
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
                    PlayAlertSound();
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

        PlayClickSound();

        ownData = DataManager.Instance.GetOwnStatisticData();
        int price = currentItemPrice;

        Debug.Log($"Attempting to buy item: {currentItemTitle} for {currentItemPrice} coins.");

        if (ownData.GetCredit() >= currentItemPrice)
        {
            

            PlayClickSound();

            PlaySuccessSound();

            successPanel.SetActive(true);
            successText.text = $"Nice! You just bought the {currentItemTitle} for {currentItemPrice} coins!";

            
            GameManager.Instance.UpdateShopItem(currentItemTitle, true);
            GameManager.Instance.SaveShopItem();

            GameManager.Instance.UpdatePlayerCredit(price, ownData.GetCredit());
            GameManager.Instance.SavePlayerData();

            

            UpdateCreditText();
            UpdateUI();
        }
        else
        {
            PlayClickSound();

            PlayNotEnoughCreditSound();
            successPanel.SetActive(true);
            successText.text = "Oh no, you don't have enough credit! Let's gain some more rewards and then turn back!";
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
        PlayClickSound();
        insurancePanel.SetActive(false);
        successPanel.SetActive(false);
    }

    private void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.time = 1.0f;
            audioSource.PlayOneShot(successSound);
        }

        
    }

    private void PlayNotEnoughCreditSound()
    {
        if (audioSource != null && notEnoughCreditSound != null)
        {
            audioSource.PlayOneShot(notEnoughCreditSound);
        }


    }

    private void PlayAlertSound()
    {
        if (audioSource != null && alertSound != null)
        {
            audioSource.PlayOneShot(alertSound);
        }


    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }


    }
    private void FilterItemsByCategory(string category)
    {
        ClearShopItems();
        List<ShopItemData> filteredItems = shopItemData.FindAll(item => item.GetCategory().Equals(category, StringComparison.OrdinalIgnoreCase));
        DisplayShopItems(filteredItems);
    }

    public void OnOutfitButtonClicked()
    {
        PlayClickSound();
        FilterItemsByCategory("OUTFIT");
    }

    public void OnAccessoriesButtonClicked()
    {
        PlayClickSound();
        FilterItemsByCategory("ACCESSORIES");
    }

    public void OnShowAllItemsClicked()
    {
        PlayClickSound();

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

    private void ShowPurchasedItems()
    {
        PlayClickSound();

        ClearShopItems();

        List<ShopItemData> purchasedItems = shopItemData.FindAll(item => item.IsBought());
        DisplayShopItems(purchasedItems);
    }

}