using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // stores the single game manager instance used throughout the game.
    public static GameManager Instance { get; private set; }

    // stores the names and information for npcs the player has discovered.
    private readonly Dictionary<string, NPCRecord> npcRecords = new();

    // stores the current story progression so other systems can react to important discoveries.
    public int StoryState { get; private set; }

    private void Awake()
    {
        // prevents duplicate game managers when scenes are loaded.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // stores this object as the single game manager instance.
        Instance = this;

        // keeps the game manager alive when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    public void RecordNPCConversation(
        string npcId,
        string npcName,
        string information
    )
    {
        // prevents invalid npc records from being added to the game state.
        if (string.IsNullOrWhiteSpace(npcId))
            return;

        // creates a new record the first time the player speaks to this npc.
        if (!npcRecords.TryGetValue(npcId, out NPCRecord record))
        {
            record = new NPCRecord(npcId, npcName);
            npcRecords.Add(npcId, record);
        }

        // adds new information to the npc's journal record without creating duplicates.
        if (!string.IsNullOrWhiteSpace(information) &&
            !record.Information.Contains(information))
        {
            record.Information.Add(information);
        }
    }

    public bool HasSpokenToNPC(string npcId)
    {
        // checks whether the player has previously interacted with this npc.
        return npcRecords.ContainsKey(npcId);
    }

    public IReadOnlyDictionary<string, NPCRecord> GetNPCRecords()
    {
        // provides read-only access to discovered npc information for the journal UI.
        return npcRecords;
    }

    public void SetStoryState(int newState)
    {
        // updates the global story progression when the player reaches a new milestone.
        StoryState = newState;
    }
}

public class NPCRecord
{
    public string Id { get; }
    public string Name { get; }
    public List<string> Information { get; }

    public NPCRecord(string id, string name)
    {
        // stores the npc's unique identifier and display name.
        Id = id;
        Name = name;

        // creates the list that will hold information learned from this npc.
        Information = new List<string>();
    }
}