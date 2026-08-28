using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("interaction settings")]
    [SerializeField] private string interactionName = "Interact";

    // returns the text that can be used by the interaction prompt or other ui.
    public string GetInteractionName()
    {
        return interactionName;
    }

    // defines the action that occurs when the player interacts with this object.
    // individual interactables can later override this behavior with their own components.
    public virtual void Interact()
    {
        Debug.Log($"interacted with {gameObject.name}");
    }
}