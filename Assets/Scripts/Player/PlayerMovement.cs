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
        // moves the rigidbody so the player interacts correctly with 2d colliders.
        Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void UpdateAnimation()
    {
        // chooses the walking animation while the player is moving.
        if (movementInput != Vector2.zero)
        {
            switch (lastDirection)
            {
                case 0:
                    animator.Play("walking_down");
                    break;

                case 1:
                    animator.Play("walking_up");
                    break;

                case 2:
                    animator.Play("walking_left");
                    break;

                case 3:
                    animator.Play("walking_right");
                    break;
            }
        }
        else
        {
            // chooses the matching single-frame idle animation when movement stops.
            switch (lastDirection)
            {
                case 0:
                    animator.Play("idle_down");
                    break;

                case 1:
                    animator.Play("idle_up");
                    break;

                case 2:
                    animator.Play("idle_left");
                    break;

                case 3:
                    animator.Play("idle_right");
                    break;
            }
        }
    }
}