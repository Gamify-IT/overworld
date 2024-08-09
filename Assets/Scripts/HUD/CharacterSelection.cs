using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
///     This class opens the <c>character selection</c> menu and includes logic for choosing and selecting new characters.
/// </summary>
public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;

    private int numberOfCharacters;
    private int currentIndex;
    private Image characterImage;
    private AudioSource audioSource;

    /// <summary>
    ///     This function initializes all necessary variables.
    /// </summary>
    void Start()
    {
        GameManager.Instance.SetIsPaused(true);

        numberOfCharacters = DataManager.Instance.GetCharacterSprites().Count;
        characterImage = GameObject.Find("Character Sprite").GetComponent<Image>();
        currentIndex = DataManager.Instance.GetCharacterIndex();
        audioSource=GetComponent<AudioSource>();

        if(audioSource == null)
        {
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip=clickSound;
    }

    /// <summary>
    ///     This function always updates the currently shown character in the carousel.
    /// </summary>
    void Update()
    {
        characterImage.sprite = Resources.Load<Sprite>("characters/character" + (currentIndex % numberOfCharacters));
    }

    /// <summary>
    ///     This function is called by the <c>Previous Character Button</c>.
    ///     This function switches to the previous character.
    /// </summary>
    public void PreviousCharacter()
    {
        PlayClickSound();
        currentIndex = PositiveModulo(currentIndex - 1, numberOfCharacters);
    }

    /// <summary>
    ///     This function is called by the <c>Next Character Button</c>.
    ///     This function switches to the next character.
    /// </summary>
    public void NextCharacter()
    {
        PlayClickSound();
        currentIndex = PositiveModulo(currentIndex + 1, numberOfCharacters);
    }

    /// <summary>
    ///     This function is called by the <c>Select Character Button</c>.
    ///     This function switches to the selected character.
    /// </summary>
    public void ConfirmButton()
    {
        // get player components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SpriteRenderer currentSprite = player.GetComponent<SpriteRenderer>();
        Animator currentAnimator = player.GetComponent<Animator>();
        Image characterHead = GameObject.Find("Player Face").GetComponent<Image>();

        // change the player's sprite, animations and head on the minimap
        currentSprite.sprite = DataManager.Instance.GetCharacterSprites()[currentIndex];
        currentAnimator.runtimeAnimatorController = DataManager.Instance.GetCharacterAnimators()[currentIndex];
        characterHead.sprite = DataManager.Instance.GetCharacterHeads()[currentIndex];

        // save new progress 
        DataManager.Instance.SetCharacterIndex(currentIndex);
        GameManager.Instance.IncreaseAchievementProgress(AchievementTitle.SELECT_CHARACTER, 1, null);
        PlayClickSound();
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
    private int PositiveModulo(int value, int modulus)
    {
        int remainder = value % modulus;
        return remainder < 0 ? remainder + modulus : remainder;
    }

}