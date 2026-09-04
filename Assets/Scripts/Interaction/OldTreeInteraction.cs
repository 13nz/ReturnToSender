using UnityEngine;
using UnityEngine.InputSystem;

public class OldTreeInteraction : Interactable
{
    [Header("ending settings")]
    [SerializeField] private string requiredNpcId = "lighthouse";

    [Header("testing")]
    // allows me to test the animation & adjust without goinf thru the whole game
    [SerializeField] private bool allowTestingWithoutProgression = true;

    private bool endingStarted;

    private void Update()
    {
        // allows the ending to be tested without full game progression
        if (allowTestingWithoutProgression &&
            Keyboard.current != null &&
            Keyboard.current.tKey.wasPressedThisFrame)
        {
            StartEnding();
        }
    }

    public override void Interact()
    {
        // starts the ending through the normal interaction system
        StartEnding();
    }

    private void StartEnding()
    {
        // prevents the ending from being triggered more than once
        if (endingStarted)
            return;

        // skips the progression requirement only while testing is enabled
        if (!allowTestingWithoutProgression)
        {
            if (GameManager.Instance == null)
                return;

            if (!GameManager.Instance.HasSpokenToNPC(requiredNpcId))
                return;
        }

        endingStarted = true;

        // finds the ending sequence attached to OldTree
        EndingSequence endingSequence =
            GetComponent<EndingSequence>();

        if (endingSequence != null)
        {
            endingSequence.StartEnding();
        }
        else
        {
            Debug.LogWarning(
                "no EndingSequence component was found on OldTree."
            );
        }
    }
}