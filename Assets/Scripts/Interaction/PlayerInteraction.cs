using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("interaction settings")]
    [SerializeField] private float interactionRadius = 1f;

    private Interactable currentInteractable;
    private SpriteRenderer interactionIcon;

    private void Awake()
    {
        // finds the interaction icon on the player so its visibility can be controlled automatically.
        Transform iconTransform = transform.Find("interaction_icon");

        if (iconTransform != null)
        {
            interactionIcon = iconTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("no child named 'interaction_icon' was found on the player.");
        }
    }

    private void Start()
    {
        // hides the icon when the game starts because no interaction has been detected yet.
        SetInteractionIcon(false);
    }

    private void Update()
    {
        FindInteractable();

        // allows the player to interact with the closest nearby object using the E key.
        if (currentInteractable != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentInteractable.Interact();
        }
    }

    private void FindInteractable()
    {
        // searches for all colliders within the player's interaction radius.
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius
        );

        Interactable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D nearbyObject in nearbyObjects)
        {
            // gets the interaction component from the nearby object or its parent.
            Interactable interactable = nearbyObject.GetComponent<Interactable>();

            if (interactable == null)
            {
                interactable = nearbyObject.GetComponentInParent<Interactable>();
            }

            if (interactable == null)
                continue;

            // calculates the distance to the interaction point so the closest one can be selected.
            float distance = Vector2.Distance(
                transform.position,
                nearbyObject.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        currentInteractable = closestInteractable;

        // shows the icon only when an interactable is within range.
        SetInteractionIcon(currentInteractable != null);
    }

    private void SetInteractionIcon(bool visible)
    {
        // safely updates the icon without enabling or disabling the player object.
        if (interactionIcon != null)
        {
            interactionIcon.enabled = visible;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // displays the interaction radius in the scene view to make interaction placement easier.
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}