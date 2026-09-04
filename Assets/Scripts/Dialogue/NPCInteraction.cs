using UnityEngine;

public class NPCInteraction : Interactable
{
    [Header("npc settings")]
    [SerializeField] private string npcId;
    [SerializeField] private string characterName = "Character";

    [Header("dialogue settings")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    [TextArea(2, 5)]
    [SerializeField] private string journalInformation;

    [Header("opening tutorial")]
    [SerializeField] private bool isPostmaster;

    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    public override void Interact()
    {
        if (dialogueManager == null)
        {
            Debug.LogWarning("no dialogue manager was found in the scene.");
            return;
        }

        if (dialogueManager.IsDialogueActive)
            return;

        dialogueManager.StartDialogue(
            characterName,
            dialogueLines,
            isPostmaster,
            npcId,
            characterName,
            journalInformation
        );
    }
}