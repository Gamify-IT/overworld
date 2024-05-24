using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Sprite character;
    private GameObject confirmButton;
    private int counter = 1;

    public AudioClip clickSound;
    private AudioSource audioSource;

    /// <summary>
    /// The <c>Start</c> function is called after the object is initialized.
    /// This function sets up the references of the object.
    /// </summary>
    void Start()
    {
        //get image component
        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        //get confirm button
        confirmButton = GameObject.Find("Confirm Button");

        //get AudioSource component
        audioSource=GetComponent<AudioSource>();
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
        character = Resources.Load<Sprite>("characters/character" + counter);
        characterImage.sprite = character;
        //enable confirm button for character 1
        if (counter == 1)
        {
            confirmButton.SetActive(true);
        }
        //disable for all other characters since they are not implemented into the game
        else
        {
            confirmButton.SetActive(false);
        }
    }

    /// <summary>
    /// This function is called by the <c>Previous Character Button</c>.
    /// This function switches to the previous character.
    /// </summary>
    public void PreviousCharacter()
    {
        counter -= 1;
        if (counter < 1)
        {
            counter = 4;
        }
        PlayClickSound();
    }

    /// <summary>
    /// This function is called by the <c>Next Character Button</c>.
    /// This function switches to the next character.
    /// </summary>
    public void NextCharacter()
    {
        counter += 1;
        if (counter > 4)
        {
            counter = 1;
        }
        PlayClickSound();
    }

    /// <summary>
    /// This function is called by the <c>Select Character Button</c>.
    /// This function switches to selected character.
    /// </summary>
    public void ConfirmButton()
    {
        PlayClickSound();
        //TODO: implement character selection
        //  -> not part of this project
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
}