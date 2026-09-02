using UnityEngine;

public class TimedParticleSystem : MonoBehaviour
{
    [Header("active times")]
    [SerializeField] private GameTime[] activeTimes;

    private ParticleSystem particleSystem;

    private void Awake()
    {
        // gets the particle system attached to this object.
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        // updates the particle system when the scene loads.
        UpdateParticleSystem();
    }

    public void UpdateParticleSystem()
    {
        // makes sure the game manager and particle system are available.
        if (GameManager.Instance == null || particleSystem == null)
            return;

        // checks whether the current game time is one of the selected active times.
        bool shouldBeActive = false;

        foreach (GameTime activeTime in activeTimes)
        {
            if (GameManager.Instance.CurrentTime == activeTime)
            {
                shouldBeActive = true;
                break;
            }
        }

        // starts or stops the particle system based on the current time.
        if (shouldBeActive)
        {
            if (!particleSystem.isPlaying)
                particleSystem.Play();
        }
        else
        {
            if (particleSystem.isPlaying)
                particleSystem.Stop();
        }
    }
}