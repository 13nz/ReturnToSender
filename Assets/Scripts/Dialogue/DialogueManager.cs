using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("dialogue ui")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    private static DialogueManager instance;

    private string[] currentLines;
    private int currentLineIndex;
    private bool dialogueActive;

    // prevents the key press that starts a conversation from immediately advancing it.
    private bool waitingForInputRelease;

    // returns whether a conversation is currently active so other systems can pause their interactions.
    public bool IsDialogueActive => dialogueActive;

    private void Awake()
    {
        // prevents duplicate dialogue managers from being created when scenes are loaded.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // stores this manager as the single dialogue manager for the entire game.
        instance = this;

        // keeps the dialogue canvas and manager alive between scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // makes sure the dialogue interface starts hidden when the game begins.
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // stops processing input when no conversation is active.
        if (!dialogueActive || Keyboard.current == null)
            return;

        // waits until the key used to start the conversation has been released.
        if (waitingForInputRelease)
        {
            if (!Keyboard.current.eKey.isPressed &&
                !Keyboard.current.spaceKey.isPressed)
            {
                waitingForInputRelease = false;
            }

            return;
        }

        // advances the conversation when the player presses E or Space.
        if (Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue(string characterName, string[] lines)
    {
        // prevents an empty conversation from opening the dialogue interface.
        if (lines == null || lines.Length == 0)
            return;

        // stores the conversation and starts at the first line.
        currentLines = lines;
        currentLineIndex = 0;
        dialogueActive = true;

        // prevents the interaction key from immediately advancing the first line.
        waitingForInputRelease = true;

        // displays the speaker's name.
        characterNameText.text = characterName;

        // displays the first line before opening the dialogue panel.
        dialogueText.text = currentLines[currentLineIndex];

        // opens the dialogue interface after the first line has been prepared.
        dialoguePanel.SetActive(true);
    }

    private void AdvanceDialogue()
    {
        // moves to the next line of the current conversation.
        currentLineIndex++;

        // closes the dialogue after the final line has been displayed.
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // displays the next conversation line.
        dialogueText.text = currentLines[currentLineIndex];
    }

    private void EndDialogue()
    {
        // marks the conversation as inactive and hides the dialogue interface.
        dialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}