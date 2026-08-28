using UnityEngine;

public class NPCInteraction : Interactable
{
    [Header("dialogue settings")]
    [SerializeField] private string characterName = "Character";

    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    private DialogueManager dialogueManager;

    private void Awake()
    {
        // finds the shared dialogue manager so every npc can use the same dialogue system.
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }

    public override void Interact()
    {
        // prevents the interaction from failing silently if the dialogue manager is missing.
        if (dialogueManager == null)
        {
            Debug.LogWarning("no dialogue manager was found in the scene.");
            return;
        }

        // starts this npc's conversation using the dialogue lines assigned in the inspector.
        dialogueManager.StartDialogue(characterName, dialogueLines);
    }
}