using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("interaction settings")]
    [SerializeField] private float interactionRadius = 1f;

    private Interactable currentInteractable;

    private void Update()
    {
        FindInteractable();

        // checks for the interaction key only when an interactable is nearby.
        if (currentInteractable != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentInteractable.Interact();
        }
    }

    private void FindInteractable()
    {
        // searches for colliders within the player's interaction radius.
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius
        );

        Interactable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D nearbyObject in nearbyObjects)
        {
            // gets the reusable interaction component from the nearby object.
            Interactable interactable = nearbyObject.GetComponent<Interactable>();

            if (interactable == null)
                continue;

            // keeps track of the closest valid interactable.
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
    }

    private void OnDrawGizmosSelected()
    {
        // displays the interaction radius in the scene view to make placement easier.
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}