using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("movement settings")]
    [SerializeField] private float moveSpeed = 3f;

    private static PlayerMovement instance;

    private Rigidbody2D rb;
    private Animator animator;

    // stores the direction currently pressed by the player.
    private Vector2 movementInput;

    // stores the last direction the player faced so the correct idle pose is preserved.
    private Vector2 lastDirection = Vector2.down;

    // stores the animation currently playing so it is not restarted every frame.
    private string currentAnimation = "";

    // references the shared dialogue manager so player movement can be disabled during conversations.
    private DialogueManager dialogueManager;

    // references the journal so player movement can be disabled while the journal is open.
    private JournalUI journalUI;

    private void Awake()
    {
        // destroys duplicate players created when a scene containing a player is loaded.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // stores this player as the single persistent player instance.
        instance = this;

        // keeps the player alive when changing between outdoor and interior scenes.
        DontDestroyOnLoad(gameObject);

        // gets the rigidbody responsible for player movement and collision.
        rb = GetComponent<Rigidbody2D>();

        // finds the animator on the player's visual child object.
        animator = GetComponentInChildren<Animator>();

        // finds the shared dialogue manager so movement can be disabled while dialogue is active.
        dialogueManager = FindFirstObjectByType<DialogueManager>();

        // finds the shared journal so movement can be disabled while the journal is open.
        journalUI = FindFirstObjectByType<JournalUI>();
    }

    private void Update()
    {
        // prevents the player from moving while dialogue is active.
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            StopMovement();
            return;
        }

        // prevents the player from moving while the journal is open.
        if (journalUI != null && journalUI.IsJournalOpen)
        {
            StopMovement();
            return;
        }

        // prevents the player from moving while the main menu is open.
        if (Time.timeScale == 0f)
        {
            StopMovement();
            return;
        }

        ReadInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // stops any remaining movement while dialogue is active.
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            StopPhysicsMovement();
            return;
        }

        // stops any remaining movement while the journal is open.
        if (journalUI != null && journalUI.IsJournalOpen)
        {
            StopPhysicsMovement();
            return;
        }

        // stops any remaining movement while the main menu is open.
        if (Time.timeScale == 0f)
        {
            StopPhysicsMovement();
            return;
        }

        MovePlayer();
    }

    private void ReadInput()
    {
        // clears movement input each frame while keeping the last facing direction.
        movementInput = Vector2.zero;

        // stops input processing if no keyboard is available.
        if (Keyboard.current == null)
            return;

        // checks vertical input first so diagonal movement is never possible.
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            movementInput = Vector2.up;
            lastDirection = Vector2.up;
        }
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            movementInput = Vector2.down;
            lastDirection = Vector2.down;
        }
        // checks horizontal input only when no vertical input is being held.
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            movementInput = Vector2.left;
            lastDirection = Vector2.left;
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            movementInput = Vector2.right;
            lastDirection = Vector2.right;
        }
    }

    private void MovePlayer()
    {
        // moves the rigidbody through Unity's physics system so collisions remain active.
        Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void StopMovement()
    {
        // clears movement input and switches the player to the appropriate idle animation.
        movementInput = Vector2.zero;
        StopPhysicsMovement();
        UpdateAnimation();
    }

    private void StopPhysicsMovement()
    {
        // immediately stops any movement that was already applied before the menu or journal opened.
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateAnimation()
    {
        string targetAnimation;

        // selects a walking animation when the player is currently moving.
        if (movementInput != Vector2.zero)
        {
            targetAnimation = GetWalkingAnimation();
        }
        else
        {
            // selects the idle animation that matches the player's last facing direction.
            targetAnimation = GetIdleAnimation();
        }

        // only changes animation states when the requested animation is different.
        if (targetAnimation != currentAnimation)
        {
            animator.Play(targetAnimation);
            currentAnimation = targetAnimation;
        }
    }

    private string GetWalkingAnimation()
    {
        // selects the walking animation directly from the stored direction.
        if (lastDirection == Vector2.up)
            return "walking_up";

        if (lastDirection == Vector2.down)
            return "walking_down";

        if (lastDirection == Vector2.left)
            return "walking_left";

        return "walking_right";
    }

    private string GetIdleAnimation()
    {
        // selects the matching single-frame idle animation when movement stops.
        if (lastDirection == Vector2.up)
            return "idle_up";

        if (lastDirection == Vector2.down)
            return "idle_down";

        if (lastDirection == Vector2.left)
            return "idle_left";

        return "idle_right";
    }
}