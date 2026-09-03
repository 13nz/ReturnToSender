using UnityEngine;

public class SeaSoundDistanceFade : MonoBehaviour
{
    [Header("player y positions")]
    [SerializeField] private float loudestPlayerY = 0f;
    [SerializeField] private float quietestPlayerY = 20f;

    [Header("volume settings")]
    [SerializeField] private float maximumVolume = 0.7f;
    [SerializeField] private float minimumVolume = 0f;

    private AudioSource audioSource;
    private Transform player;

    private void Awake()
    {
        // gets the AudioSource attached to this object.
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning(
                $"no AudioSource found on {gameObject.name}."
            );

            enabled = false;
            return;
        }

        // uses 2D audio because volume is controlled manually by player y.
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = true;
        audioSource.loop = true;
    }

    private void Start()
    {
        // finds the persistent player after the scene has loaded.
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning("no player with the 'Player' tag was found.");
            enabled = false;
            return;
        }

        player = playerObject.transform;
    }

    private void Update()
    {
        if (player == null)
            return;

        // uses only the player's y position.
        float playerY = player.position.y;

        // converts the y position into a value between 0 and 1.
        float fadeAmount = Mathf.InverseLerp(
            quietestPlayerY,
            loudestPlayerY,
            playerY
        );

        // changes the sea volume based only on player y.
        audioSource.volume = Mathf.Lerp(
            minimumVolume,
            maximumVolume,
            fadeAmount
        );
    }
}