using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MagicTree : MonoBehaviour
{
    public float animationSpeed = 0.02f;

    public List<Sprite> sprites;

    private float animationFrame = 0;

    private float targetAnimationFrame = 0;

    private SpriteRenderer spriteRenderer;

    private Transform player;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Animation>().transform;
    }

    void FixedUpdate()
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
