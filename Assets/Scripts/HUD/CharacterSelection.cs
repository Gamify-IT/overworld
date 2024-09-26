using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Image glassesImage;
    private Image hatImage;
    private Sprite character;
    private Sprite glasses;
    private Sprite hat;
    private int numberOfCharacters = 9;
    private int numberOfGlasses = 4;
    private int numberOfHats = 3;
    private List<ShopItemData> shopItemData;
    private PlayerAnimation animationScript;

    [SerializeField] private TMP_Text descriptionAccessory;
    [SerializeField] private TMP_Text characterDescriptionText;
    [SerializeField] private GameObject lockImageOutfit;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject[] characterPrefabs;

    public Button glassesButton;
    public Button hatButton;
    public TextMeshProUGUI warningText;

    public AudioClip clickSound;
    private AudioSource audioSource;

    private int currentIndex = 0;
    private int currentGlasses = 0;
    private int currentHat = 0;

    private Color mainColor = new Color(0.82f, 0.69f, 0.56f);
    private Color selectedColor = new Color(0.9f, 0.76f, 0.62f);
    private Color unselectedColor = new Color(0.82f, 0.69f, 0.56f, 0.5f);

    public enum AccessoryType
    {
        Glasses,
        Hat
    }

    private AccessoryType currentAccessoryType = AccessoryType.Glasses;

    void Start()
    {
        shopItemData = DataManager.Instance.GetShopItems();
        GameManager.Instance.isPaused = true;
        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        glassesImage = GameObject.Find("Glasses Sprite").GetComponent<Image>();
        hatImage = GameObject.Find("Hat Sprite").GetComponent<Image>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;

        UpdateButtonVisuals();
        UpdateWarnings();
        UpdateCharacterDisplay();
        CheckCharacterStatus();
        UpdateAccessoryDescriptions();
    }

    void Update()
    {
        character = Resources.Load<Sprite>("characters/character" + (currentIndex % numberOfCharacters));
        characterImage.sprite = character;

        UpdateVisualsAndStatus();
        UpdateAccessoryDescriptions();
        UpdateWarnings();
    }

    private void UpdateVisualsAndStatus()
    {
        if (currentAccessoryType == AccessoryType.Glasses)
        {
            glasses = Resources.Load<Sprite>("Glasses/glasses" + (currentGlasses % numberOfGlasses));
            glassesImage.sprite = glasses;
            glassesImage.color = Color.white;
            hatImage.sprite = null;
            hatImage.color = new Color(1, 1, 1, 0);
            CheckAccessoryStatus(glassesImage.sprite.name);
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            hat = Resources.Load<Sprite>("Hats/hat" + (currentHat % numberOfHats));
            hatImage.sprite = hat;
            hatImage.color = Color.white;
            glassesImage.sprite = null;
            glassesImage.color = new Color(1, 1, 1, 0);
            CheckAccessoryStatus(hatImage.sprite.name);
        }

        CheckCharacterStatus();
        UpdateCharacterDisplay();
    }

    private void CheckAccessoryStatus(string currentImageName)
    {
        bool isLocked = true;
        foreach (var item in shopItemData)
        {
            if (item.GetImageName() == currentImageName)
            {
                if (item.IsBought())
                {
                    isLocked = false;
                }
                break;
            }
        }
        lockImage.SetActive(isLocked);
    }

    private void CheckCharacterStatus()
    {
        string characterImageName = "character" + (currentIndex % numberOfCharacters);
        bool isLocked = true;

        foreach (var item in shopItemData)
        {
            if (item.GetImageName() == characterImageName)
            {
                if (item.IsBought())
                {
                    isLocked = false;
                }
                break;
            }
        }

        lockImageOutfit.SetActive(isLocked);

        if (currentIndex == 6)  
        {
            DisableNextPreviousButtons(); 
        }
    }


    private void DisableNextPreviousButtons()
    {
        var previousButton = GameObject.Find("PreviousButton");
        var nextButton = GameObject.Find("NextButton");

        if (previousButton != null && nextButton != null)
        {
            previousButton.GetComponent<Button>().interactable = false;
            nextButton.GetComponent<Button>().interactable = false;
        }
    }

    private void UpdateWarnings()
    {
        if (currentIndex == 6) 
        {
            warningText.text = "Looks like Titanium Knight's suit prefers to go solo, no hats or glasses with this one!";

            descriptionAccessory.text = "";

            DisableAccessoryButtons();
            HideAccessories();

            lockImage.SetActive(false);

            DisableNextPreviousButtons();
        }
        else
        {
            warningText.text = "";
            EnableAccessoryButtons();
            UpdateVisualsAndStatus();

            EnableNextPreviousButtons();
        }
    }


    private void DisableAccessoryButtons()
    {
        glassesButton.interactable = false;
        glassesButton.image.color = Color.gray;

        hatButton.interactable = false;
        hatButton.image.color = Color.gray;
    }

    private void EnableAccessoryButtons()
    {
        glassesButton.interactable = true;
        glassesButton.image.color = selectedColor;

        hatButton.interactable = true;
        hatButton.image.color = unselectedColor;
    }

    private void HideAccessories()
    {
        glassesImage.sprite = null;
        glassesImage.color = new Color(1, 1, 1, 0);

        hatImage.sprite = null;
        hatImage.color = new Color(1, 1, 1, 0);
    }

    private void EnableNextPreviousButtons()
    {
        var previousButton = GameObject.Find("PreviousButton");
        var nextButton = GameObject.Find("NextButton");

        if (previousButton != null && nextButton != null)
        {
            previousButton.GetComponent<Button>().interactable = true;
            nextButton.GetComponent<Button>().interactable = true;
        }
    }

    public void PreviousAccessory()
    {
        if (currentAccessoryType == AccessoryType.Glasses)
        {
            Previousglasses();
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            PreviousHats();
        }
        UpdateVisualsAndStatus();
    }

    public void NextAccessory()
    {
        if (currentAccessoryType == AccessoryType.Glasses)
        {
            Nextglasses();
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            NextHats();
        }
        UpdateVisualsAndStatus();
    }

    public void SetAccessoryToHat()
    {
        currentAccessoryType = AccessoryType.Hat;
        UpdateVisualsAndStatus();
        UpdateButtonVisuals();
    }

    public void SetAccessoryToGlasses()
    {
        currentAccessoryType = AccessoryType.Glasses;
        UpdateVisualsAndStatus();
        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        if (currentAccessoryType == AccessoryType.Glasses)
        {
            glassesButton.image.color = selectedColor;
            hatButton.image.color = unselectedColor;
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            glassesButton.image.color = unselectedColor;
            hatButton.image.color = selectedColor;

        }
    }

    public void PreviousCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex - 1, numberOfCharacters);
        UpdateVisualsAndStatus();
    }

    public void NextCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex + 1, numberOfCharacters);
        UpdateVisualsAndStatus();
    }

    public void Previousglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses - 1, numberOfGlasses);
    }

    public void Nextglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses + 1, numberOfGlasses);
    }

    public void PreviousHats()
    {
        PlayClickSound();
        currentHat = Modulo(currentHat - 1, numberOfHats);
    }

    public void NextHats()
    {
        PlayClickSound();
        currentHat = Modulo(currentHat + 1, numberOfHats);
    }

    public void ConfirmButton()
    {
        // TODO right place ??
        PlayClickSound();
        animationScript.SetOutfitAnimator("body", "head");
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private int Modulo(int a, int b)
    {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    private void UpdateCharacterDisplay()
    {
        string characterImageName = "character" + (currentIndex % numberOfCharacters);
        bool isFreeSkin = characterImageName == "character0" || characterImageName == "character1" || characterImageName == "character2";
        bool isLocked = !isFreeSkin;

        if (!isFreeSkin)
        {
            foreach (var item in shopItemData)
            {
                if (item.GetImageName() == characterImageName)
                {
                    if (item.IsBought())
                    {
                        isLocked = false;
                    }
                    break;
                }
            }
        }

        lockImageOutfit.SetActive(isLocked);

        string descriptionText = "";
        foreach (var item in shopItemData)
        {
            if (item.GetImageName() == characterImageName)
            {
                descriptionText = $"Character: {item.GetTitle()}\nBought: {(item.IsBought() ? "Yes" : "No")}";


                if (!item.IsBought())
                {
                    descriptionText += $"\nPrice: {item.GetCost()}";
                }

                if (isFreeSkin)
                {
                    descriptionText += "\nFREE SKIN";
                }

                break;
            }
        }

        characterDescriptionText.text = descriptionText;
    }


    private void UpdateAccessoryDescriptions()
    {
        string descriptionText = "";

        if (currentAccessoryType == AccessoryType.Glasses)
        {
            foreach (var item in shopItemData)
            {
                if (item.GetImageName() == glassesImage.sprite.name)
                {
                    descriptionText = $"Accessory: {item.GetTitle()}\nBought: {(item.IsBought() ? "Yes" : "No")}";

                    if (!item.IsBought())
                    {
                        descriptionText += $"\nPrice: {item.GetCost()}";
                    }
                    break;
                }
            }
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            foreach (var item in shopItemData)
            {
                if (item.GetImageName() == hatImage.sprite.name)
                {
                    descriptionText = $"Accessory: {item.GetTitle()}\nBought: {(item.IsBought() ? "Yes" : "No")}";

                    if (!item.IsBought())
                    {
                        descriptionText += $"\nPrice: {item.GetCost()}";
                    }
                    break;
                }
            }
        }

        descriptionAccessory.text = descriptionText;
    }

}
