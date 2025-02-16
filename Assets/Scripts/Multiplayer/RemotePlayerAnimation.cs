using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Manages the movement and animations of remote player, analogous to the <c>PlayerAnimation</c> class for the local player.
/// </summary>
public class RemotePlayerAnimation : MonoBehaviour
{
    // movement
    private Vector2 movement;
    private Vector2 position;
    private Rigidbody2D playerRigidBody;

    // animation
    private Animator playerAnimator;
    private Animator accessoireAnimator;
    private Transform accessoireTransform;

    // timeout 
    private float lastMessageTime;
    private readonly float timeoutDuration = 0.2f;

    // outfits
    private Dictionary<string, Vector3> positions = new();


    // clipping
    private AreaLocationDTO areaInformation;
    private readonly float tolerance = 42f;
    private bool isActive = true;

    /// <summary>
    ///     Initializes the remote players components and outfits.
    /// </summary>
    public void Initialize()
    {
        position = transform.position;
        movement = Vector2.zero;

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        accessoireAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        accessoireTransform = gameObject.transform.GetChild(0).GetComponent<Transform>();

        InitializeOutfitPositions();
    }

    /// <summary>
    ///     Updates the remote players position and animation based on the received messages.
    /// </summary>
    private void FixedUpdate()
    {
        if (Time.unscaledTime - lastMessageTime > timeoutDuration)
        {
            movement = Vector2.zero;
        }

        if (IsPlayerInView())
        {
            if (!isActive) ActivateComponents();

            float horizontalAnimationFloat = movement.x > 0.01f ? movement.x : movement.x < -0.01f ? 1 : 0;
            float verticalAnimationFloat = movement.y > 0.01f ? movement.y : movement.y < -0.01f ? 1 : 0;

            playerRigidBody.MovePosition(position);

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
        else
        {
            DeactivateComponents();
        }
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
    public void SetOutfitAnimator(string head, string body)
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

        accessoireTransform.localPosition = positions[head];
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
    #endregion

    /// <summary>
    ///     Checks whether a remote player is in the players view frustum. 
    /// </summary>
    /// <returns>true if the player is visible</returns>
    private bool IsPlayerInView()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(position.x, position.y, 0));
        return screenPosition.x >= -tolerance && screenPosition.x <= Screen.width + tolerance &&
                screenPosition.y >= -tolerance && screenPosition.y <= Screen.height + tolerance &&
                DataManager.Instance.GetPlayerData().GetCurrentArea().Equals(areaInformation);
    }

    /// <summary>
    ///     Deactivates the remote players components.
    /// </summary>
    private void DeactivateComponents()
    {
        isActive = false;
        playerAnimator.enabled = false;
        accessoireAnimator.enabled = false;
        playerRigidBody.isKinematic = true;
    }

    /// <summary>
    ///     Activates the remote players components.
    /// </summary>
    private void ActivateComponents()
    {
        isActive = true;
        playerAnimator.enabled = true;
        accessoireAnimator.enabled = true;
        playerRigidBody.isKinematic = false;
    }

    /// <summary>
    ///     Updates position and movement of the player.
    /// </summary>
    /// <param name="position">player's new position</param>
    /// <param name="movement">player's normalized movement vector</param>
    public void UpdatePosition(Vector2 position, Vector2 movement)
    {
        lastMessageTime = Time.unscaledTime;
        this.position = position;
        this.movement = movement;
    }

    /// <summary>
    ///     Updates head and body of the player.
    /// </summary>
    /// <param name="head">player's head</param>
    /// <param name="body">player's body</param>
    public void UpdateCharacterOutfit(string head, string body)
    {
        SetOutfitAnimator(head, body);
    }

    /// <summary>
    ///     Updates the current area information of the player.
    /// </summary>
    public void UpdateAreaInformation(byte worldIndex, byte dungeonIndex)
    {
        areaInformation = new(worldIndex, dungeonIndex);
    }
}