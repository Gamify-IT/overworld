using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
