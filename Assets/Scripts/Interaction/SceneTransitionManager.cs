using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    // keeps one transition manager alive across all scenes.
    private static SceneTransitionManager instance;

    // stores the door id that the player should arrive at after the scene loads.
    private static string destinationDoorId;

    // tracks whether a scene transition is currently waiting to be completed.
    private static bool transitionPending;

    [Header("fade settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isTransitioning;

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

        // finds the fade canvas group if it was not assigned in the Inspector.
        if (fadeCanvasGroup == null)
        {
            GameObject fadeObject = GameObject.Find("BlackFade");

            if (fadeObject != null)
                fadeCanvasGroup = fadeObject.GetComponent<CanvasGroup>();
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        // listens for scenes finishing their load.
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

        // prevents duplicate transition requests.
        if (instance == null || instance.isTransitioning)
            return;

        destinationDoorId = doorId;
        transitionPending = true;

        instance.StartCoroutine(instance.FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        isTransitioning = true;

        // fades the screen to black before loading the new scene.
        yield return FadeTo(1f);

        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ignores normal scene loading that was not caused by a door interaction.
        if (!transitionPending)
            return;

        transitionPending = false;

        // finds the persistent player in the newly loaded scene.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("no persistent player with the 'Player' tag was found.");
        }
        else
        {
            // finds all doors in the destination scene.
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

                // places the player at the correct side of the destination door.
                player.transform.position = door.transform.position + spawnOffset;

                // plays the opening sound after the player arrives.
                door.PlayOpenSound();

                break;
            }
        }

        // fades the screen back in after the destination scene is ready.
        StartCoroutine(FinishTransition());
    }

    private IEnumerator FinishTransition()
    {
        yield return FadeTo(0f);
        isTransitioning = false;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
            yield break;

        float startingAlpha = fadeCanvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float fadeAmount = Mathf.Clamp01(timer / fadeDuration);

            fadeCanvasGroup.alpha = Mathf.Lerp(
                startingAlpha,
                targetAlpha,
                fadeAmount
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    private void OnDestroy()
    {
        // removes the scene event listener when this manager is destroyed.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}