using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpeningTutorialManager : MonoBehaviour
{
    public static OpeningTutorialManager Instance { get; private set; }

    public enum TutorialStep
    {
        Move,
        TalkToPostmaster,
        ViewJournal,
        Complete
    }

    [Header("shared ui")]
    [SerializeField] private TMP_Text subtitleText;

    [Header("exit door")]
    [SerializeField] private Collider2D exitDoorCollider;

    [Header("tutorial")]
    [SerializeField] private TutorialStep currentStep;

    private bool tutorialIsActive;

    public TutorialStep CurrentStep => currentStep;
    public bool TutorialIsActive => tutorialIsActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.openingTutorialCompleted)
        {
            CompleteTutorialWithoutShowingSubtitles();
            return;
        }

        BeginTutorial();
    }

    private void Update()
    {
        if (!tutorialIsActive)
        {
            return;
        }

        if (currentStep == TutorialStep.Move && HasPlayerMoved())
        {
            currentStep = TutorialStep.TalkToPostmaster;

            ShowSubtitle("press E to interact with the postmaster");
        }
    }

    private void BeginTutorial()
    {
        tutorialIsActive = true;
        currentStep = TutorialStep.Move;

        LockExitDoor();
        ShowSubtitle("use WASD to move");
    }

    private void CompleteTutorialWithoutShowingSubtitles()
    {
        tutorialIsActive = false;
        currentStep = TutorialStep.Complete;

        UnlockExitDoor();
        HideSubtitle();
    }

    private bool HasPlayerMoved()
    {
        if (Keyboard.current == null)
        {
            return false;
        }

        return Keyboard.current.wKey.isPressed
            || Keyboard.current.aKey.isPressed
            || Keyboard.current.sKey.isPressed
            || Keyboard.current.dKey.isPressed;
    }

    public bool CanTalkToPostmaster()
    {
        return !tutorialIsActive ||
               currentStep == TutorialStep.TalkToPostmaster;
    }

    public bool IsWaitingForPostmaster()
    {
        return tutorialIsActive &&
               currentStep == TutorialStep.TalkToPostmaster;
    }

    public bool IsWaitingForJournal()
    {
        return tutorialIsActive &&
               currentStep == TutorialStep.ViewJournal;
    }

    public void CompletePostmasterConversation()
    {
        if (!tutorialIsActive ||
            currentStep != TutorialStep.TalkToPostmaster)
        {
            return;
        }

        currentStep = TutorialStep.ViewJournal;

        ShowSubtitle("press Tab to view journal");
    }

    public void NotifyJournalOpened()
    {
        if (!tutorialIsActive ||
            currentStep != TutorialStep.ViewJournal)
        {
            return;
        }

        currentStep = TutorialStep.Complete;
        tutorialIsActive = false;

        UnlockExitDoor();

        ShowSubtitle("Press an arrow key to switch documents");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.openingTutorialCompleted = true;
        }
    }

    public void NotifyJournalClosed()
    {
        if (currentStep == TutorialStep.Complete)
        {
            HideSubtitle();
        }
    }

    public void ShowSubtitle(string message)
    {
        if (subtitleText == null)
        {
            return;
        }

        subtitleText.text = message;
        subtitleText.gameObject.SetActive(true);
    }

    public void HideSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.gameObject.SetActive(false);
        }
    }

    private void LockExitDoor()
    {
        if (exitDoorCollider != null)
        {
            exitDoorCollider.enabled = false;
        }
    }

    private void UnlockExitDoor()
    {
        if (exitDoorCollider != null)
        {
            exitDoorCollider.enabled = true;
        }
    }
}