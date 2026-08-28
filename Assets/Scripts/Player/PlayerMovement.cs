using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("movement settings")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;

    // stores the direction currently pressed by the player.
    private Vector2 movementInput;

    // stores the direction the player was most recently facing.
    private int lastDirection = 0;

    // stores the animation state currently being played so it is not restarted every frame.
    private string currentAnimation = "";

    private void Awake()
    {
        // gets the rigidbody used for physics-based player movement.
        rb = GetComponent<Rigidbody2D>();

        // gets the animator from the visual child object.
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadInput()
    {
        movementInput = Vector2.zero;

        // stops input checks if no keyboard is currently available.
        if (Keyboard.current == null)
            return;

        // vertical input takes priority, preventing diagonal movement.
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            movementInput = Vector2.up;
            lastDirection = 1;
        }
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            movementInput = Vector2.down;
            lastDirection = 0;
        }
        // horizontal input is checked only when no vertical key is pressed.
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            movementInput = Vector2.left;
            lastDirection = 2;
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            movementInput = Vector2.right;
            lastDirection = 3;
        }
    }

    private void MovePlayer()
    {
        // moves the rigidbody through the physics system so collisions work correctly.
        Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void UpdateAnimation()
    {
        string targetAnimation;

        // selects the walking animation when the player has movement input.
        if (movementInput != Vector2.zero)
        {
            targetAnimation = GetWalkingAnimation();
        }
        else
        {
            // selects the matching single-frame idle animation when the player stops.
            targetAnimation = GetIdleAnimation();
        }

        // only changes the animator state when the required animation has actually changed.
        if (targetAnimation != currentAnimation)
        {
            animator.Play(targetAnimation);
            currentAnimation = targetAnimation;
        }
    }

    private string GetWalkingAnimation()
    {
        // converts the stored direction into the corresponding walking animation state.
        switch (lastDirection)
        {
            case 1:
                return "walking_up";

            case 2:
                return "walking_left";

            case 3:
                return "walking_right";

            default:
                return "walking_down";
        }
    }

    private string GetIdleAnimation()
    {
        // converts the stored direction into the corresponding idle animation state.
        switch (lastDirection)
        {
            case 1:
                return "idle_up";

            case 2:
                return "idle_left";

            case 3:
                return "idle_right";

            default:
                return "idle_down";
        }
    }
}