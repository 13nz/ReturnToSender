using UnityEngine;

public class OldTreeInteraction : Interactable
{
    [Header("ending settings")]
    [SerializeField] private string requiredNpcId = "lighthouse";

    private bool endingStarted;

    public override void Interact()
    {
        // prevents the ending from being triggered more than once.
        if (endingStarted)
            return;

        // makes sure the game manager is available.
        if (GameManager.Instance == null)
            return;

        // prevents the player from triggering the ending before the lighthouse keeper has been spoken to.
        if (!GameManager.Instance.HasSpokenToNPC(requiredNpcId))
            return;

        endingStarted = true;

        // starts the ending sequence.
        EndingSequence endingSequence = FindFirstObjectByType<EndingSequence>();

        if (endingSequence != null)
        {
            endingSequence.StartEnding();
        }
        else
        {
            Debug.LogWarning("no ending sequence was found in the scene.");
        }
    }
}