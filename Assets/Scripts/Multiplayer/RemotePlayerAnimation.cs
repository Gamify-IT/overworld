using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Manages the movement and animations of remote player, analogous to the <c>PlayerAnimation</c> class for the local player.
/// </summary>
public class RemotePlayerAnimation : MonoBehaviour
{
    // movement
    private Vector2 movement = Vector2.zero;
    private Vector2 newPosition = Vector2.zero;
    private Rigidbody2D playerRigidBody;

    // animation
    private Animator playerAnimator;
    private Animator accessoireAnimator;
    public Transform accessoireTransform;

    // timeout 
    private float lastMessageTime;
    private readonly float timeoutDuration = 0.2f;

    // outfits
    readonly Dictionary<string, Vector3> positions = new();

    /// <summary>
    ///     Initializes the remote players components and outfits.
    /// </summary>
    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();


        accessoireAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        accessoireTransform = gameObject.transform.GetChild(0).GetComponent<Transform>();

        InitializeOutfitPositions();

        // TODO: reconstruct outfit with character index from received message

        SetOutfitAnimator("character_default", "none");
    }

    #region outfit animator
    /// <summary>
    ///     Initializes the position of each outfit.
    /// </summary>
    private void InitializeOutfitPositions()
    {
        positions.Add("3D_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("blonde_hair", new Vector3(0, 0.3f, 0));
        positions.Add("cool_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("flaming_hair", new Vector3(0, 0.3f, 0));
        positions.Add("globe_hat", new Vector3(0, 0.76f, 0));
        positions.Add("heart_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("retro_glasses", new Vector3(0, 0.3f, 0));
        positions.Add("safety_helmet", new Vector3(0, 0.31f, 0));
        positions.Add("none", new Vector3(0, 0, 0));
    }

    /// <summary>
    ///     This function changes the animation of the character based on the outfit selected in the 
    ///     character selection. Also adjusts the hitbox or scaling for certain outfits.
    /// </summary>
    /// <param name="body"> A string that needs to match one from the validOutfits list. It determines which body animator to use. </param>
    /// <param name="head"> A string that needs to match one from the validOutfits list. It determines which accessory animator to use. </param>
    public void SetOutfitAnimator(string body, string head)
    {
        if (!PlayerAnimation.validOutfits.Contains(body) || !PlayerAnimation.validOutfits.Contains(head))
        {
            throw new ArgumentException("Provided argument(s) are invalid, provide an existing name of a outfit.");
        }

        string bodyPath = "AnimatorControllers/" + body;
        string headPath = "AnimatorControllers/" + head;
        accessoireAnimator.runtimeAnimatorController = Resources.Load(headPath) as RuntimeAnimatorController;
        playerAnimator.runtimeAnimatorController = Resources.Load(bodyPath) as RuntimeAnimatorController;

        if (head == "safety_helmet")
        {
            accessoireTransform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        }
        else
        {
            accessoireTransform.localScale = new Vector3(1, 1, 1);
        }

        if (new List<string> { "character_default", "character_blue_and_purple", "character_black_and_white" }.Contains(body))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.05f);
            accessoireTransform.localPosition = positions[head];
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.25f);
            accessoireTransform.localPosition = positions[head] - new Vector3(0, 0.3f, 0);
        }
    }
    #endregion

    /// <summary>
    ///     Updates the remote players position and animation based on the received messages.
    /// </summary>
    private void FixedUpdate()
    {
        if (Time.unscaledTime - lastMessageTime > timeoutDuration)
        {
            movement = Vector2.zero;
        }

        // TODO: add clipping mechanics

        float horizontalAnimationFloat = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
        float verticalAnimationFloat = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

        playerRigidBody.MovePosition(newPosition);

        SetLookingDirection();

        playerAnimator.SetFloat("Horizontal", movement.x);
        playerAnimator.SetFloat("Vertical", movement.y);
        playerAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
        playerAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
        accessoireAnimator.SetFloat("Horizontal", movement.x);
        accessoireAnimator.SetFloat("Vertical", movement.y);
        accessoireAnimator.SetFloat("VerticalSpeed", verticalAnimationFloat);
        accessoireAnimator.SetFloat("HorizontalSpeed", horizontalAnimationFloat);
    }

    /// <summary>
    ///     Sets the looking direction of the remote player, required for animation.
    /// </summary>
    protected void SetLookingDirection()
    {
        if (movement.y > 0.01f)
        {
            playerAnimator.SetBool("LookUp", true);
            playerAnimator.SetBool("LookRight", false);
            accessoireAnimator.SetBool("LookUp", true);
            accessoireAnimator.SetBool("LookRight", false);
        }

        if (movement.y < 0f)
        {
            playerAnimator.SetBool("LookUp", false);
            playerAnimator.SetBool("LookRight", false);
            accessoireAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookRight", false);
        }

        if (movement.x > 0.01f)
        {
            playerAnimator.SetBool("LookRight", true);
            playerAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookRight", true);
        }

        if (movement.x < 0f)
        {
            playerAnimator.SetBool("LookRight", false);
            playerAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookUp", false);
            accessoireAnimator.SetBool("LookRight", false);
        }
    }

    /// <summary>
    ///     Updates position and movement of the player.
    /// </summary>
    /// <param name="position">players new position</param>
    /// <param name="movement">players normalized movement vector</param>
    public void UpdatePosition(Vector2 position, Vector2 movement)
    {
        lastMessageTime = Time.unscaledTime;
        newPosition = position;
        this.movement = movement;
    }
}