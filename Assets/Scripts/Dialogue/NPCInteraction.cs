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

    private DialogueManager dialogueManager;

    private void Awake()
    {
        // finds the shared dialogue manager so every npc can use the same dialogue system.
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    public override void Interact()
    {
        // prevents the interaction from failing if the dialogue manager is unavailable.
        if (dialogueManager == null)
        {
            Debug.LogWarning("no dialogue manager was found in the scene.");
            return;
        }

        // records that the player has spoken to this npc before starting the conversation.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordNPCConversation(
                npcId,
                characterName,
                journalInformation
            );
        }

        // starts this npc's conversation using the dialogue lines assigned in the inspector.
        dialogueManager.StartDialogue(characterName, dialogueLines);
    }
}