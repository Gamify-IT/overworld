using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    private Image characterImage;
    private Sprite character;
    private GameObject confirmButton;
    private int numberOfCharacters = 3;
    private int characterIndex = 0;
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private Sprite[] playerFaces;
    

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
    }

    /// <summary>
    /// The <c>Update</c> function is called once every frame.
    /// This function sets up the character selection menu.
    /// </summary>
    void Update()
    {
        character = Resources.Load<Sprite>("characters/character" + (characterIndex + 1));
        characterImage.sprite = character;
        
    }

    /// <summary>
    /// This function is called by the <c>Previous Character Button</c>.
    /// This function switches to the previous character.
    /// </summary>
    public void PreviousCharacter()
    {
        characterIndex = (characterIndex - 1) % numberOfCharacters;
    }

    /// <summary>
    /// This function is called by the <c>Next Character Button</c>.
    /// This function switches to the next character.
    /// </summary>
    public void NextCharacter()
    {
        characterIndex = (characterIndex + 1) % numberOfCharacters;
    }

    /// <summary>
    /// This function is called by the <c>Select Character Button</c>.
    /// This function switches to selected character.
    /// </summary>
    public void ConfirmButton()
    {
        // current player properties 
        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        Vector3 position = currentPlayer.transform.position;
        Quaternion rotation = currentPlayer.transform.rotation;
        GameObject miniMapCamera = GameObject.Find("Minimap Camera");
        Image playerFace = GameObject.Find("Player Face").GetComponent<Image>();
        //Debug.Log("Player " + characterIndex + " Face");
        
        // reset current character, instance and face
        Destroy(currentPlayer);
        PlayerAnimation.Instance.ResetInstance();
        //currentFace.SetActive(false);
        playerFace.sprite = playerFaces[characterIndex];

        // create new character in player scene 
        GameObject newPlayer = Instantiate(characterPrefabs[characterIndex], position, rotation);
        SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneByName("Player"));

        // add minimap camera to new character 
        miniMapCamera.transform.parent = newPlayer.transform;

        // change minimap face
        GameObject newFace = GameObject.Find("Player " + (characterIndex + 1) + " Face");
        //Debug.Log("Player " + (characterIndex + 1) + " Face");
        //newFace.SetActive(true);
    }
}