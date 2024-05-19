using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Sprite character;
    private GameObject confirmButton;
    private int counter = 1;

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

        confirmButton.GetComponent<Button>().onClick.AddListener(ConfirmButton);
    }

    /// <summary>
    /// The <c>Update</c> function is called once every frame.
    /// This function sets up the character selection menu.
    /// </summary>
    void Update()
    {
        character = Resources.Load<Sprite>("characters/character" + counter);
        characterImage.sprite = character;
        
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
    }

    /// <summary>
    /// This function is called by the <c>Select Character Button</c>.
    /// This function switches to selected character.
    /// </summary>
    public void ConfirmButton()
    {
        //TODO: implement character selection
        GameManager.Instance.SelectCharacter(character);
        Debug.Log(character);

    }
}