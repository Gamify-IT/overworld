using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using System.Collections.Generic;
using System;

/// <summary>
///     This class opens the <c>character selection</c> menu and includes logic for choosing and selecting new characters.
/// </summary>
public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Image glassesImage;
    private Image hatImage;
    private Sprite character;
    private Sprite glasses;
    private Sprite hat;
    private readonly int numberOfCharacters = 9;
    private readonly int numberOfGlasses = 5;
    private readonly int numberOfHats = 5;
    private List<ShopItemData> shopItemData;
    private PlayerAnimation animationScript;

    [SerializeField] private TMP_Text descriptionAccessory;
    [SerializeField] private TMP_Text characterDescriptionText;
    [SerializeField] private GameObject lockImageOutfit;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private AudioClip clickSound;

    public Button glassesButton;
    public Button hatButton;
    public TextMeshProUGUI warningText;
    private AudioSource audioSource;
    private int currentIndex = 0;
    private int currentGlasses = 0;
    private int currentHat = 0;

    private Color mainColor = new Color(0.82f, 0.69f, 0.56f);
    private Color selectedColor = new Color(0.9f, 0.76f, 0.62f);
    private Color unselectedColor = new Color(0.82f, 0.69f, 0.56f, 0.5f);

    private string selectedBody;
    private string selectedHead;
    private PlayerStatisticData ownData;

    readonly Dictionary<string, string> imagenameToAnimationString = new Dictionary<string, string>();
    readonly Dictionary<string, string> animationToImage = new Dictionary<string, string>();

    public enum AccessoryType
    {
        Glasses,
        Hat
    }

    private AccessoryType currentAccessoryType = AccessoryType.Glasses;

    /// <summary>
    /// This function is called when the character selection is opened.
    /// It manages the initial setup of the data and displays the players active outfit.
    /// </summary>
    void Start()
    {
        SetupDictionaries();

        ownData = DataManager.Instance.GetPlayerData();
        animationScript = GameObject.FindObjectOfType<PlayerAnimation>();
        shopItemData = DataManager.Instance.GetShopItems();
        GameManager.Instance.SetIsPaused(true);

        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        glassesImage = GameObject.Find("Glasses Sprite").GetComponent<Image>();
        hatImage = GameObject.Find("Hat Sprite").GetComponent<Image>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;

        string currentCharacter = ownData.GetCurrentCharacter();
        string currentCharacterImage = animationToImage[currentCharacter];
        currentIndex = Convert.ToInt32(currentCharacterImage[currentCharacterImage.Length - 1] - '0');
        string currentAccessory = ownData.GetCurrentAccessory();
        string currentAccessoryImage = animationToImage[currentAccessory];

        if (currentAccessoryImage.Contains("hat"))
        {
            currentHat = Convert.ToInt32(currentAccessoryImage[currentAccessoryImage.Length - 1] - '0');
            currentAccessoryType = AccessoryType.Hat;
        }
        else
        {
            currentGlasses = Convert.ToInt32(currentAccessoryImage[currentAccessoryImage.Length - 1] - '0');
            currentAccessoryType = AccessoryType.Glasses;
        }

        RefreshAccessoryButtonVisuals();
        UpdateAccessoryWarnings();
        ValidateCharacterUnlockStatus();
    }

    void Update()
    {
        character = Resources.Load<Sprite>("characters/character" + (currentIndex % numberOfCharacters));
        characterImage.sprite = character;

        UpdateCharacterAndAccessoryVisuals();
        UpdateAccessoryWarnings();
    }

    /// <summary>
    /// Sets up the dictionaries used to look up the animation name corresponding to the internal index.
    /// </summary>
    void SetupDictionaries()
    {
        imagenameToAnimationString.Add("hat0", "flammen_haare");
        imagenameToAnimationString.Add("hat1", "globus_hut");
        imagenameToAnimationString.Add("hat2", "schutzhelm");
        imagenameToAnimationString.Add("hat3", "blonde_haare");
        imagenameToAnimationString.Add("hat4", "none");
        imagenameToAnimationString.Add("glasses0", "3D_brille");
        imagenameToAnimationString.Add("glasses1", "coole_brille");
        imagenameToAnimationString.Add("glasses2", "herzbrille");
        imagenameToAnimationString.Add("glasses3", "retro_brille");
        imagenameToAnimationString.Add("glasses4", "none");
        imagenameToAnimationString.Add("character0", "character_default");
        imagenameToAnimationString.Add("character1", "character_blue_and_purple");
        imagenameToAnimationString.Add("character2", "character_black_and_white");
        imagenameToAnimationString.Add("character3", "character_trainingsanzug");
        imagenameToAnimationString.Add("character4", "character_anzug");
        imagenameToAnimationString.Add("character5", "character_jeans_karo");
        imagenameToAnimationString.Add("character6", "character_lange_haare");
        imagenameToAnimationString.Add("character7", "character_ironman");
        imagenameToAnimationString.Add("character8", "character_santa");

        animationToImage.Add("flammen_haare", "hat0");
        animationToImage.Add("globus_hut", "hat1");
        animationToImage.Add("schutzhelm", "hat2");
        animationToImage.Add("blonde_haare", "hat3");
        animationToImage.Add("none", "glasses4");
        animationToImage.Add("3D_brille", "glasses0");
        animationToImage.Add("coole_brille", "glasses1");
        animationToImage.Add("herzbrille", "glasses2");
        animationToImage.Add("retro_brille", "glasses3");
        animationToImage.Add("character_default", "character0");
        animationToImage.Add("character_blue_and_purple", "character1");
        animationToImage.Add("character_black_and_white", "character2");
        animationToImage.Add("character_trainingsanzug", "character3");
        animationToImage.Add("character_anzug", "character4");
        animationToImage.Add("character_jeans_karo", "character5");
        animationToImage.Add("character_lange_haare", "character6");
        animationToImage.Add("character_ironman", "character7");
        animationToImage.Add("character_santa", "character8");
    }

    /// <summary>
    /// Updates what is displayed in the character selection.
    /// </summary>
    private void UpdateCharacterAndAccessoryVisuals()
    {
        if (currentIndex == 7 || currentIndex == 8)
        {
            glassesImage.sprite = Resources.Load<Sprite>("Glasses/glasses4");
            glassesImage.color = new Color(1, 1, 1, 0);

            hatImage.sprite = Resources.Load<Sprite>("Hats/hat4");
            hatImage.color = new Color(1, 1, 1, 0);

            selectedHead = "none";
            selectedBody = imagenameToAnimationString["character" + currentIndex];

            lockImage.SetActive(false);
            return;
        }

        if (currentAccessoryType == AccessoryType.Glasses)
        {
            glasses = Resources.Load<Sprite>("Glasses/glasses" + (currentGlasses % numberOfGlasses));
            glassesImage.sprite = glasses;
            glassesImage.color = Color.white;
            hatImage.sprite = null;
            hatImage.color = new Color(1, 1, 1, 0);
            glassesButton.image.color = selectedColor;
            hatButton.image.color = unselectedColor;

            selectedHead = imagenameToAnimationString[glassesImage.sprite.name];
            ValidateAccessoryStatus(glassesImage.sprite.name);
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            hat = Resources.Load<Sprite>("Hats/hat" + (currentHat % numberOfHats));
            hatImage.sprite = hat;
            hatImage.color = Color.white;
            glassesImage.sprite = null;
            glassesImage.color = new Color(1, 1, 1, 0);
            glassesButton.image.color = unselectedColor;
            hatButton.image.color = selectedColor;

            selectedHead = imagenameToAnimationString[hatImage.sprite.name];
            ValidateAccessoryStatus(hatImage.sprite.name);
        }

        ValidateCharacterUnlockStatus();
    }

    /// <summary>
    /// Validates if a given accessoryis unlocked or not.
    /// The unlock condition is set through the shop.
    /// </summary>
    private void ValidateAccessoryStatus(string currentImageName)
    {
        bool isLocked = true;

        if (currentImageName == "glasses4" || currentImageName == "hat4")
        {
            isLocked = false;
        }
        else
        {
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
        }

        lockImage.SetActive(isLocked);
    }

    /// <summary>
    /// Validates if a given character outfit is unlocked or not.
    /// Besides the always available outfits, the unlock condition for the others is set through the shop.
    /// </summary>
    private void ValidateCharacterUnlockStatus()
    {
        string characterImageName = "character" + (currentIndex % numberOfCharacters);

        selectedBody = imagenameToAnimationString[characterImageName];

        bool isLocked = true;
        bool isFreeSkin = characterImageName == "character0" || characterImageName == "character1";
        isFreeSkin = isFreeSkin || characterImageName == "character2";

        if (isFreeSkin)
        {
            isLocked = false;
        }

        foreach (var item in shopItemData)
        {
            if (item.GetImageName() == characterImageName)
            {
                if (item.IsBought() || isFreeSkin)
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
        if (previousButton != null && nextButton != null)
        {
            previousButton.interactable = false;
            nextButton.interactable = false;
        }
    }

    /// <summary>
    /// Updates the text displayed below the character depending on what is selected.
    /// This is used to inform the player about restriction in the selection or free outfits.
    /// </summary>
    private void UpdateAccessoryWarnings()
    {
        if (currentIndex == 7)
        {
            DisableAccessoryButtons();
            warningText.text = "Titanium Knight fights alone. No accessories allowed on this mission!";
        }
        else if (currentIndex == 8)
        {
            DisableAccessoryButtons();
            warningText.text = "Santa\'s fashion rule: \"No hats or glasses, just festive cheer!\"";
        }
        else
        {
            warningText.text = "";
            EnableAccessoryButtons();
            UpdateCharacterAndAccessoryVisuals();
            EnableNextPreviousButtons();
        }

        if (currentIndex == 0 || currentIndex == 1 || currentIndex == 2)
        {
            warningText.text = "These outfits are for free!";
        }
    }

    /// <summary>
    /// Disables the buttons to choose between hats and glasses.
    /// This is used if a outfit does not allow accessories.
    /// </summary>
    private void DisableAccessoryButtons()
    {
        glassesButton.interactable = false;
        glassesButton.image.color = Color.gray;

        hatButton.interactable = false;
        hatButton.image.color = Color.gray;
    }

    /// <summary>
    /// Enables the buttons to choose between hats and glasses.
    /// </summary>
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
        if (previousButton != null && nextButton != null)
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
        }
    }

    /// <summary>
    /// Handles the left button to scroll through the accessories.
    /// </summary>
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
        UpdateCharacterAndAccessoryVisuals();
    }

    /// <summary>
    /// Handles the right button to scroll through the accessories.
    /// </summary>
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
        UpdateCharacterAndAccessoryVisuals();
    }

    /// <summary>
    /// Handles the button to switch from glasses to hats.
    /// </summary>
    public void SetAccessoryToHat()
    {
        currentAccessoryType = AccessoryType.Hat;
        UpdateCharacterAndAccessoryVisuals();
        RefreshAccessoryButtonVisuals();
    }

    /// <summary>
    /// Handles the button to switch from hats to glasses.
    /// </summary>
    public void SetAccessoryToGlasses()
    {
        currentAccessoryType = AccessoryType.Glasses;
        UpdateCharacterAndAccessoryVisuals();
        RefreshAccessoryButtonVisuals();
    }

    private void RefreshAccessoryButtonVisuals()
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

    /// <summary>
    /// Handles the left button to scroll through character outfits.
    /// </summary>
    public void PreviousCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex - 1, numberOfCharacters);
        ValidateCharacterUnlockStatus();
        UpdateCharacterAndAccessoryVisuals();
    }

    /// <summary>
    /// Handles the right button to scroll through character outfits.
    /// </summary>
    public void NextCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex + 1, numberOfCharacters);
        ValidateCharacterUnlockStatus();
        UpdateCharacterAndAccessoryVisuals();
    }

    /// <summary>
    /// Handles the scrolling to the previous glasses accessory.
    /// </summary>
    public void Previousglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses - 1, numberOfGlasses);
    }

    /// <summary>
    /// Handles the scrolling to the next glasses accessory.
    /// </summary>
    public void Nextglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses + 1, numberOfGlasses);
    }

    /// <summary>
    /// Handles the scrolling to the previous hat accessory.
    /// </summary>
    public void PreviousHats()
    {
        PlayClickSound();
        currentHat = Modulo(currentHat - 1, numberOfHats);
    }

    /// <summary>
    /// Handles the scrolling to the next hat accessory.
    /// </summary>
    public void NextHats()
    {
        PlayClickSound();
        currentHat = Modulo(currentHat + 1, numberOfHats);
    }


    /// <summary>
    ///     This function is called by the <c>Select Character Button</c>.
    ///     This function switches to the selected character.
    /// </summary>
    public void ConfirmButton()
    {
        DataManager.Instance.SetupCharacter();
        PlayClickSound();

        bool outfitLocked = lockImageOutfit.activeSelf;
        bool glassesLocked = lockImage.activeSelf;
        bool hatLocked = lockImage.activeSelf;

        if (outfitLocked || glassesLocked || hatLocked)
        {
            warningPanel.SetActive(true);
        }
        else
        {
            animationScript.SetOutfitAnimator(selectedBody, selectedHead);
            ownData.SetCurrentCharacter(selectedBody);
            ownData.SetCurrentAccessory(selectedHead);
            DataManager.Instance.UpdateCharacterIndex(selectedBody);
            DataManager.Instance.UpdateAccessoryIndex(selectedHead);

            GameManager.Instance.SavePlayerData();
            GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.SELECT_CHARACTER, 1, null);
        }
    }

    public void CloseWarningPanel()
    {
        warningPanel.SetActive(false);
    }

    /// <summary>
    ///     This function is called by the <c>Navigation Buttons</c>.
    ///     This function plays the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    /// <summary>
    ///     This function returns the positive remainder of the division of an integer by a modulus.
    /// </summary>
    /// <param name="value">An integer that can be positive, negative, or zero.</param>
    /// <param name="modulus">The modulus, which must be a positive integer.</param>
    /// <returns>A positive remainder, which is always between 0 (inclusive) and b (exclusive).</returns>
    private int Modulo(int a, int b)
    {
        int remainder = a % b;
        return remainder < 0 ? remainder + b : remainder;
    }

    /// <summary>
    /// Disables the buttons to navigate through the accessories.
    /// </summary>
    private void DisableAccessoryNavigationButtons()
    {
        if (nextButton != null)
        {
            nextButton.interactable = false;
        }

        if (previousButton != null)
        {
            previousButton.interactable = false;
        }
    }

    /// <summary>
    /// Enables the buttons to navigate through the accessories.
    /// </summary>
    private void EnableAccessoryNavigationButtons()
    {
        if (nextButton != null)
        {
            nextButton.interactable = true;
        }

        if (previousButton != null)
        {
            previousButton.interactable = true;
        }
    }
}