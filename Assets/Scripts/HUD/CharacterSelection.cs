using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.U2D;

public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Image glassesImage;
    private Sprite character;
    private Sprite glasses;
    private int numberOfCharacters = 3;
    private int numberOfGlasses = 4;
    private int numberOfHats = 2;

    private int currentIndex = 0; 
    private int currentGlasses = 0;
    private int currentHat = 0;


    [SerializeField] private GameObject[] characterPrefabs;
    

    public AudioClip clickSound;
    private AudioSource audioSource;

    /// <summary>
    /// The <c>Start</c> function is called after the object is initialized.
    /// This function sets up the references of the object.
    /// </summary>
    void Start()
    {
        GameManager.Instance.isPaused = true;
        //get image component
        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        //get the index of the currently selected character 
        currentIndex = DataManager.Instance.GetCharacterIndex();

        glassesImage = GameObject.Find("Glasses Sprite").GetComponent<Image>();
        currentGlasses = DataManager.Instance.GetGlassesIndex();


        //get AudioSource component
        audioSource = GetComponent<AudioSource>();
        //add AudioSource component if necessary
        if(audioSource == null)
        {
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        //set audio clip
        audioSource.clip=clickSound;
    }

    /// <summary>
    /// The <c>Update</c> function is called once every frame.
    /// This function sets up the character selection menu.
    /// </summary>
    void Update()
    {
        character = Resources.Load<Sprite>("characters/character" + (currentIndex % numberOfCharacters));
        characterImage.sprite = character;

        glasses = Resources.Load<Sprite>("glasses/brille" + (currentGlasses % numberOfGlasses));
        glassesImage.sprite = glasses;

    }

    /// <summary>
    /// This function is called by the <c>Previous Character Button</c>.
    /// This function switches to the previous character.
    /// </summary>
    public void PreviousCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex - 1, numberOfCharacters);
    }


    /// <summary>
    /// This function is called by the <c>Previous Character Button</c>.
    /// This function switches to the previous character.
    /// </summary>
    public void Previousglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses - 1, numberOfGlasses);
    }

    /// <summary>
    /// This function is called by the <c>Next Character Button</c>.
    /// This function switches to the next character.
    /// </summary>
    public void NextCharacter()
    {
        PlayClickSound();
        currentIndex = Modulo(currentIndex + 1, numberOfCharacters);
    }

    /// <summary>
    /// This function is called by the <c>Next Character Button</c>.
    /// This function switches to the next character.
    /// </summary>
    public void Nextglasses()
    {
        PlayClickSound();
        currentGlasses = Modulo(currentGlasses + 1, numberOfGlasses);
    }

    /// <summary>
    /// This function is called by the <c>Select Character Button</c>.
    /// This function switches to the selected character.
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
    /// This function is called by the <c>Navigation Buttons</c>.
    /// This function plays the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
        

    /// <summary>
    ///     This method realizes the modulo operator from modular arithmetic.
    /// </summary>
    /// <param name="a">arbitrary number</param>
    /// <param name="b">modulus</param>
    /// <returns>positive remainder</returns>
    private int Modulo(int a, int b)
    {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

}