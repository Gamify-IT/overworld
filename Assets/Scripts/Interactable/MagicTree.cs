using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class manages the MagicTrees used in World 3.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class MagicTree : MonoBehaviour
{
    public float animationSpeed = 0.02f;

    public List<Sprite> sprites;

    private float animationFrame;

    private Transform player;

    private SpriteRenderer spriteRenderer;

    private float targetAnimationFrame;

    /// <summary>
    ///     This function initializes the component.
    /// </summary>
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Animation>().transform;
    }

    /// <summary>
    ///     This function manages the popup of the magic trees if a player is nearby.
    /// </summary>
    private void FixedUpdate()
    {
        if (Vector3.Distance(player.position, transform.position) < 6)
        {
            targetAnimationFrame = 1;
        }
        else
        {
            targetAnimationFrame = 0;
        }

        if (animationFrame < targetAnimationFrame)
        {
            animationFrame += animationSpeed;
            if (animationFrame > targetAnimationFrame)
            {
                animationFrame = targetAnimationFrame;
            }
        }
        else if (animationFrame > targetAnimationFrame)
        {
            animationFrame -= animationSpeed;
            if (animationFrame < targetAnimationFrame)
            {
                animationFrame = targetAnimationFrame;
            }
        }

        int currentAnimationFrame = Mathf.FloorToInt(animationFrame * (sprites.Count - 1));

        spriteRenderer.sprite = sprites[currentAnimationFrame];
    }
}