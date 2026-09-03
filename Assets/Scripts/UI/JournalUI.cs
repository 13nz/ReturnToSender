using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [Header("document ui")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject envelopePanel;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [Header("journal list")]
    [SerializeField] private Transform npcList;

    [Header("journal entry")]
    [SerializeField] private GameObject journalEntryPrefab;


    // sounds 
    private AudioSource audioSource;
    private AudioClip paperRustleSound;
    private bool journalOpen;

    public bool IsJournalOpen => journalOpen;

    // 0 = journal, 1 = envelope
    private int currentPage;

    private void Awake()
    {
        // dound effect
        // gets the existing AudioSource if one is already attached.
        audioSource = GetComponent<AudioSource>();

        // adds an AudioSource if one is missing.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // loads the paper rustle sound from Resources.
        paperRustleSound =
            Resources.Load<AudioClip>("Audio/Sounds/paper_rustle");

        // configures the AudioSource.
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.pitch = 1f;
        audioSource.panStereo = 0f;
        audioSource.reverbZoneMix = 1f;
        // keeps the journal canvas and its children alive when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // starts with the documents interface closed.
        SetJournalOpen(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // checks for Tab so the player can open and close the documents interface.
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleJournal();
            return;
        }

        // ignores page controls while the documents interface is closed.
        if (!journalOpen)
            return;

        // switches to the previous page with the left arrow key.
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            PreviousPage();
        }

        // switches to the next page with the right arrow key.
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            NextPage();
        }
    }

    private void ToggleJournal()
    {
        // plays the paper rustle when the notes open.
        if (paperRustleSound != null)
        {
            audioSource.PlayOneShot(paperRustleSound);
        }
        // switches the documents interface between its open and closed states.
        SetJournalOpen(!journalOpen);
    }

    private void SetJournalOpen(bool open)
    {
        // stores the current open state.
        journalOpen = open;

        if (!journalOpen)
        {
            // hides the journal and envelope when the interface is closed.
            journalPanel.SetActive(false);
            envelopePanel.SetActive(false);

            // hides the navigation arrows while the interface is closed.
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);

            return;
        }

        // always start on the journal when opening the documents interface.
        currentPage = 0;

        // refreshes the journal so it contains the latest npc information.
        RefreshJournal();

        // displays the current page.
        ShowCurrentPage();
    }

    public void PreviousPage()
    {
        
        // ignores button presses while the documents interface is closed.
        if (!journalOpen)
            return;

        // plays the paper rustle when the notes open.
        if (paperRustleSound != null)
        {
            audioSource.PlayOneShot(paperRustleSound);
        }

        // moves to the previous page.
        currentPage--;

        // wraps from the journal back to the envelope.
        if (currentPage < 0)
        {
            currentPage = 1;
        }

        ShowCurrentPage();
    }

    public void NextPage()
    {
        // ignores button presses while the documents interface is closed.
        if (!journalOpen)
            return;

        // plays the paper rustle when the notes open.
        if (paperRustleSound != null)
        {
            audioSource.PlayOneShot(paperRustleSound);
        }

        // moves to the next page.
        currentPage++;

        // wraps from the envelope back to the journal.
        if (currentPage > 1)
        {
            currentPage = 0;
        }

        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        // shows the journal when the current page is zero.
        journalPanel.SetActive(currentPage == 0);

        // shows the envelope when the current page is one.
        envelopePanel.SetActive(currentPage == 1);

        // keeps the navigation arrows visible while the documents interface is open.
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

    private void RefreshJournal()
    {
        // prevents the journal from trying to read game data before the game manager exists.
        if (GameManager.Instance == null)
            return;

        // removes the previous entries so the journal reflects the current game state.
        foreach (Transform child in npcList)
        {
            Destroy(child.gameObject);
        }

        // gets the npc records that the player has discovered.
        IReadOnlyDictionary<string, NPCRecord> records =
            GameManager.Instance.GetNPCRecords();

        // creates one entry for every npc the player has spoken to.
        foreach (NPCRecord record in records.Values)
        {
            GameObject entry = Instantiate(
                journalEntryPrefab,
                npcList
            );

            // finds the text components belonging to this journal entry.
            TMP_Text[] textFields = entry.GetComponentsInChildren<TMP_Text>();

            // displays the npc's name in the first text field.
            if (textFields.Length > 0)
            {
                textFields[0].text = record.Name;
            }

            // displays the information learned from the npc in the second text field.
            if (textFields.Length > 1)
            {
                textFields[1].text = string.Join("\n", record.Information);
            }
        }
    }
}