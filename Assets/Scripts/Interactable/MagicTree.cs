using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
/*
 * This script manages animated trees. 
 * If the player is too far away, they are just roots.
 * If the player gets close enough, they grow really fast. 
 */
public class MagicTree : MonoBehaviour
{
    public float animationSpeed = 0.02f;
    public List<Sprite> sprites;
    private float animationFrame = 0;
    private float targetAnimationFrame = 0;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    /*
     * The Start function is called when the object is initialized and sets up the starting values and state of the object. 
     */
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Animation>().transform;
    }

    /*
     * The FixedUpdate function is called every physics step. It updates all values according to the changes happened since the last physics engine update. 
     * If the player is close enough, the growing animation is triggered and the shown sprite gets updated. 
     */
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