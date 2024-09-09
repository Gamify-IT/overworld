using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


/// <summary>
///     This class manages the shop items in the shop such as buy options, description like image and price and filter options
/// </summary>
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

    /// <summary>
    /// Initializes the shop by loading player data, shop items, and setting up UI elements.
    /// Adds listeners to the various buttons for user interaction.
    /// </summary>
    void Start()
    {
        ownData = DataManager.Instance.GetOwnPlayerData();
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

    /// <summary>
    /// Updates the UI by clearing existing shop items and displaying the current items available for purchase.
    /// </summary>
    void UpdateUI()
    {
        ClearShopItems();
        DisplayShopItems(shopItemData);
    }


    /// <summary>
    /// Displays the list of shop items provided as a parameter by instantiating UI elements for each item.
    /// </summary>
    /// <param name="shopItemsToDisplay">A list of shop items to display in the shop UI.</param>

    private void DisplayShopItems(List<ShopItemData> shopItemsToDisplay)
    {
        foreach (ShopItemData shopItem in shopItemsToDisplay)
        {
            DisplayShopItem(shopItem);
        }
    }

    /// <summary>
    /// Creates a UI element for a single shop item, sets up its display, and configures the interaction behavior.
    /// Changes the button interaction based on whether the item is already purchased.
    /// </summary>
    /// <param name="shopItem">The shop item to display in the UI.</param>
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
               
                Color originalColor = panelImage.color;

                float brightnessFactor = 1.2f; 
                Color brighterColor = new Color(
                    Mathf.Clamp(originalColor.r * brightnessFactor, 0f, 1f),
                    Mathf.Clamp(originalColor.g * brightnessFactor, 0f, 1f),
                    Mathf.Clamp(originalColor.b * brightnessFactor, 0f, 1f),
                    originalColor.a 
                );

                panelImage.color = brighterColor;

                if (buyButtonText != null)
                {
                    buyButtonText.text = "<i>Already Bought!</i>";
                }

                buyButton.onClick.AddListener(() => {
                    PlayAlertSound();
                    successPanel.SetActive(true);
                    successText.text = "You already bought this item!";
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

    /// <summary>
    /// Opens a confirmation panel to ask the player if they want to purchase a specific item.
    /// Sets the item title and price for the confirmation.
    /// </summary>
    /// <param name="title">The title of the item to be purchased.</param>
    /// <param name="price">The cost of the item in coins.</param>
    private void OpenInsurancePanel(string title, int price)
    {
        insurancePanel.SetActive(true);
        insuranceText.text = $"Are you sure you want to buy the article {title} for {price} coins?";
        currentItemTitle = title;
        currentItemPrice = price;
    }

    /// <summary>
    /// Handles the logic for when the player confirms purchasing an item.
    /// Checks if the player has enough credit, processes the purchase, and updates the shop and player data.
    /// </summary>
    private void YesButtonClicked()
    {

        PlayClickSound();

        ownData = DataManager.Instance.GetOwnPlayerData();
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

    /// <summary>
    /// Updates the player's current credit displayed in the shop UI.
    /// </summary>
    private void UpdateCreditText()
    {

        int credit = ownData.GetCredit();
        creditText.text = $"{credit}";
    }

    /// <summary>
    /// Closes any open panels such as the insurance and success panels.
    /// </summary>
    private void ClosePanels()
    {
        PlayClickSound();
        insurancePanel.SetActive(false);
        successPanel.SetActive(false);
    }

    /// <summary>
    /// Plays the success sound effect when the player successfully purchases an item.
    /// </summary>
    private void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.time = 1.0f;
            audioSource.PlayOneShot(successSound);
        }

        
    }

    /// <summary>
    /// Plays the sound effect for when the player doesn't have enough credit to buy an item.
    /// </summary>
    private void PlayNotEnoughCreditSound()
    {
        if (audioSource != null && notEnoughCreditSound != null)
        {
            audioSource.PlayOneShot(notEnoughCreditSound);
        }


    }

    /// <summary>
    /// Plays the alert sound effect, usually when the player attempts to buy an already purchased item.
    /// </summary>
    private void PlayAlertSound()
    {
        if (audioSource != null && alertSound != null)
        {
            audioSource.PlayOneShot(alertSound);
        }


    }

    /// <summary>
    /// Plays the standard click sound effect for button interactions.
    /// </summary>
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }


    }

    /// <summary>
    /// Filters the shop items based on the provided category and updates the UI with the filtered results.
    /// </summary>
    /// <param name="category">The category to filter items by (e.g., OUTFIT, ACCESSORIES).</param>
    private void FilterItemsByCategory(string category)
    {
        ClearShopItems();
        List<ShopItemData> filteredItems = shopItemData.FindAll(item => item.GetCategory().Equals(category, StringComparison.OrdinalIgnoreCase));
        DisplayShopItems(filteredItems);
    }

    /// <summary>
    /// Handles the outfit category button click, filters the shop items by "OUTFIT" category.
    /// </summary>
    public void OnOutfitButtonClicked()
    {
        PlayClickSound();
        FilterItemsByCategory("OUTFIT");
    }

    /// <summary>
    /// Handles the accessories category button click, filters the shop items by "ACCESSORIES" category.
    /// </summary>
    public void OnAccessoriesButtonClicked()
    {
        PlayClickSound();
        FilterItemsByCategory("ACCESSORIES");
    }

    /// <summary>
    /// Shows all items by clearing any filters and displaying all available shop items.
    /// </summary>
    public void OnShowAllItemsClicked()
    {
        PlayClickSound();

        ClearShopItems();
        DisplayShopItems(shopItemData);
    }

    /// <summary>
    /// Clears all shop items currently displayed in the UI by destroying their game objects.
    /// </summary>
    private void ClearShopItems()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Displays the items the player has already purchased by filtering out non-purchased items.
    /// </summary>
    private void ShowPurchasedItems()
    {
        PlayClickSound();

        ClearShopItems();

        List<ShopItemData> purchasedItems = shopItemData.FindAll(item => item.IsBought());
        DisplayShopItems(purchasedItems);
    }

}