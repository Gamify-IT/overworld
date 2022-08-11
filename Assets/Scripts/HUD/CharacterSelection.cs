using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Sprite character;
    private GameObject confirmButton;
    private int counter = 1;

    void Start()
    {
        //get image component
        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        //get confirm button
        confirmButton = GameObject.Find("Confirm Button");
    }

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

    public void PreviousCharacter()
    {
        counter -= 1;
        if (counter < 1)
        {
            counter = 4;
        }
    }

    public void NextCharacter()
    {
        counter += 1;
        if (counter > 4)
        {
            counter = 1;
        }
    }

    public void ConfirmButton()
    {
        //code that is executed when the Confirm button is pressed
        //currently empty since there is no character selection functionality
    }
}