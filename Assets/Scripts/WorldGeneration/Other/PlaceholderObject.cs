using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private PlaceholderType type;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite minigameSprite;
    [SerializeField] private Sprite npcSprite;
    [SerializeField] private Sprite bookSprite;
    [SerializeField] private Sprite teleporterSprite;
    [SerializeField] private Sprite sceneTransitionSprite;
    [SerializeField] private Sprite barrierSprite;

    public void Setup(PlaceholderType type)
    {
        this.type = type;

        switch(type)
        {
            case PlaceholderType.UNSET:
                spriteRenderer.sprite = null;
                break;

            case PlaceholderType.MINIGAME:
                spriteRenderer.sprite = minigameSprite;
                break;

            case PlaceholderType.NPC:
                spriteRenderer.sprite = npcSprite;
                break;

            case PlaceholderType.BOOK:
                spriteRenderer.sprite = bookSprite;
                break;

            case PlaceholderType.TELEPORTER:
                spriteRenderer.sprite = teleporterSprite;
                break;

            case PlaceholderType.SCENE_TRANSITION:
                spriteRenderer.sprite = sceneTransitionSprite;
                break;

            case PlaceholderType.BARRIER:
                spriteRenderer.sprite = barrierSprite;
                break;
        }
    }
}
