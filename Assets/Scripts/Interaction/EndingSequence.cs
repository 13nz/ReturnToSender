using UnityEngine;

public class EndingSequence : MonoBehaviour
{
    [Header("ending animation")]
    [SerializeField] private Animator chipalopeAnimator;

    private bool endingStarted;

    public void StartEnding()
    {
        // prevents the ending from starting more than once.
        if (endingStarted)
            return;

        endingStarted = true;

        // stops the player from interacting with anything else during the ending.
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // starts the chipalope ending animation.
        if (chipalopeAnimator != null)
        {
            chipalopeAnimator.SetTrigger("appear");
        }
        else
        {
            Debug.LogWarning("no chipalope animator has been assigned.");
        }
    }
}