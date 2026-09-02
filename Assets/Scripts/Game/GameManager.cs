using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameTime
{
    Morning,
    LateMorning,
    Noon,
    Afternoon,
    LateAfternoon,
    Evening,
    Night,
    LateNight
}

[System.Serializable]
public class NPCCheckpoint
{
    public string npcId;
    public GameTime time;
}

public class GameManager : MonoBehaviour
{
    // stores the single game manager instance used throughout the game.
    public static GameManager Instance { get; private set; }

    // stores the names and information for npcs the player has discovered.
    private readonly Dictionary<string, NPCRecord> npcRecords = new();

    [Header("story progression")]
    // stores the current story progression so other systems can react to important discoveries.
    public int StoryState { get; private set; }

    [Header("npc checkpoints")]
    // stores the ordered npc checkpoints that control story progression and time.
    [SerializeField] private List<NPCCheckpoint> npcCheckpoints = new();

    // stores the current time of day.
    public GameTime CurrentTime { get; private set; }

    // stores which checkpoint the player is currently expected to complete.
    public int CurrentCheckpoint { get; private set; }

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

        // listens for scenes being loaded so building states can be refreshed when returning to main.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // starts the game at the first checkpoint's assigned time.
        if (npcCheckpoints.Count > 0)
        {
            CurrentTime = npcCheckpoints[0].time;
        }
    }

    public bool RecordNPCConversation(
        string npcId,
        string npcName,
        string information
    )
    {
        // prevents invalid npc records from being added to the game state.
        if (string.IsNullOrWhiteSpace(npcId))
            return false;

        bool firstConversation = false;

        // creates a new record the first time the player speaks to this npc.
        if (!npcRecords.TryGetValue(npcId, out NPCRecord record))
        {
            record = new NPCRecord(npcId, npcName);
            npcRecords.Add(npcId, record);

            // marks this as the first conversation with this npc.
            firstConversation = true;
        }

        // adds new information to the npc's journal record without creating duplicates.
        if (!string.IsNullOrWhiteSpace(information) &&
            !record.Information.Contains(information))
        {
            record.Information.Add(information);
        }

        return firstConversation;
    }

    public bool HasSpokenToNPC(string npcId)
    {
        // checks whether the player has previously interacted with this npc.
        return npcRecords.ContainsKey(npcId);
    }

    public IReadOnlyDictionary<string, NPCRecord> GetNPCRecords()
    {
        // provides read-only access to discovered npc information for the journal ui.
        return npcRecords;
    }

    public void CompleteNPCCheckpoint(string npcId)
    {
        // prevents checkpoint progression when the id is invalid.
        if (string.IsNullOrWhiteSpace(npcId))
            return;

        // prevents checkpoint progression if no checkpoints have been configured.
        if (npcCheckpoints.Count == 0)
            return;

        // prevents the player from progressing past the end of the checkpoint list.
        if (CurrentCheckpoint >= npcCheckpoints.Count)
            return;

        // gets the checkpoint that matches the npc id.
        int checkpointIndex = npcCheckpoints.FindIndex(
            checkpoint => checkpoint.npcId == npcId
        );

        // ignores npc ids that are not part of the checkpoint sequence.
        if (checkpointIndex == -1)
            return;

        // prevents the player from skipping ahead in the checkpoint sequence.
        if (checkpointIndex != CurrentCheckpoint)
            return;

        // changes the game time to the time assigned to this checkpoint.
        CurrentTime = npcCheckpoints[checkpointIndex].time;

        // advances to the next checkpoint.
        CurrentCheckpoint++;

    }

    public void SetStoryState(int newState)
    {
        // updates the global story progression when the player reaches a new milestone.
        StoryState = newState;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // only refreshes building states when the main scene is loaded.
        if (scene.name == "Main")
        {
            RefreshBuildings();
        }
    }

    private void RefreshBuildings()
    {
        Building[] buildings = FindObjectsByType<Building>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Building building in buildings)
        {
            building.UpdateBuildingState();
        }

        NightLight[] lights = FindObjectsByType<NightLight>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (NightLight light in lights)
        {
            light.UpdateLight();
        }

        TimedParticleSystem[] particleSystems = FindObjectsByType<TimedParticleSystem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (TimedParticleSystem particleSystem in particleSystems)
        {
            particleSystem.UpdateParticleSystem();
        }
    }

    public NPCCheckpoint GetCurrentNPCCheckpoint()
    {
        // prevents invalid checkpoint indexes from causing errors.
        if (CurrentCheckpoint >= npcCheckpoints.Count)
            return null;

        // returns the npc checkpoint currently being played.
        return npcCheckpoints[CurrentCheckpoint];
    }

    private void OnDestroy()
    {
        // stops the game manager from listening for scene changes after it is destroyed.
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
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