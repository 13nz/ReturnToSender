using UnityEngine;

public class Door : Interactable
{
    [Header("door settings")]
    [SerializeField] private string destinationScene;
    [SerializeField] private string doorId;

    [Header("door audio settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private float spatialBlend = 0f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 10f;

    private AudioSource audioSource;
    private AudioClip openSound;
    private AudioClip closeSound;
    private bool interactable = true;

    public string DoorId => doorId;

    public bool IsInteractable => interactable;

    private void Awake()
    {
        // gets the existing AudioSource if one is already attached.
        audioSource = GetComponent<AudioSource>();

        // adds an AudioSource if this door does not already have one.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // loads the door sounds from the Resources folder.
        openSound = Resources.Load<AudioClip>("Audio/Sounds/door_open");
        closeSound = Resources.Load<AudioClip>("Audio/Sounds/door_close");

        // configures the AudioSource.
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.priority = 128;
        audioSource.volume = volume;
        audioSource.pitch = 1f;
        audioSource.panStereo = 0f;
        audioSource.spatialBlend = spatialBlend;
        audioSource.reverbZoneMix = 1f;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        // warns if either sound could not be loaded.
        if (openSound == null)
        {
            Debug.LogWarning(
                $"could not load door_open.mp3 for {gameObject.name}. " +
                "make sure it is inside Assets/Resources/Audio/Sounds."
            );
        }

        if (closeSound == null)
        {
            Debug.LogWarning(
                $"could not load door_close.mp3 for {gameObject.name}. " +
                "make sure it is inside Assets/Resources/Audio/Sounds."
            );
        }
    }

    public void SetInteractable(bool canInteract)
    {
        interactable = canInteract;
    }

    public override void Interact()
    {
        if (!interactable)
            return;

        if (string.IsNullOrWhiteSpace(destinationScene))
        {
            Debug.LogWarning(
                $"no destination scene assigned to {gameObject.name}."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(doorId))
        {
            Debug.LogWarning(
                $"no door id assigned to {gameObject.name}."
            );
            return;
        }


        SceneTransitionManager.LoadScene(destinationScene, doorId);
    }

    public void PlayOpenSound()
    {
        // plays when the player enters a scene through this door.
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }

    public void PlayCloseSound()
    {
        // plays when the player exits a scene through this door.
        if (closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }
    }
}