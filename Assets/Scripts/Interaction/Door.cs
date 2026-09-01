using UnityEngine;

public class Door : Interactable
{
    [Header("door settings")]
    [SerializeField] private string destinationScene;
    [SerializeField] private string doorId;

    // stores whether this door can currently be interacted with.
    private bool interactable = true;

    // provides the door id so the transition manager can find the matching door after a scene change.
    public string DoorId => doorId;

    // provides the current interaction state so the player interaction system
    // knows whether to show the interaction icon.
    public bool IsInteractable => interactable;

    // enables or disables interaction with this door.
    public void SetInteractable(bool canInteract)
    {
        interactable = canInteract;
    }

    // starts a scene transition when the player interacts with this door.
    public override void Interact()
    {
        // prevents the door from being used while the building is closed.
        if (!interactable)
            return;

        // prevents a missing destination scene from causing a confusing scene-loading error.
        if (string.IsNullOrWhiteSpace(destinationScene))
        {
            Debug.LogWarning($"no destination scene assigned to {gameObject.name}.");
            return;
        }

        // prevents the destination scene from loading without a matching arrival door.
        if (string.IsNullOrWhiteSpace(doorId))
        {
            Debug.LogWarning($"no door id assigned to {gameObject.name}.");
            return;
        }

        // asks the persistent transition manager to load the destination and remember this door id.
        SceneTransitionManager.LoadScene(destinationScene, doorId);
    }
}