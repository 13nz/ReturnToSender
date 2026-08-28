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

        // stores this dialogue manager as the single manager for the entire game.
        instance = this;

        // keeps the dialogue manager and its canvas hierarchy alive between scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // makes sure the dialogue interface starts hidden when the game begins.
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // advances the conversation when the player presses E or Space.
        if (dialogueActive &&
            Keyboard.current != null &&
            (Keyboard.current.eKey.wasPressedThisFrame ||
             Keyboard.current.spaceKey.wasPressedThisFrame))
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue(string characterName, string[] lines)
    {
        // prevents an empty conversation from opening the dialogue interface.
        if (lines == null || lines.Length == 0)
            return;

        // stores the conversation so each line can be displayed in order.
        currentLines = lines;
        currentLineIndex = 0;
        dialogueActive = true;

        // displays the speaker's name and opens the dialogue interface.
        characterNameText.text = characterName;
        dialoguePanel.SetActive(true);

        // displays the first line of the conversation.
        ShowCurrentLine();
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
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        // updates the dialogue text with the current conversation line.
        dialogueText.text = currentLines[currentLineIndex];
    }

    private void EndDialogue()
    {
        // marks the conversation as inactive and hides the dialogue interface.
        dialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}