using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // prevents duplicate music managers when changing scenes.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // keeps the music playing between scenes.
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("no audio source was found on the music manager.");
            enabled = false;
            return;
        }

        // makes sure the music is treated as non-spatial background music.
        audioSource.spatialBlend = 0f;
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        // prevents the music from starting again if the object is already playing.
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}