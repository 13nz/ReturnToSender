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

        // records the conversation and checks whether this is the first time
        // the player has spoken to this npc.
        if (GameManager.Instance != null)
        {
            bool firstConversation = GameManager.Instance.RecordNPCConversation(
                npcId,
                characterName,
                journalInformation
            );

            // only advances the story checkpoint the first time this npc
            // is spoken to.
            if (firstConversation)
            {
                GameManager.Instance.CompleteNPCCheckpoint(npcId);
            }
        }

        // starts this npc's conversation using the dialogue lines assigned in the inspector.
        dialogueManager.StartDialogue(characterName, dialogueLines);
    }
}