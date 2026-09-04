using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("dialogue ui")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("typing effect")]
    [SerializeField] private float charactersPerSecond = 45f;
    [SerializeField] private AudioClip typingSound;

    private AudioSource typingAudioSource;
    private Coroutine typingCoroutine;

    private bool isTyping;
    private string currentLine;

    private static DialogueManager instance;

    private string[] currentLines;
    private int currentLineIndex;
    private bool dialogueActive;

    // prevents the key press that starts a conversation from immediately advancing it
    private bool waitingForInputRelease;

    // returns whether a conversation is currently active
    public bool IsDialogueActive => dialogueActive;

    // checks whether the current conversation is the postmaster conversation
    private bool currentConversationIsPostmaster;

    private void Awake()
    {
        // prevents duplicate dialogue managers from being created when scenes are loaded
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // stores this manager as the single dialogue manager for the entire game
        instance = this;

        // gets the audio source attached to the same dialogue canvas
        typingAudioSource = GetComponent<AudioSource>();

        // keeps the dialogue canvas and manager alive between scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // makes sure the dialogue interface starts hidden when the game begins
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // configures the typing audio source
        if (typingAudioSource != null)
        {
            typingAudioSource.playOnAwake = false;
            typingAudioSource.loop = true;
        }
    }

    private void Update()
    {
        // stops processing input when no conversation is active
        if (!dialogueActive || Keyboard.current == null)
        {
            return;
        }

        // waits until the key used to start the conversation has been released
        if (waitingForInputRelease)
        {
            if (!Keyboard.current.eKey.isPressed &&
                !Keyboard.current.spaceKey.isPressed)
            {
                waitingForInputRelease = false;
            }

            return;
        }

        // advances or completes the current line when the player presses space or e
        if (Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue(
        string characterName,
        string[] lines,
        bool isPostmaster = false
    )
    {
        // prevents an empty conversation from opening the dialogue interface
        if (lines == null || lines.Length == 0)
        {
            return;
        }

        // stops any previous typing coroutine before starting a new conversation
        StopTyping();

        // stores the conversation and starts at the first line
        currentLines = lines;
        currentLineIndex = 0;
        dialogueActive = true;

        // stores whether this is the postmaster conversation
        currentConversationIsPostmaster = isPostmaster;

        // prevents the interaction key from immediately advancing the first line
        waitingForInputRelease = true;

        // displays the speaker's name
        if (characterNameText != null)
        {
            characterNameText.text = characterName;
        }

        // opens the dialogue interface
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // starts typing the first line
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        // ends the conversation if there are no more lines
        if (currentLines == null ||
            currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // stops any previous typing coroutine
        StopTyping();

        // starts typing the current line
        typingCoroutine = StartCoroutine(
            TypeLine(currentLines[currentLineIndex])
        );
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        currentLine = line;

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        // starts the typing sound only while characters are appearing
        if (typingAudioSource != null && typingSound != null)
        {
            typingAudioSource.clip = typingSound;
            typingAudioSource.loop = true;
            typingAudioSource.Play();
        }

        // prevents division by zero if the speed is set incorrectly
        float delay = charactersPerSecond > 0f
            ? 1f / charactersPerSecond
            : 0f;

        foreach (char character in line)
        {
            if (dialogueText != null)
            {
                dialogueText.text += character;
            }

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }

        // the line has finished typing naturally
        StopTypingSound();
        isTyping = false;
        typingCoroutine = null;
    }

    private void AdvanceDialogue()
    {
        if (!dialogueActive)
        {
            return;
        }

        // first press finishes the current line without advancing
        if (isTyping)
        {
            FinishCurrentLine();
            return;
        }

        // second press advances to the next line
        currentLineIndex++;

        // closes the conversation after the final line
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // starts typing the next line
        ShowCurrentLine();
    }

    private void FinishCurrentLine()
    {
        // stops the typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // immediately displays the complete line
        if (dialogueText != null)
        {
            dialogueText.text = currentLine;
        }

        // stops the typing sound immediately
        StopTypingSound();

        isTyping = false;
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        StopTypingSound();
        isTyping = false;
    }

    private void StopTypingSound()
    {
        if (typingAudioSource != null &&
            typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }

    private void EndDialogue()
    {
        // stops typing and any associated sound
        StopTyping();

        // marks the conversation as inactive
        dialogueActive = false;

        // hides the dialogue interface
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // advances the opening tutorial only after the postmaster conversation ends
        if (currentConversationIsPostmaster &&
            OpeningTutorialManager.Instance != null)
        {
            OpeningTutorialManager.Instance.CompletePostmasterConversation();
        }

        // resets the conversation type
        currentConversationIsPostmaster = false;
    }
}