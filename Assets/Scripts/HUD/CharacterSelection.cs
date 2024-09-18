using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
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
    public TextMeshProUGUI warningText; 

    public AudioClip clickSound;
    private AudioSource audioSource;

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
        UpdateWarnings(); 
    }

    void Update()
    {
        character = Resources.Load<Sprite>("characters/character" + (currentIndex % numberOfCharacters));
        characterImage.sprite = character;

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

        UpdateWarnings(); 
    }

    private void UpdateWarnings()
    {
        if (currentIndex == 6) 
        {
            warningText.text = "Looks like Iron Man's suit prefers to go solo, no hats or glasses with this one!";
            glassesButton.interactable = false;
            hatButton.interactable = false;

            glassesImage.sprite = null;
            hatImage.sprite = null;
            glassesImage.color = new Color(1, 1, 1, 0); 
            hatImage.color = new Color(1, 1, 1, 0); 
        }
        else
        {
            warningText.text = "";
            glassesButton.interactable = true;
            hatButton.interactable = true;

            if (currentAccessoryType == AccessoryType.Glasses)
            {
                glassesImage.sprite = Resources.Load<Sprite>("glasses/brille" + (currentGlasses % numberOfGlasses));
                glassesImage.color = Color.white;
            }
            else if (currentAccessoryType == AccessoryType.Hat)
            {
                hatImage.sprite = Resources.Load<Sprite>("hats/hat" + (currentHat % numberOfHats));
                hatImage.color = Color.white;
            }
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
    }

    public void SetAccessoryToHat()
    {
        currentAccessoryType = AccessoryType.Hat;
        Update();
        UpdateButtonVisuals();
    }

    public void SetAccessoryToGlasses()
    {
        currentAccessoryType = AccessoryType.Glasses;
        Update();
        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        Color selectedColor = new Color(1, 1, 1, 1);
        Color unselectedColor = new Color(1, 1, 1, 0.5f);

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

    public void PreviousCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex - 1, numberOfCharacters);
    }

    public void NextCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex + 1, numberOfCharacters);
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
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        Vector3 position = currentPlayer.transform.position;
        Quaternion rotation = currentPlayer.transform.rotation;
        GameObject miniMapCamera = GameObject.Find("Minimap Camera");
        Image playerFace = GameObject.Find("Player Face").GetComponent<Image>();
        PixelPerfectCamera pixelCam = currentPlayer.GetComponentInChildren<PixelPerfectCamera>();

        Destroy(currentPlayer);
        PlayerAnimation.Instance.ResetInstance();
        playerFace.sprite = DataManager.Instance.GetCharacterFaces()[currentIndex];

        GameObject newPlayer = Instantiate(characterPrefabs[currentIndex], position, rotation);
        SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneByName("Player"));
        DataManager.Instance.SetCharacterIndex(currentIndex);

        miniMapCamera.transform.parent = newPlayer.transform;
        miniMapCamera.GetComponent<Camera>().enabled = true;
        miniMapCamera.GetComponent<ZoomScript>().enabled = true;

        PixelPerfectCamera newPixelCam = newPlayer.GetComponentInChildren<PixelPerfectCamera>();
        ZoomScript.Instance.ChangePixelCam(newPixelCam);
        newPixelCam.refResolutionX = pixelCam.refResolutionX;
        newPixelCam.refResolutionY = pixelCam.refResolutionY;

        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.SELECT_CHARACTER, 1);
        PlayClickSound();
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
}
