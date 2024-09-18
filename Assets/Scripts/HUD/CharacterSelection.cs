using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.U2D;

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

    private int currentIndex = 0;
    private int currentGlasses = 0;
    private int currentHat = 0;

    public enum AccessoryType
    {
        Glasses,
        Hat
    }

    private AccessoryType currentAccessoryType = AccessoryType.Glasses; 

    [SerializeField] private GameObject[] characterPrefabs;

    public Button glassesButton;
    public Button hatButton;

    public AudioClip clickSound;
    private AudioSource audioSource;

    /// <summary>
    /// The <c>Start</c> function is called after the object is initialized.
    /// This function sets up the references of the object.
    /// </summary>
    void Start()
    {
        GameManager.Instance.isPaused = true;
        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        currentIndex = DataManager.Instance.GetCharacterIndex();

        glassesImage = GameObject.Find("Glasses Sprite").GetComponent<Image>();
        currentGlasses = DataManager.Instance.GetGlassesIndex();

        hatImage = GameObject.Find("Hat Sprite").GetComponent<Image>();
        currentHat = DataManager.Instance.GetHatIndex();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;

        UpdateButtonVisuals();
    }

    /// <summary>
    /// The <c>Update</c> function is called once every frame.
    /// This function sets up the character selection menu.
    /// </summary>
    void Update()
    {
        // Load and set character sprite
        character = Resources.Load<Sprite>("characters/character" + (currentIndex % numberOfCharacters));
        characterImage.sprite = character;

        // Only show the accessory that is currently selected
        if (currentAccessoryType == AccessoryType.Glasses)
        {
            glasses = Resources.Load<Sprite>("glasses/brille" + (currentGlasses % numberOfGlasses));
            glassesImage.sprite = glasses;
            glassesImage.color = Color.white; 

            hatImage.sprite = null; 
            hatImage.color = new Color(1, 1, 1, 0); 
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            hat = Resources.Load<Sprite>("hats/hat" + (currentHat % numberOfHats));
            hatImage.sprite = hat;
            hatImage.color = Color.white; 

            glassesImage.sprite = null; 
            glassesImage.color = new Color(1, 1, 1, 0); 
        }
    }

    /// <summary>
    /// Switch to the previous accessory (glasses or hat depending on the current mode).
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
    }

    /// <summary>
    /// Switch to the next accessory (glasses or hat depending on the current mode).
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
    }

    /// <summary>
    /// Set the accessory mode to hats.
    /// </summary>
    public void SetAccessoryToHat()
    {
        currentAccessoryType = AccessoryType.Hat;
        Update(); 
        UpdateButtonVisuals(); 
    }

    /// <summary>
    /// Set the accessory mode to glasses.
    /// </summary>
    public void SetAccessoryToGlasses()
    {
        currentAccessoryType = AccessoryType.Glasses;
        Update(); 
        UpdateButtonVisuals(); 
    }

    /// <summary>
    /// Update the visuals of the buttons to indicate which accessory is active.
    /// </summary>
    private void UpdateButtonVisuals()
    {
        // Define a lighter color for the selected button and a darker color for the unselected button
        Color selectedColor = new Color(1, 1, 1, 1); 
        Color unselectedColor = new Color(1, 1, 1, 0.5f); 

        // Update button colors based on the current accessory type
        if (currentAccessoryType == AccessoryType.Glasses)
        {
            glassesButton.image.color = selectedColor;
            hatButton.image.color = unselectedColor;
        }
        else if (currentAccessoryType == AccessoryType.Hat)
        {
            hatButton.image.color = selectedColor;
            glassesButton.image.color = unselectedColor;
        }
    }

    /// <summary>
    /// Switch to the previous character (left arrow).
    /// </summary>
    public void PreviousCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex - 1, numberOfCharacters);
    }

    /// <summary>
    /// Switch to the next character (right arrow).
    /// </summary>
    public void NextCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex + 1, numberOfCharacters);
    }

    /// <summary>
    /// Switch to the previous glasses.
    /// </summary>
    public void Previousglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses - 1, numberOfGlasses);
    }

    /// <summary>
    /// Switch to the next glasses.
    /// </summary>
    public void Nextglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses + 1, numberOfGlasses);
    }

    /// <summary>
    /// Switch to the previous hat.
    /// </summary>
    public void PreviousHats()
    {
        PlayClickSound();
        currentHat = Modulo(currentHat - 1, numberOfHats);
    }

    /// <summary>
    /// Switch to the next hat.
    /// </summary>
    public void NextHats()
    {
        PlayClickSound();
        currentHat = Modulo(currentHat + 1, numberOfHats);
    }

    /// <summary>
    /// Confirm the selected character and accessories.
    /// </summary>
    public void ConfirmButton()
    {
        // current player properties
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        Vector3 position = currentPlayer.transform.position;
        Quaternion rotation = currentPlayer.transform.rotation;
        GameObject miniMapCamera = GameObject.Find("Minimap Camera");
        Image playerFace = GameObject.Find("Player Face").GetComponent<Image>();
        PixelPerfectCamera pixelCam = currentPlayer.GetComponentInChildren<PixelPerfectCamera>();

        // reset current character, instance and face
        Destroy(currentPlayer);
        PlayerAnimation.Instance.ResetInstance();
        playerFace.sprite = DataManager.Instance.GetCharacterFaces()[currentIndex];

        // create new character in player scene 
        GameObject newPlayer = Instantiate(characterPrefabs[currentIndex], position, rotation);
        SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneByName("Player"));
        DataManager.Instance.SetCharacterIndex(currentIndex);

        // add minimap camera to new character 
        miniMapCamera.transform.parent = newPlayer.transform;
        miniMapCamera.GetComponent<Camera>().enabled = true;
        miniMapCamera.GetComponent<ZoomScript>().enabled = true;

        // adjust main camera
        PixelPerfectCamera newPixelCam = newPlayer.GetComponentInChildren<PixelPerfectCamera>();
        ZoomScript.Instance.ChangePixelCam(newPixelCam);
        newPixelCam.refResolutionX = pixelCam.refResolutionX;
        newPixelCam.refResolutionY = pixelCam.refResolutionY;

        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.SELECT_CHARACTER, 1);
        PlayClickSound();
    }

    /// <summary>
    /// Play the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    /// <summary>
    /// Realize the modulo operator for positive remainders.
    /// </summary>
    private int Modulo(int a, int b)
    {
        int r = a % b;
        return r < 0 ? r + b : r;
    }
}
