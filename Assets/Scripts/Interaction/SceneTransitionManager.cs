using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // keeps one transition manager alive across all scenes.
    private static SceneTransitionManager instance;

    // stores the door id that the player should arrive at after the scene loads.
    private static string destinationDoorId;

    // tracks whether a scene transition is currently waiting to be completed.
    private static bool transitionPending;

    private void Awake()
    {
        // prevents duplicate transition managers if another scene contains one accidentally.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // keeps this manager alive when the current scene is replaced.
        DontDestroyOnLoad(gameObject);

        // listens for scenes finishing their load so the player can be positioned at the correct door.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void LoadScene(string sceneName, string doorId)
    {
        // prevents a transition from being started without a valid destination scene.
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("cannot load a scene because the destination scene is empty.");
            return;
        }

        // prevents the destination scene from loading without a door to spawn at.
        if (string.IsNullOrWhiteSpace(doorId))
        {
            Debug.LogWarning("cannot load a scene because the destination door id is empty.");
            return;
        }

        // stores the door that should be used as the player's arrival point.
        destinationDoorId = doorId;

        // tells the scene-loaded event that this load was caused by a door interaction.
        transitionPending = true;

        // loads the requested destination scene.
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ignores normal scene loading that was not caused by a door interaction.
        if (!transitionPending)
            return;

        // clears the pending state so this transition is processed only once.
        transitionPending = false;

        // finds the persistent player in the newly loaded scene.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("no persistent player with the 'Player' tag was found.");
            return;
        }

        // finds all doors in the destination scene so the matching arrival door can be located.
        Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);

        foreach (Door door in doors)
        {
            // ignores doors that do not represent the doorway used for this transition.
            if (door.DoorId != destinationDoorId)
                continue;

            // the player enters an interior from above the door and returns to Main from below it.
            Vector3 spawnOffset = scene.name == "Main"
                ? Vector3.down
                : Vector3.up;

            // 24 pixels at 24 pixels per unit equals one Unity unit.
            player.transform.position = door.transform.position + spawnOffset;

            return;
        }

        Debug.LogWarning(
            $"no door with id '{destinationDoorId}' was found in scene '{scene.name}'."
        );
    }

    private void OnDestroy()
    {
        // removes the scene event listener when this manager is destroyed.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}