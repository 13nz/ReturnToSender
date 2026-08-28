using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class JournalUI : MonoBehaviour
{
    [Header("journal ui")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private Transform npcList;

    [Header("journal entry")]
    [SerializeField] private GameObject journalEntryPrefab;

    private bool journalOpen;

    private void Awake()
    {
        // keeps the journal canvas and its children alive when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // makes sure the journal starts closed when the game begins.
        SetJournalOpen(false);
    }

    private void Update()
    {
        // checks for Tab so the player can open and close the journal.
        if (Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleJournal();
        }
    }

    private void ToggleJournal()
    {
        // switches the journal between its open and closed states.
        SetJournalOpen(!journalOpen);
    }

    private void SetJournalOpen(bool open)
    {
        // stores the current journal state so other systems know whether it is open.
        journalOpen = open;

        // shows or hides the paper.
        journalPanel.SetActive(journalOpen);

        // refreshes the entries whenever the journal is opened.
        if (journalOpen)
        {
            RefreshJournal();
        }
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