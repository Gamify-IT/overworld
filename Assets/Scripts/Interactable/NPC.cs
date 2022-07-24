using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This script manages NPCs which you can talk to. 
 * If the player is close enough and interacts with the NPC by pressing the 'E' button, a dialogue is started. 
 */
public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject nextButton;
    public float wordSpeed;
    public bool playerIsClose;

    /*
     * The Update function is called every frame. It updates all values according to the changes happened since the last frame. 
     * If the player is close enough and wants to interact with the NPC it starts the dialogue.
     * If the player is already interacting with the NPC the dialogue is ended. 
     * If a sentence is finished, a button to start the next sentence gets set active. 
     */
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if(dialoguePanel.activeInHierarchy)
            {
                ResetText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        if(dialogueText.text == dialogue[index])
        {
            nextButton.SetActive(true);
        }
    }

    /*
     * This function resets and closes the dialogue panel. 
     */
    public void ResetText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    /*
     * This functions manages the text animation. It text appears letter by letter. 
     */
    IEnumerator Typing()
    {
        foreach(char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    /*
     * This function skips to the next sentence. If is was the last one, the dialogue panel gets reset and closed. 
     */
    public void NextLine()
    {

        nextButton.SetActive(false);

        if(index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            ResetText();
        }
    }

    /*
     * This functions notices when the player gets close enough to start a dialogue.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    /*
     * This functions notices when the player is no longer close enough to start a dialogue.
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            ResetText();
        }
    }
}
