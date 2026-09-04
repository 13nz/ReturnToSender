using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("interaction settings")]
    [SerializeField] private float interactionRadius = 1f;

    private Interactable currentInteractable;
    private SpriteRenderer interactionIcon;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        // finds the interaction icon on the player so its visibility can be controlled automatically
        Transform iconTransform = transform.Find("interaction_icon");

        if (iconTransform != null)
        {
            interactionIcon = iconTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("no child named 'interaction_icon' was found on the player.");
        }

        // finds the shared dialogue manager so world interaction can pause during conversations
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    private void Start()
    {
        // hides the interaction icon until an interactable is within range
        SetInteractionIcon(false);
    }

    private void Update()
    {
        // prevents world interactions while a dialogue conversation is active
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            currentInteractable = null;
            SetInteractionIcon(false);
            return;
        }

        FindInteractable();

        // interacts with the closest nearby object when the player presses E
        if (currentInteractable != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            // starts the interaction and stops this update before any other world interaction can occur
            currentInteractable.Interact();
            return;
        }
    }

    private void FindInteractable()
    {
        // searches for colliders within the player's interaction radius
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius
        );

        Interactable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D nearbyObject in nearbyObjects)
        {
            // checks the collider itself first so simple interactables work without extra hierarchy requirements
            Interactable interactable = nearbyObject.GetComponent<Interactable>();

            if (interactable == null)
            {
                // checks the parent in case the collider is on a child object of the interactable
                interactable = nearbyObject.GetComponentInParent<Interactable>();
            }

            if (interactable == null)
                continue;

            // ignores closed doors so they cannot display the interaction icon or be selected
            Door door = interactable as Door;

            if (door != null && !door.IsInteractable)
                continue;

            // measures the distance between the player and the nearby interaction collider
            float distance = Vector2.Distance(
                transform.position,
                nearbyObject.transform.position
            );

            // keeps the closest valid interactable as the current interaction target
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        currentInteractable = closestInteractable;

        // only shows the icon when a valid interactable is within range
        SetInteractionIcon(currentInteractable != null);
    }

    private void SetInteractionIcon(bool visible)
    {
        // changes only the sprite renderer so the interaction icon remains attached to the player
        if (interactionIcon != null)
        {
            interactionIcon.enabled = visible;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // displays the interaction radius in the scene view for easier interaction placement
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}