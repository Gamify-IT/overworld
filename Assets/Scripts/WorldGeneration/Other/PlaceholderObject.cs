using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This enum contains all possible placeholder icon types
/// </summary>
public enum PlaceholderType
{
    UNSET,
    MINIGAME,
    NPC,
    BOOK,
    TELEPORTER,
    SCENE_TRANSITION,
    BARRIER
}

/// <summary>
///     This class describes an placeholder icon used in the generator and inspector modes
/// </summary>
public class PlaceholderObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSpriteRenderer;
    [SerializeField] private SpriteRenderer tensSpriteRenderer;
    [SerializeField] private SpriteRenderer onesSpriteRenderer;

    [SerializeField] private Sprite minigameSprite;
    [SerializeField] private Sprite npcSprite;
    [SerializeField] private Sprite bookSprite;
    [SerializeField] private Sprite teleporterSprite;
    [SerializeField] private Sprite sceneTransitionSprite;
    [SerializeField] private Sprite barrierSprite;

    [SerializeField] private List<Sprite> digits;

    /// <summary>
    ///     This function sets up the placeholder icon with the given type and index
    /// </summary>
    /// <param name="type">The type of the placeholder icon</param>
    /// <param name="index">The index of the placeholder icon</param>
    public void Setup(PlaceholderType type, int index)
    {
        switch(type)
        {
            case PlaceholderType.UNSET:
                iconSpriteRenderer.sprite = null;
                break;

            case PlaceholderType.MINIGAME:
                iconSpriteRenderer.sprite = minigameSprite;
                break;

            case PlaceholderType.NPC:
                iconSpriteRenderer.sprite = npcSprite;
                break;

            case PlaceholderType.BOOK:
                iconSpriteRenderer.sprite = bookSprite;
                break;

            case PlaceholderType.TELEPORTER:
                iconSpriteRenderer.sprite = teleporterSprite;
                break;

            case PlaceholderType.SCENE_TRANSITION:
                iconSpriteRenderer.sprite = sceneTransitionSprite;
                break;

            case PlaceholderType.BARRIER:
                iconSpriteRenderer.sprite = barrierSprite;
                break;
        }

        if(index > 0 && index < 100)
        {
            int tens = index / 10;
            if(tens != 0)
            {
                tensSpriteRenderer.sprite = digits[tens];
            }            

            int ones = index % 10;
            onesSpriteRenderer.sprite = digits[ones];
        }        
    }
}
